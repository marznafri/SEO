using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SEO.Core.Model;

namespace SEO.Core.Process.Processing
{
    public class Processing : IProcessing
    {
        IRedisCache _redisCache;

        public Processing(IRedisCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<Guid> RegisterSearchText(SEOObject seoObj)
        {
            var newID = Guid.NewGuid();
            await _redisCache.Set(newID.ToString(), seoObj, 3600);

            return newID;
        }

        public async Task<bool> IsURLValidAsync(string URL)
        {
            var isValidURL = false;
            try
            {
                var web = new HtmlWeb();
                var docURL = await web.LoadFromWebAsync(URL);
                if (docURL != null)
                {
                    isValidURL = true;
                }
            }
            catch (Exception)
            {
            }

            return isValidURL;
        }

        public async Task<ExceptionObject> HandleError(string identifier, Exception ex)
        {
            var exceptionObject = new ExceptionObject();

            try
            {
                await HandleErrorManagement(identifier, exceptionObject, ex);
            }
            catch (Exception)
            {
            }

            return exceptionObject;
        }

        private async Task HandleErrorManagement(string identifier, ExceptionObject exceptionObject, Exception ex)
        {
            exceptionObject.Source = ex.Source;
            exceptionObject.ErrorMethod = identifier;
            exceptionObject.Message = ex.Message;

            if (ex.InnerException != null)
            {
                await HandleErrorManagement("inner_" + identifier, exceptionObject, ex.InnerException);
            }
        }
    }
}
