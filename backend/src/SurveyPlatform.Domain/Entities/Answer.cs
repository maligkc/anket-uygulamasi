namespace SurveyPlatform.Domain.Entities;

public class Answer
{
    public Guid Id { get; set; }
    public Guid FormResponseId { get; set; }
    public Guid QuestionId { get; set; }
    public string? Value { get; set; }

    public FormResponse FormResponse { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public ICollection<AnswerSelection> Selections { get; set; } = new List<AnswerSelection>();
}
