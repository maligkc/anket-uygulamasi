using System.Net;

namespace SurveyPlatform.Application.Common;

public class AppException : Exception
{
    public AppException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = (int)statusCode;
    }

    public int StatusCode { get; }
}
