using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyPlatform.Application.Features.Responses;

namespace SurveyPlatform.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/forms/{id:guid}/responses")]
public class ResponsesController : ControllerBase
{
    private readonly IResponsesService _responsesService;

    public ResponsesController(IResponsesService responsesService)
    {
        _responsesService = responsesService;
    }

    [HttpGet]
    public async Task<ActionResult<FormResponsesDto>> GetFormResponses(Guid id, CancellationToken cancellationToken)
    {
        var responses = await _responsesService.GetFormResponsesAsync(id, cancellationToken);
        return Ok(responses);
    }
}
