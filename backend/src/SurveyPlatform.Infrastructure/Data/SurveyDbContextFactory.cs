using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SurveyPlatform.Infrastructure.Data;

public class SurveyDbContextFactory : IDesignTimeDbContextFactory<SurveyDbContext>
{
    public SurveyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SurveyDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=survey_platform;Username=mali;Password=");

        return new SurveyDbContext(optionsBuilder.Options);
    }
}
