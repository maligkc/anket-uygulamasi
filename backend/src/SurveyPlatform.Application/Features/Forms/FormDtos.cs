using SurveyPlatform.Domain.Enums;

namespace SurveyPlatform.Application.Features.Forms;

public class FormListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublished { get; set; }
    public string? ShareKey { get; set; }
    public int ResponseCount { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class QuestionOptionDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class QuestionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public List<QuestionOptionDto> Options { get; set; } = [];
}

public class FormDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublished { get; set; }
    public string? ShareKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<QuestionDto> Questions { get; set; } = [];
}
