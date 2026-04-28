using SurveyPlatform.Domain.Entities;

namespace SurveyPlatform.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
