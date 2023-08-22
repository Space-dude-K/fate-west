using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace wc3_fate_west_web.Extensions
{
    public static class WebExtensionscs
    {
        public static T ParseValue<T>(this System.Data.Common.DbDataReader reader, string column)
        {
            T result = default(T);

            if (!reader.IsDBNull(reader.GetOrdinal(column)))
                result = (T)reader.GetValue(reader.GetOrdinal(column));

            return result;
        }
        public static string GetKDAColor(this double kda)
        {
            if (kda >= 5.0)
            {
                return "kdagold";
            }
            if (kda >= 4.0)
            {
                return "kdablue";
            }
            if (kda >= 3.0)
            {
                return "kdagreen";
            }
            if (kda >= 2.0)
            {
                return "kdanormal";
            }

            return "kdalow";
        }
        public static ViewResult View(this PageModel pageModel, string viewName)
        {
            return new ViewResult()
            {
                ViewName = viewName,
                ViewData = pageModel.ViewData,
                TempData = pageModel.TempData
            };
        }

        public static ViewResult View<TModel>(this PageModel pageModel, string viewName, TModel model)
        {
            var viewDataDictionary = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };
            foreach (var kvp in pageModel.ViewData) viewDataDictionary.Add(kvp);

            return new ViewResult
            {
                ViewName = viewName,
                ViewData = viewDataDictionary,
                TempData = pageModel.TempData
            };
        }
    }
}