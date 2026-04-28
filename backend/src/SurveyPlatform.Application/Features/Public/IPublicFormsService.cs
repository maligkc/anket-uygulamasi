namespace SurveyPlatform.Application.Features.Public;

public interface IPublicFormsService
{
    Task<PublicFormDto> GetPublicFormByShareKeyAsync(string shareKey, CancellationToken cancellationToken = default);
    Task<SubmitResponseResultDto> SubmitResponseAsync(string shareKey, SubmitResponseRequest request, CancellationToken cancellationToken = default);
}
