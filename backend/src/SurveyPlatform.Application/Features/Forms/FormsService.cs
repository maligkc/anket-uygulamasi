using System.Net;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SurveyPlatform.Application.Abstractions.Data;
using SurveyPlatform.Application.Abstractions.Services;
using SurveyPlatform.Application.Common;
using SurveyPlatform.Domain.Entities;
using SurveyPlatform.Domain.Enums;

namespace SurveyPlatform.Application.Features.Forms;

public class FormsService : IFormsService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public FormsService(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<List<FormListItemDto>> GetMyFormsAsync(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        return await _dbContext.Forms
            .AsNoTracking()
            .Where(f => f.OwnerId == userId)
            .OrderByDescending(f => f.UpdatedAt)
            .Select(f => new FormListItemDto
            {
                Id = f.Id,
                Title = f.Title,
                Description = f.Description,
                IsPublished = f.IsPublished,
                ShareKey = f.ShareKey,
                UpdatedAt = f.UpdatedAt,
                ResponseCount = f.Responses.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<FormDetailDto> GetFormByIdAsync(Guid formId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var form = await GetOwnedFormWithQuestionsAsync(formId, userId, cancellationToken);
        return MapForm(form);
    }

    public async Task<FormDetailDto> CreateFormAsync(CreateFormRequest request, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var now = DateTime.UtcNow;

        var form = new Form
        {
            Id = Guid.NewGuid(),
            OwnerId = userId,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
            IsPublished = false
        };

        _dbContext.Forms.Add(form);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapForm(form);
    }

    public async Task<FormDetailDto> UpdateFormAsync(Guid formId, UpdateFormRequest request, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var form = await GetOwnedFormWithQuestionsAsync(formId, userId, cancellationToken);

        form.Title = request.Title.Trim();
        form.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        form.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapForm(form);
    }

    public async Task DeleteFormAsync(Guid formId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var form = await _dbContext.Forms
            .FirstOrDefaultAsync(f => f.Id == formId && f.OwnerId == userId, cancellationToken);

        if (form is null)
        {
            throw new AppException("Form not found.", HttpStatusCode.NotFound);
        }

        _dbContext.Forms.Remove(form);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<FormDetailDto> PublishFormAsync(Guid formId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var form = await GetOwnedFormWithQuestionsAsync(formId, userId, cancellationToken);

        if (form.Questions.Count == 0)
        {
            throw new AppException("Form must contain at least one question before publishing.", HttpStatusCode.BadRequest);
        }

        foreach (var question in form.Questions)
        {
            if (IsChoiceType(question.Type) && question.Options.Count < 2)
            {
                throw new AppException("Choice questions must have at least two options.", HttpStatusCode.BadRequest);
            }
        }

        if (string.IsNullOrWhiteSpace(form.ShareKey))
        {
            form.ShareKey = await GenerateUniqueShareKeyAsync(cancellationToken);
        }

        form.IsPublished = true;
        form.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapForm(form);
    }

    public async Task<FormDetailDto> UnpublishFormAsync(Guid formId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var form = await GetOwnedFormWithQuestionsAsync(formId, userId, cancellationToken);

        form.IsPublished = false;
        form.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapForm(form);
    }

    public async Task<QuestionDto> AddQuestionAsync(Guid formId, AddQuestionRequest request, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var form = await _dbContext.Forms
            .Include(f => f.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(f => f.Id == formId && f.OwnerId == userId, cancellationToken);

        if (form is null)
        {
            throw new AppException("Form not found.", HttpStatusCode.NotFound);
        }

        var options = ValidateAndNormalizeOptions(request.Type, request.Options);

        var nextOrder = form.Questions.Count == 0 ? 1 : form.Questions.Max(q => q.Order) + 1;
        var order = request.Order ?? nextOrder;

        var question = new Question
        {
            Id = Guid.NewGuid(),
            FormId = formId,
            Title = request.Title.Trim(),
            Type = request.Type,
            IsRequired = request.IsRequired,
            Order = order
        };

        if (IsChoiceType(request.Type))
        {
            question.Options = options
                .Select((value, index) => new QuestionOption
                {
                    Id = Guid.NewGuid(),
                    Value = value,
                    Order = index + 1
                })
                .ToList();
        }

        _dbContext.Questions.Add(question);

        form.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapQuestion(question);
    }

    public async Task<QuestionDto> UpdateQuestionAsync(Guid formId, Guid questionId, UpdateQuestionRequest request, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var question = await _dbContext.Questions
            .Include(q => q.Form)
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == questionId && q.FormId == formId, cancellationToken);

        if (question is null || question.Form.OwnerId != userId)
        {
            throw new AppException("Question not found.", HttpStatusCode.NotFound);
        }

        var options = ValidateAndNormalizeOptions(request.Type, request.Options);

        question.Title = request.Title.Trim();
        question.Type = request.Type;
        question.IsRequired = request.IsRequired;
        question.Order = request.Order;

        if (question.Options.Count > 0)
        {
            _dbContext.QuestionOptions.RemoveRange(question.Options);
            question.Options.Clear();
        }

        if (IsChoiceType(request.Type))
        {
            foreach (var (value, index) in options.Select((option, index) => (option, index)))
            {
                question.Options.Add(new QuestionOption
                {
                    Id = Guid.NewGuid(),
                    QuestionId = question.Id,
                    Value = value,
                    Order = index + 1
                });
            }
        }

        question.Form.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapQuestion(question);
    }

    public async Task DeleteQuestionAsync(Guid formId, Guid questionId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var question = await _dbContext.Questions
            .Include(q => q.Form)
            .FirstOrDefaultAsync(q => q.Id == questionId && q.FormId == formId, cancellationToken);

        if (question is null || question.Form.OwnerId != userId)
        {
            throw new AppException("Question not found.", HttpStatusCode.NotFound);
        }

        _dbContext.Questions.Remove(question);

        question.Form.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private Guid GetCurrentUserId()
    {
        if (!_currentUserService.UserId.HasValue)
        {
            throw new AppException("Unauthorized.", HttpStatusCode.Unauthorized);
        }

        return _currentUserService.UserId.Value;
    }

    private async Task<Form> GetOwnedFormWithQuestionsAsync(Guid formId, Guid userId, CancellationToken cancellationToken)
    {
        var form = await _dbContext.Forms
            .Include(f => f.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(f => f.Id == formId && f.OwnerId == userId, cancellationToken);

        if (form is null)
        {
            throw new AppException("Form not found.", HttpStatusCode.NotFound);
        }

        return form;
    }

    private async Task<string> GenerateUniqueShareKeyAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var shareKey = Convert.ToHexString(RandomNumberGenerator.GetBytes(6)).ToLowerInvariant();
            var exists = await _dbContext.Forms.AnyAsync(f => f.ShareKey == shareKey, cancellationToken);

            if (!exists)
            {
                return shareKey;
            }
        }
    }

    private static List<string> ValidateAndNormalizeOptions(QuestionType type, IEnumerable<string> rawOptions)
    {
        if (!IsChoiceType(type))
        {
            return [];
        }

        var options = rawOptions
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (options.Count < 2)
        {
            throw new AppException("Choice questions must have at least two options.", HttpStatusCode.BadRequest);
        }

        return options;
    }

    private static bool IsChoiceType(QuestionType questionType)
    {
        return questionType is QuestionType.SingleChoice or QuestionType.MultipleChoice;
    }

    private static FormDetailDto MapForm(Form form)
    {
        return new FormDetailDto
        {
            Id = form.Id,
            Title = form.Title,
            Description = form.Description,
            IsPublished = form.IsPublished,
            ShareKey = form.ShareKey,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt,
            Questions = form.Questions
                .OrderBy(q => q.Order)
                .Select(MapQuestion)
                .ToList()
        };
    }

    private static QuestionDto MapQuestion(Question question)
    {
        return new QuestionDto
        {
            Id = question.Id,
            Title = question.Title,
            Type = question.Type,
            IsRequired = question.IsRequired,
            Order = question.Order,
            Options = question.Options
                .OrderBy(o => o.Order)
                .Select(o => new QuestionOptionDto
                {
                    Id = o.Id,
                    Value = o.Value,
                    Order = o.Order
                })
                .ToList()
        };
    }
}
