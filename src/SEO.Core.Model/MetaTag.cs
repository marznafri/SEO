using System;
using System.Collections.Generic;
using System.Text;

namespace SEO.Core.Model
{
    public class MetaTag
    {
        public string Name { get; set; }
        public string Property { get; set; }
        public string ItemProp { get; set; }
        public string HttpEquiv { get; set; }
        public string Content { get; set; }
        public Dictionary<string, int> URLInfoList { get; set; }
        public Dictionary<string, int> WordsInfoList { get; set; }
        public int TotalWordCount { get; set; }
    }
}
