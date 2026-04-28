using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyPlatform.Application.Features.Forms;

namespace SurveyPlatform.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/forms")]
public class FormsController : ControllerBase
{
    private readonly IFormsService _formsService;

    public FormsController(IFormsService formsService)
    {
        _formsService = formsService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FormListItemDto>>> GetMyForms(CancellationToken cancellationToken)
    {
        var forms = await _formsService.GetMyFormsAsync(cancellationToken);
        return Ok(forms);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FormDetailDto>> GetFormById(Guid id, CancellationToken cancellationToken)
    {
        var form = await _formsService.GetFormByIdAsync(id, cancellationToken);
        return Ok(form);
    }

    [HttpPost]
    public async Task<ActionResult<FormDetailDto>> CreateForm(CreateFormRequest request, CancellationToken cancellationToken)
    {
        var form = await _formsService.CreateFormAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetFormById), new { id = form.Id }, form);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FormDetailDto>> UpdateForm(Guid id, UpdateFormRequest request, CancellationToken cancellationToken)
    {
        var form = await _formsService.UpdateFormAsync(id, request, cancellationToken);
        return Ok(form);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteForm(Guid id, CancellationToken cancellationToken)
    {
        await _formsService.DeleteFormAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult<FormDetailDto>> PublishForm(Guid id, CancellationToken cancellationToken)
    {
        var form = await _formsService.PublishFormAsync(id, cancellationToken);
        return Ok(form);
    }

    [HttpPost("{id:guid}/unpublish")]
    public async Task<ActionResult<FormDetailDto>> UnpublishForm(Guid id, CancellationToken cancellationToken)
    {
        var form = await _formsService.UnpublishFormAsync(id, cancellationToken);
        return Ok(form);
    }
}
