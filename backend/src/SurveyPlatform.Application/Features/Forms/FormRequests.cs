using System.ComponentModel.DataAnnotations;
using SurveyPlatform.Domain.Enums;

namespace SurveyPlatform.Application.Features.Forms;

public class CreateFormRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}

public class UpdateFormRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}

public class AddQuestionRequest
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public QuestionType Type { get; set; }

    public bool IsRequired { get; set; }

    [Range(1, int.MaxValue)]
    public int? Order { get; set; }

    public List<string> Options { get; set; } = [];
}

public class UpdateQuestionRequest
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public QuestionType Type { get; set; }

    public bool IsRequired { get; set; }

    [Range(1, int.MaxValue)]
    public int Order { get; set; }

    public List<string> Options { get; set; } = [];
}
