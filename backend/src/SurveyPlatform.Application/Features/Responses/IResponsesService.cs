namespace SurveyPlatform.Application.Features.Responses;

public interface IResponsesService
{
    Task<FormResponsesDto> GetFormResponsesAsync(Guid formId, CancellationToken cancellationToken = default);
}
