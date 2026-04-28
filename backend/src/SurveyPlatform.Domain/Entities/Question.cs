using SurveyPlatform.Domain.Enums;

namespace SurveyPlatform.Domain.Entities;

public class Question
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string Title { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }

    public Form Form { get; set; } = null!;
    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
