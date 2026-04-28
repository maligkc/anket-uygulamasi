using System.Security.Claims;
using SurveyPlatform.Application.Abstractions.Services;

namespace SurveyPlatform.Api.Infrastructure;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdRaw = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdRaw, out var userId))
            {
                return userId;
            }

            return null;
        }
    }
}
