using Microsoft.EntityFrameworkCore;
using SurveyPlatform.Application.Abstractions.Data;
using SurveyPlatform.Domain.Entities;
using SurveyPlatform.Domain.Enums;

namespace SurveyPlatform.Infrastructure.Data;

public class SurveyDbContext : DbContext, IAppDbContext
{
    public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Form> Forms => Set<Form>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<FormResponse> FormResponses => Set<FormResponse>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<AnswerSelection> AnswerSelections => Set<AnswerSelection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Form>(entity =>
        {
            entity.ToTable("forms");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.ShareKey).HasMaxLength(50);
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();

            entity.HasOne(x => x.Owner)
                .WithMany(x => x.Forms)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.ShareKey)
                .IsUnique();
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("questions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(x => x.IsRequired).IsRequired();
            entity.Property(x => x.Order).IsRequired();

            entity.HasOne(x => x.Form)
                .WithMany(x => x.Questions)
                .HasForeignKey(x => x.FormId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.FormId, x.Order });
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.ToTable("question_options");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Value).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Order).IsRequired();

            entity.HasOne(x => x.Question)
                .WithMany(x => x.Options)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.QuestionId, x.Order });
        });

        modelBuilder.Entity<FormResponse>(entity =>
        {
            entity.ToTable("form_responses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SubmittedAt).IsRequired();

            entity.HasOne(x => x.Form)
                .WithMany(x => x.Responses)
                .HasForeignKey(x => x.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.ToTable("answers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Value).HasMaxLength(5000);

            entity.HasOne(x => x.FormResponse)
                .WithMany(x => x.Answers)
                .HasForeignKey(x => x.FormResponseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Question)
                .WithMany(x => x.Answers)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnswerSelection>(entity =>
        {
            entity.ToTable("answer_selections");
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Answer)
                .WithMany(x => x.Selections)
                .HasForeignKey(x => x.AnswerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.QuestionOption)
                .WithMany(x => x.AnswerSelections)
                .HasForeignKey(x => x.QuestionOptionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.AnswerId, x.QuestionOptionId }).IsUnique();
        });
    }
}
