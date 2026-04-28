using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyPlatform.Application.Features.Forms;

namespace SurveyPlatform.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/forms/{formId:guid}/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IFormsService _formsService;

    public QuestionsController(IFormsService formsService)
    {
        _formsService = formsService;
    }

    [HttpPost]
    public async Task<ActionResult<QuestionDto>> AddQuestion(Guid formId, AddQuestionRequest request, CancellationToken cancellationToken)
    {
        var question = await _formsService.AddQuestionAsync(formId, request, cancellationToken);
        return Ok(question);
    }

    [HttpPut("{questionId:guid}")]
    public async Task<ActionResult<QuestionDto>> UpdateQuestion(Guid formId, Guid questionId, UpdateQuestionRequest request, CancellationToken cancellationToken)
    {
        var question = await _formsService.UpdateQuestionAsync(formId, questionId, request, cancellationToken);
        return Ok(question);
    }

    [HttpDelete("{questionId:guid}")]
    public async Task<IActionResult> DeleteQuestion(Guid formId, Guid questionId, CancellationToken cancellationToken)
    {
        await _formsService.DeleteQuestionAsync(formId, questionId, cancellationToken);
        return NoContent();
    }
}
