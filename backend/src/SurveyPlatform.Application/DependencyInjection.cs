using Microsoft.Extensions.DependencyInjection;
using SurveyPlatform.Application.Features.Auth;
using SurveyPlatform.Application.Features.Forms;
using SurveyPlatform.Application.Features.Public;
using SurveyPlatform.Application.Features.Responses;

namespace SurveyPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFormsService, FormsService>();
        services.AddScoped<IPublicFormsService, PublicFormsService>();
        services.AddScoped<IResponsesService, ResponsesService>();

        return services;
    }
}
