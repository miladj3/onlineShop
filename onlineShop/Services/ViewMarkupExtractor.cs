using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using onlineShop.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace onlineShop.Services
{
    public class ViewMarkupExtractor : IViewMarkupExtractor
    {
        private readonly IRazorViewEngine _razorViewEngine;

        public ViewMarkupExtractor(IRazorViewEngine razorViewEngine)
        {
            _razorViewEngine = razorViewEngine;
        }

        public async Task<string> MarkupToString(
            ControllerContext controllerContext, 
            string viewName, 
            object model, 
            ITempDataDictionary tempData)
        {
            using (var stringWriter = new StringWriter())
            {
                // get required view
                ViewEngineResult viewResult = _razorViewEngine.FindView(controllerContext, viewName, false);

                if (!viewResult.Success)
                    return null;

                // build ViewDataDictionary - model expected by the view
                var viewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                    { Model = model };

                // get markup and write it to string using stringWriter
                ViewContext viewContext = new ViewContext(
                    controllerContext,
                    viewResult.View,
                    viewData,
                    tempData,
                    stringWriter,
                    new HtmlHelperOptions());

                await viewResult.View.RenderAsync(viewContext);

                return stringWriter.ToString();
            }
        }
    }
}
