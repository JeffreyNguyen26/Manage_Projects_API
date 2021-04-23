using Manage_Projects_API.Models.Https;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Models
{
    public class RequestException : Exception
    {
        public RequestException(string message = null) : base(message) { }
        public HttpResponseError Error { get; set; }
    }
}
