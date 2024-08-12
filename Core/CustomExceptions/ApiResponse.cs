namespace API.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string? errorMessage = null)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage ?? GetDefaultMessageFromStatusCode(statusCode);
        }

        private string? GetDefaultMessageFromStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "You Made A bad Request",
                401 => "UnAuthorized , you are not",
                404 => "Resource Not Found",
                500 => "Internal server Error",

                _ => null
            };
        }

        public int StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }


}
