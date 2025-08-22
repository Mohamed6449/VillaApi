using System.Net;

namespace MagicVilla_Web.Dto.ApiResponses
{
    public class ApiResponse
    {
        public HttpStatusCode statusCode {  get; set; }

        public bool Success { get; set; } = true;

        public object? result { get; set; }

        public List<string> Errors { get; set; }= new List<string>();


    }
}