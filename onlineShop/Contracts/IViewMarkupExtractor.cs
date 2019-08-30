using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace onlineShop.Contracts
{
    public interface IViewMarkupExtractor
    {
        Task<string> MarkupToString(ControllerContext controllerContext, string viewName, object model, ITempDataDictionary tempData);
    }
}