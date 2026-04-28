namespace SurveyPlatform.Domain.Entities;

public class AnswerSelection
{
    public Guid Id { get; set; }
    public Guid AnswerId { get; set; }
    public Guid QuestionOptionId { get; set; }

    public Answer Answer { get; set; } = null!;
    public QuestionOption QuestionOption { get; set; } = null!;
}
