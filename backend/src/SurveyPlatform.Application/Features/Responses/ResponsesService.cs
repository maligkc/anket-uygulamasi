using System.Net;
using Microsoft.EntityFrameworkCore;
using SurveyPlatform.Application.Abstractions.Data;
using SurveyPlatform.Application.Abstractions.Services;
using SurveyPlatform.Application.Common;
using SurveyPlatform.Domain.Enums;

namespace SurveyPlatform.Application.Features.Responses;

public class ResponsesService : IResponsesService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public ResponsesService(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<FormResponsesDto> GetFormResponsesAsync(Guid formId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var form = await _dbContext.Forms
            .Include(f => f.Questions)
            .ThenInclude(q => q.Options)
            .Include(f => f.Responses)
            .ThenInclude(r => r.Answers)
            .ThenInclude(a => a.Selections)
            .ThenInclude(s => s.QuestionOption)
            .FirstOrDefaultAsync(f => f.Id == formId && f.OwnerId == userId, cancellationToken);

        if (form is null)
        {
            throw new AppException("Form not found.", HttpStatusCode.NotFound);
        }

        var questionMap = form.Questions.ToDictionary(q => q.Id);

        var submissions = form.Responses
            .OrderByDescending(r => r.SubmittedAt)
            .Select(response => new FormSubmissionDto
            {
                ResponseId = response.Id,
                SubmittedAt = response.SubmittedAt,
                Answers = response.Answers
                    .Where(a => questionMap.ContainsKey(a.QuestionId))
                    .OrderBy(a => questionMap[a.QuestionId].Order)
                    .Select(answer =>
                    {
                        var question = questionMap[answer.QuestionId];

                        return new SubmittedAnswerDto
                        {
                            QuestionId = question.Id,
                            QuestionTitle = question.Title,
                            QuestionType = question.Type,
                            Value = answer.Value,
                            SelectedOptions = answer.Selections
                                .OrderBy(selection => selection.QuestionOption.Order)
                                .Select(selection => selection.QuestionOption.Value)
                                .ToList()
                        };
                    })
                    .ToList()
            })
            .ToList();

        var choiceSummaries = form.Questions
            .Where(question => question.Type is QuestionType.SingleChoice or QuestionType.MultipleChoice)
            .OrderBy(question => question.Order)
            .Select(question =>
            {
                var optionCounts = form.Responses
                    .SelectMany(response => response.Answers)
                    .Where(answer => answer.QuestionId == question.Id)
                    .SelectMany(answer => answer.Selections)
                    .GroupBy(selection => selection.QuestionOptionId)
                    .ToDictionary(group => group.Key, group => group.Count());

                return new ChoiceQuestionSummaryDto
                {
                    QuestionId = question.Id,
                    QuestionTitle = question.Title,
                    Options = question.Options
                        .OrderBy(option => option.Order)
                        .Select(option => new ChoiceOptionCountDto
                        {
                            OptionId = option.Id,
                            OptionValue = option.Value,
                            Count = optionCounts.GetValueOrDefault(option.Id)
                        })
                        .ToList()
                };
            })
            .ToList();

        return new FormResponsesDto
        {
            FormId = form.Id,
            FormTitle = form.Title,
            ResponseCount = form.Responses.Count,
            Submissions = submissions,
            ChoiceSummaries = choiceSummaries
        };
    }

    private Guid GetCurrentUserId()
    {
        if (!_currentUserService.UserId.HasValue)
        {
            throw new AppException("Unauthorized.", HttpStatusCode.Unauthorized);
        }

        return _currentUserService.UserId.Value;
    }
}
