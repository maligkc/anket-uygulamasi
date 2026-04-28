namespace SurveyPlatform.Domain.Entities;

public class FormResponse
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public DateTime SubmittedAt { get; set; }

    public Form Form { get; set; } = null!;
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
