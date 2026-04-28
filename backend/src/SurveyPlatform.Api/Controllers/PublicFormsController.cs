using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyPlatform.Application.Features.Public;

namespace SurveyPlatform.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/public/forms")]
public class PublicFormsController : ControllerBase
{
    private readonly IPublicFormsService _publicFormsService;

    public PublicFormsController(IPublicFormsService publicFormsService)
    {
        _publicFormsService = publicFormsService;
    }

    [HttpGet("{shareKey}")]
    public async Task<ActionResult<PublicFormDto>> GetPublicFormByShareKey(string shareKey, CancellationToken cancellationToken)
    {
        var form = await _publicFormsService.GetPublicFormByShareKeyAsync(shareKey, cancellationToken);
        return Ok(form);
    }

    [HttpPost("{shareKey}/responses")]
    public async Task<ActionResult<SubmitResponseResultDto>> SubmitResponse(string shareKey, SubmitResponseRequest request, CancellationToken cancellationToken)
    {
        var result = await _publicFormsService.SubmitResponseAsync(shareKey, request, cancellationToken);
        return Ok(result);
    }
}
