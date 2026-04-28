using System.ComponentModel.DataAnnotations;

namespace SurveyPlatform.Application.Features.Public;

public class SubmitAnswerRequest
{
    [Required]
    public Guid QuestionId { get; set; }

    [MaxLength(5000)]
    public string? Value { get; set; }

    public List<Guid> SelectedOptionIds { get; set; } = [];
}

public class SubmitResponseRequest
{
    [Required]
    public List<SubmitAnswerRequest> Answers { get; set; } = [];
}
