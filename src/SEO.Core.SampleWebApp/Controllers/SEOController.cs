using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SEO.Core.Model;
using SEO.Core.Process.Processing;

namespace SEO.Core.SampleWebApp.Controllers
{
    public class SEOController : Controller
    {
        IAnalysis _analysis;
        IProcessing _processing;
        IRedisCache _redisCache;

        public SEOController(IAnalysis analysis, IProcessing processing, IRedisCache redisCache)
        {
            _analysis = analysis;
            _processing = processing;
            _redisCache = redisCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterSearchText([FromBody] SEOObject seoObj)
        {
            Guid ID = Guid.Empty;
            try
            {
                if (seoObj.IsURL)
                {
                    await ValidateURL(seoObj.SearchText);
                }

                ID = await _processing.RegisterSearchText(seoObj);
            }
            catch (Exception ex)
            {
                var exceptionItem = await _processing.HandleError("RegisterSearchText", ex);

                return NotFound(exceptionItem);
            }

            return Ok(ID);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWordsInfo(string searchID, IList<string> stopWords)
        {

            Dictionary<string, int> wordsDictionary = null;

            try
            {
                var analyserObj = await ValidateSearchID(searchID);
                await ValidateFilter(analyserObj, "GetAllWordsInfo");

                if (analyserObj != null)
                {
                    wordsDictionary = await _analysis.GetAllWordsInfo(analyserObj.SearchText, analyserObj.IsPageFilterStopWords, stopWords);
                }

            }
            catch (Exception ex)
            {
                var exceptionItem = await _processing.HandleError("GetAllWordsInfo", ex);

                return NotFound(exceptionItem);
            }            

            return Ok(wordsDictionary.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMetaTagsInfo(string searchID)
        {
            var listOfMetaTagInfo = new List<MetaTag>();
            try
            {
                var analyserObj = await ValidateSearchID(searchID);
                await ValidateFilter(analyserObj, "GetAllMetaTagsInfo");

                if (analyserObj != null)
                {
                    listOfMetaTagInfo = await _analysis.GetAllMetaTagsInfo(analyserObj.SearchText, analyserObj.IsPageFilterStopWords);
                }
            }
            catch (Exception ex)
            {
                var exceptionItem = await _processing.HandleError("GetAllMetaTagsInfo", ex);

                return NotFound(exceptionItem);
            }

            return Ok(listOfMetaTagInfo);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExternalLinks(string searchID)
        {
            Dictionary<string, int> wordsDictionary = null;
            try
            {
                var analyserObj = await ValidateSearchID(searchID);
                await ValidateFilter(analyserObj, "GetAllExternalLinks");

                if (analyserObj != null)
                {
                    wordsDictionary = await _analysis.GetAllExternalLinks(analyserObj.SearchText);
                }
            }
            catch (Exception ex)
            {
                var exceptionItem = await _processing.HandleError("GetAllExternalLinks", ex);

                return NotFound(exceptionItem);
            }

            return Ok(wordsDictionary.ToList());
        }

        private async Task<SEOObject> ValidateSearchID(string searchID)
        {
            var analyserObj = await _redisCache.Get<SEOObject>(searchID);
            if (analyserObj == null || string.IsNullOrWhiteSpace(analyserObj.SearchText))
            {
                throw new Exception("SearchID not found");
            }

            return analyserObj;
        }

        private async Task<bool> ValidateURL(string searchText)
        {
            var isURLValid = await _processing.IsURLValidAsync(searchText);

            if (!isURLValid)
            {
                throw new Exception("URL given is not Valid");
            }

            return true;
        }

        private async Task ValidateFilter(SEOObject seoObj, string page)
        {
            await Task.Run(() =>
            {
                if (!seoObj.IsCountNumberofWords && page == "GetAllWordsInfo")
                {
                    throw new Exception("You never choose this filter [Get all words]");
                }

                if (!seoObj.IsMetaTagsInfo && page == "GetAllMetaTagsInfo")
                {
                    throw new Exception("You never choose this filter [Get all meta tags]");
                }

                if (!seoObj.IsGetExternalLink && page == "GetAllExternalLinks")
                {
                    throw new Exception("You never choose this filter [Get all external links]");
                }
            });
        }
    }
}