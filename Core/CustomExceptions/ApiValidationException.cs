namespace API.Errors
{
    public class ApiValidationException : ApiResponse
    {
        public ApiValidationException() : base(400)
        {
        }

        public List<string> Errors { get; set; }
    }
}
