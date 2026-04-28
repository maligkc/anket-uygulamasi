using Microsoft.EntityFrameworkCore;
using SurveyPlatform.Domain.Entities;

namespace SurveyPlatform.Application.Abstractions.Data;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Form> Forms { get; }
    DbSet<Question> Questions { get; }
    DbSet<QuestionOption> QuestionOptions { get; }
    DbSet<FormResponse> FormResponses { get; }
    DbSet<Answer> Answers { get; }
    DbSet<AnswerSelection> AnswerSelections { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
