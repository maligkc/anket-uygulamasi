namespace SurveyPlatform.Domain.Entities;

public class QuestionOption
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Order { get; set; }

    public Question Question { get; set; } = null!;
    public ICollection<AnswerSelection> AnswerSelections { get; set; } = new List<AnswerSelection>();
}
