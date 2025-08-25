using static ClassLibrary1.SD;
using System.IO.Pipelines;

namespace MagicVilla_Web.Dto
{
    public  class ApiRequest
    {
        public ApiType apiType { set; get; } = ApiType.Get;
        public string url { set; get; }

        public string? token { set; get; }
        public Object model { set; get; }
    }
}
    