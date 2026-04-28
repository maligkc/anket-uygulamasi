using SurveyPlatform.Domain.Enums;

namespace SurveyPlatform.Application.Features.Responses;

public class ChoiceOptionCountDto
{
    public Guid OptionId { get; set; }
    public string OptionValue { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ChoiceQuestionSummaryDto
{
    public Guid QuestionId { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public List<ChoiceOptionCountDto> Options { get; set; } = [];
}

public class SubmittedAnswerDto
{
    public Guid QuestionId { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string? Value { get; set; }
    public List<string> SelectedOptions { get; set; } = [];
}

public class FormSubmissionDto
{
    public Guid ResponseId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<SubmittedAnswerDto> Answers { get; set; } = [];
}

public class FormResponsesDto
{
    public Guid FormId { get; set; }
    public string FormTitle { get; set; } = string.Empty;
    public int ResponseCount { get; set; }
    public List<FormSubmissionDto> Submissions { get; set; } = [];
    public List<ChoiceQuestionSummaryDto> ChoiceSummaries { get; set; } = [];
}
