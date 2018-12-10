using System;
using System.Collections.Generic;
using System.Text;

namespace SEO.Core.Model
{
    public class ExceptionObject
    {
        public string Source { get; set; }
        public string ErrorMethod { get; set; }
        public string Message { get; set; }
        public ExceptionObject InnerException { get; set; }
    }
}
