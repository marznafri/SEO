using System;
using System.Threading.Tasks;
using SEO.Core.Model;

namespace SEO.Core.Process.Processing
{
    public interface IProcessing
    {
        Task<ExceptionObject> HandleError(string identifier, Exception ex);
        Task<bool> IsURLValidAsync(string URL);
        Task<Guid> RegisterSearchText(SEOObject seoObj);
    }
}