using ClassLibrary1;
using static ClassLibrary1.SD;


namespace MagicVilla_Web.Dto
{
    public  class ApiRequest
    {
        public ApiType apiType { set; get; } = ApiType.Get;
        public string url { set; get; }

        public string? token { set; get; }
        public Object model { set; get; }

        public ContentType contentType { set; get; } = ContentType.Json;
    }
}
    