using System;
using System.Collections.Generic;
using System.Text;

namespace SEO.Core.Model
{
    public class RedisCacheSettings
    {
        public string Host { get; set; }
        public string Category { get; set; }
        public int Timeout { get; set; }

        public string RedisFormat = "{0}_{1}";
    }
}
