namespace SurveyPlatform.Application.Features.Forms;

public interface IFormsService
{
    Task<List<FormListItemDto>> GetMyFormsAsync(CancellationToken cancellationToken = default);
    Task<FormDetailDto> GetFormByIdAsync(Guid formId, CancellationToken cancellationToken = default);
    Task<FormDetailDto> CreateFormAsync(CreateFormRequest request, CancellationToken cancellationToken = default);
    Task<FormDetailDto> UpdateFormAsync(Guid formId, UpdateFormRequest request, CancellationToken cancellationToken = default);
    Task DeleteFormAsync(Guid formId, CancellationToken cancellationToken = default);
    Task<FormDetailDto> PublishFormAsync(Guid formId, CancellationToken cancellationToken = default);
    Task<FormDetailDto> UnpublishFormAsync(Guid formId, CancellationToken cancellationToken = default);
    Task<QuestionDto> AddQuestionAsync(Guid formId, AddQuestionRequest request, CancellationToken cancellationToken = default);
    Task<QuestionDto> UpdateQuestionAsync(Guid formId, Guid questionId, UpdateQuestionRequest request, CancellationToken cancellationToken = default);
    Task DeleteQuestionAsync(Guid formId, Guid questionId, CancellationToken cancellationToken = default);
}
