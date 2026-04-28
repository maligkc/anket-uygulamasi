namespace SurveyPlatform.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
