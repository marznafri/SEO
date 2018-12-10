using System.Collections.Generic;
using System.Threading.Tasks;
using SEO.Core.Model;

namespace SEO.Core.Process.Processing
{
    public interface IAnalysis
    {
        Task<Dictionary<string, int>> GetAllExternalLinks(string searchText);
        Task<Dictionary<string, int>> GetAllExternalLinksForURL(string searchText);
        Task<List<MetaTag>> GetAllMetaTagsInfo(string searchText, bool isPageFilterStopWords);
        Task<List<MetaTag>> GetAllMetaTagsInfoForURL(string searchText, bool isPageFilterStopWords, IList<string> stopWords);
        Task<Dictionary<string, int>> GetAllWordsInfo(string searchText, bool isPageFilterStopWords, IList<string> stopWords);
        Task<Dictionary<string, int>> GetAllWordsInfoForURL(string searchText, bool isPageFilterStopWords, IList<string> stopWords);
    }
}