using SurveyPlatform.Domain.Enums;

namespace SurveyPlatform.Application.Features.Public;

public class PublicFormOptionDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class PublicFormQuestionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public List<PublicFormOptionDto> Options { get; set; } = [];
}

public class PublicFormDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PublicFormQuestionDto> Questions { get; set; } = [];
}

public class SubmitResponseResultDto
{
    public Guid ResponseId { get; set; }
    public DateTime SubmittedAt { get; set; }
}
