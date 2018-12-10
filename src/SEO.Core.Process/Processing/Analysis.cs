using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using HtmlAgilityPack;
using NUglify;
using SEO.Core.Model;
using System.Text.RegularExpressions;

namespace SEO.Core.Process.Processing
{
    public class Analysis : IAnalysis
    {
        //IDataProvider _dataProvider;
        //IHostingEnvironment _hostingEnvironment;

        public Analysis(IHostingEnvironment environment)
        {
            //_dataProvider = dataProvider;
            //_hostingEnvironment = environment;
        }
        public async Task<Dictionary<string, int>> GetAllWordsInfo(string searchText, bool isPageFilterStopWords, IList<string> stopWords)
        {
            var listOfWords = new List<string>();

            listOfWords = await Utility.GetAllWords(searchText);

            if (isPageFilterStopWords)
            {
                listOfWords = await Utility.FilterStopWords(listOfWords, stopWords);
            }
            return await Utility.GroupListOfString(listOfWords);
        }

        public async Task<List<MetaTag>> GetAllMetaTagsInfo(string searchText, bool isPageFilterStopWords)
        {

            return new List<MetaTag>();
        }
        public async Task<Dictionary<string, int>> GetAllExternalLinks(string searchText)
        {
            var listofURL = new List<string>();
            listofURL = await Utility.GetAllExternalLinksFromText(searchText);
            return await Utility.GroupListOfString(listofURL);
        }

        public async Task<Dictionary<string, int>> GetAllWordsInfoForURL(string searchText, bool isPageFilterStopWords, IList<string> stopWords)
        {
            var listOfWords = new List<string>();

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(searchText);
            var root = doc.DocumentNode.SelectSingleNode("//body");
            //var allText = Uglify.HtmlToText(root.InnerHtml);
            var allText = Uglify.HtmlToText(root.OuterHtml);
            listOfWords = await Utility.GetAllWords(allText.Code);

            if (isPageFilterStopWords)
            {
                listOfWords = await Utility.FilterStopWords(listOfWords, stopWords);
            }

            return await Utility.GroupListOfString(listOfWords);
        }

        public async Task<List<MetaTag>> GetAllMetaTagsInfoForURL(string searchText, bool isPageFilterStopWords, IList<string> stopWords)
        {

            var listOfWords = new List<string>();

            var webGet = new HtmlWeb();
            var document = await webGet.LoadFromWebAsync(searchText);
            var metaTags = document.DocumentNode.SelectNodes("//meta");
            var listofMetaTagInfo = new List<MetaTag>();

            foreach (var tag in metaTags.ToList())
            {
                var metaTagInfo = new MetaTag();

                List<string> listofURL = new List<string>();
                List<string> listofWords = new List<string>();

                string content = tag.Attributes["content"] != null ? tag.Attributes["content"].Value : "";
                string property = tag.Attributes["property"] != null ? tag.Attributes["property"].Value : "";
                string name = tag.Attributes["name"] != null ? tag.Attributes["name"].Value : "";
                string itemProp = tag.Attributes["itemprop"] != null ? tag.Attributes["itemprop"].Value : "";
                string httpEquiv = tag.Attributes["http-equiv"] != null ? tag.Attributes["http-equiv"].Value : "";

                metaTagInfo.Content = content;
                metaTagInfo.Property = property;
                metaTagInfo.Name = name;
                metaTagInfo.ItemProp = itemProp;
                metaTagInfo.HttpEquiv = httpEquiv;

                var hrefList = Regex.Replace(metaTagInfo.Content, FilterFormat.GetAllLinks, "$1");

                if (hrefList.ToString().ToUpper().Contains("HTTP") || hrefList.ToString().ToUpper().Contains("://"))
                {
                    //isURL
                    listofURL.Add(hrefList);
                }
                else
                {
                    //isWords
                    var words = await Task.Run(() => { return Utility.SplitSentenceIntoWords(hrefList.ToLower(), 1); });
                    listofWords.AddRange(words);
                }

                if (isPageFilterStopWords)
                {
                    listOfWords = await Utility.FilterStopWords(listOfWords, stopWords);
                }

                metaTagInfo.TotalWordCount = listofWords.Count();
                metaTagInfo.URLInfoList = await Utility.GroupListOfString(listofURL);
                metaTagInfo.WordsInfoList = await Utility.GroupListOfString(listofWords);

                if (!string.IsNullOrWhiteSpace(metaTagInfo.Content))
                {
                    listofMetaTagInfo.Add(metaTagInfo);
                }
            }

            return listofMetaTagInfo;
        }
        public async Task<Dictionary<string, int>> GetAllExternalLinksForURL(string searchText)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(searchText);

            var listofURL = new List<String>();
            var nodeSingle = doc.DocumentNode.SelectSingleNode("//html");
            listofURL = await Utility.GetAllExternalLinksFromText(nodeSingle.OuterHtml);

            return await Utility.GroupListOfString(listofURL);
        }
    }
}
