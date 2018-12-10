using System;

namespace SEO.Core.Model
{
    public class SEOObject
    {
        public string SearchText { get; set; }
        public bool IsURL { get; set; }
        public bool IsPageFilterStopWords { get; set; }
        public bool IsCountNumberofWords { get; set; }
        public bool IsMetaTagsInfo { get; set; }
        public bool IsGetExternalLink { get; set; }
    }
}
