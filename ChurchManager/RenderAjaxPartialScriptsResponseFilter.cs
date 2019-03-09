using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace ChurchManager
{
    public class RenderAjaxPartialScriptsResponseFilter : MemoryStream
    {
        private readonly Stream _response;
        private readonly ActionExecutingContext _filterContext;

        public RenderAjaxPartialScriptsResponseFilter(Stream response, ActionExecutingContext filterContext)
        {
            _response = response;
            _filterContext = filterContext;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _response.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            var scriptsHtml = GetScripts();
            var buffer = Encoding.UTF8.GetBytes(scriptsHtml);
            _response.Write(buffer, 0, buffer.Length);
            base.Flush();
        }

        private string GetScripts()
        {
            string html = "";
            var itemsToRemove = new List<object>();
            foreach (object key in _filterContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    var template = _filterContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    if (template != null)
                    {
                        html += (template(null));
                        itemsToRemove.Add(key);
                    }
                }
            }
            foreach (var key in itemsToRemove)
                _filterContext.HttpContext.Items.Remove(key);

            return html;
        }
    }

    /// <summary>
    /// Appends partial view scripts to the html response of an AJAX request
    /// </summary>
    public class RenderAjaxPartialScriptsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var response = filterContext.HttpContext.Response;
                if (response.Filter != null)
                    response.Filter = new RenderAjaxPartialScriptsResponseFilter(response.Filter, filterContext);
            }
        }
    }
}