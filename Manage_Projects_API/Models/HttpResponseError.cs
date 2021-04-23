using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Models.Https
{
    public class HttpResponseError
    {
        public int StatusCode { get; set; }
        public HttpResponseErrorDetail Detail { get; set; }
    }

    public class HttpResponseErrorDetail
    {
        public string Message { get; set; }
        public string InnerMessage { get; set; }
    }
}
