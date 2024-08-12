namespace API.Errors
{
    /// <summary>
    /// Used to shape the error response of server error with the addtion property details if you in the development
    /// </summary>
    public class ApiException : ApiResponse
    {
        public ApiException(int statusCode, string? errorMessage = null , string? details = null) : base(statusCode, errorMessage)
        {
            Details = details;
        }

        public string? Details { get; set; }
    }
}
