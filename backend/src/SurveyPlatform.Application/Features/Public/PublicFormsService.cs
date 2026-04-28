using System.Net;
using Microsoft.EntityFrameworkCore;
using SurveyPlatform.Application.Abstractions.Data;
using SurveyPlatform.Application.Common;
using SurveyPlatform.Domain.Entities;
using SurveyPlatform.Domain.Enums;

namespace SurveyPlatform.Application.Features.Public;

public class PublicFormsService : IPublicFormsService
{
    private readonly IAppDbContext _dbContext;

    public PublicFormsService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PublicFormDto> GetPublicFormByShareKeyAsync(string shareKey, CancellationToken cancellationToken = default)
    {
        var form = await _dbContext.Forms
            .AsNoTracking()
            .Include(f => f.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(f => f.ShareKey == shareKey && f.IsPublished, cancellationToken);

        if (form is null)
        {
            throw new AppException("Form not found.", HttpStatusCode.NotFound);
        }

        return new PublicFormDto
        {
            Id = form.Id,
            Title = form.Title,
            Description = form.Description,
            Questions = form.Questions
                .OrderBy(q => q.Order)
                .Select(q => new PublicFormQuestionDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Type = q.Type,
                    IsRequired = q.IsRequired,
                    Order = q.Order,
                    Options = q.Options
                        .OrderBy(o => o.Order)
                        .Select(o => new PublicFormOptionDto
                        {
                            Id = o.Id,
                            Value = o.Value,
                            Order = o.Order
                        })
                        .ToList()
                })
                .ToList()
        };
    }

    public async Task<SubmitResponseResultDto> SubmitResponseAsync(string shareKey, SubmitResponseRequest request, CancellationToken cancellationToken = default)
    {
        var form = await _dbContext.Forms
            .Include(f => f.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(f => f.ShareKey == shareKey && f.IsPublished, cancellationToken);

        if (form is null)
        {
            throw new AppException("Form not found.", HttpStatusCode.NotFound);
        }

        var answers = request.Answers ?? [];

        var duplicateQuestionIds = answers
            .GroupBy(a => a.QuestionId)
            .Where(g => g.Key != Guid.Empty && g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateQuestionIds.Count > 0)
        {
            throw new AppException("Each question can only be answered once.", HttpStatusCode.BadRequest);
        }

        var questionIds = form.Questions.Select(q => q.Id).ToHashSet();

        if (answers.Any(a => !questionIds.Contains(a.QuestionId)))
        {
            throw new AppException("Response contains invalid question ids.", HttpStatusCode.BadRequest);
        }

        var answerLookup = answers.ToDictionary(a => a.QuestionId);

        var formResponse = new FormResponse
        {
            Id = Guid.NewGuid(),
            FormId = form.Id,
            SubmittedAt = DateTime.UtcNow
        };

        foreach (var question in form.Questions)
        {
            answerLookup.TryGetValue(question.Id, out var submittedAnswer);

            var answer = BuildAnswer(question, submittedAnswer);

            if (answer is not null)
            {
                answer.Id = Guid.NewGuid();
                answer.FormResponseId = formResponse.Id;
                answer.QuestionId = question.Id;
                formResponse.Answers.Add(answer);
            }
        }

        _dbContext.FormResponses.Add(formResponse);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SubmitResponseResultDto
        {
            ResponseId = formResponse.Id,
            SubmittedAt = formResponse.SubmittedAt
        };
    }

    private static Answer? BuildAnswer(Question question, SubmitAnswerRequest? submitted)
    {
        if (question.Type is QuestionType.ShortText or QuestionType.Paragraph)
        {
            var value = submitted?.Value?.Trim();

            if (submitted is not null && submitted.SelectedOptionIds.Count > 0)
            {
                throw new AppException("Text questions cannot contain selected options.", HttpStatusCode.BadRequest);
            }

            if (question.IsRequired && string.IsNullOrWhiteSpace(value))
            {
                throw new AppException($"Question '{question.Title}' is required.", HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return new Answer
            {
                Value = value
            };
        }

        if (submitted is not null && !string.IsNullOrWhiteSpace(submitted.Value))
        {
            throw new AppException("Choice questions cannot contain text value.", HttpStatusCode.BadRequest);
        }

        var selectedOptionIds = (submitted?.SelectedOptionIds ?? [])
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (question.IsRequired && selectedOptionIds.Count == 0)
        {
            throw new AppException($"Question '{question.Title}' is required.", HttpStatusCode.BadRequest);
        }

        if (question.Type == QuestionType.SingleChoice && selectedOptionIds.Count > 1)
        {
            throw new AppException($"Question '{question.Title}' allows only one option.", HttpStatusCode.BadRequest);
        }

        if (selectedOptionIds.Count == 0)
        {
            return null;
        }

        var validOptionIds = question.Options.Select(o => o.Id).ToHashSet();

        if (selectedOptionIds.Any(optionId => !validOptionIds.Contains(optionId)))
        {
            throw new AppException($"Question '{question.Title}' contains invalid option ids.", HttpStatusCode.BadRequest);
        }

        var answer = new Answer();

        foreach (var optionId in selectedOptionIds)
        {
            answer.Selections.Add(new AnswerSelection
            {
                Id = Guid.NewGuid(),
                QuestionOptionId = optionId
            });
        }

        return answer;
    }
}
