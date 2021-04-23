using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Models
{
    public class ErrorVM
    {
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public string Side { get; set; }
        public string Where { get; set; }
        public string ErrorMessage { get; set; }
        public string InnerErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }

    public class ServerExceptionVM
    {
        public string Message { get; set; }
        public int TraceId { get; set; }
        public string Side { get; set; }
    }
}
