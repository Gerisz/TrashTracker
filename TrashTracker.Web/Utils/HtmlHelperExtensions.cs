using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Linq.Expressions;
using System.Text.Encodings.Web;

namespace TrashTracker.Web.Utils
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent IconActionLink(
            this IHtmlHelper helper,
            String linkText,
            String iconClass,
            String actionName,
            String controllerName,
            Object routeValues,
            Object htmlAttributes)
        {
            var urlHelperFactory = helper.ViewContext.HttpContext.RequestServices.
                GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(helper.ViewContext);
            var link = IconActionLink(linkText, iconClass, htmlAttributes);
            link.MergeAttribute("href", urlHelper.Action(new UrlActionContext()
            {
                Action = actionName,
                Controller = controllerName,
                Values = routeValues
            }));
            if (htmlAttributes != null)
                foreach (var prop in htmlAttributes.GetType().GetProperties())
                    link.MergeAttribute(prop.Name.Replace("_", "-"),
                        prop.GetValue(htmlAttributes)!.ToString());
            return link;
        }

        public static IHtmlContent IconLabel<TModel>(
            this IHtmlHelper<TModel> helper,
            String labelText,
            String iconClass,
            Object htmlAttributes)
        {
            var icon = "";
            using (var writer = new StringWriter())
            {
                var iconBuilder = new TagBuilder("i");
                iconBuilder.AddCssClass(iconClass);
                iconBuilder.WriteTo(writer, HtmlEncoder.Default);
                icon = writer.ToString();
            }
            var label = new TagBuilder("label");
            label.InnerHtml.AppendHtml($"{icon} {labelText}");
            if (htmlAttributes != null)
                foreach (var prop in htmlAttributes.GetType().GetProperties())
                    label.MergeAttribute(prop.Name.Replace("_", "-"),
                        prop.GetValue(htmlAttributes)!.ToString());
            return label;
        }

        public static IHtmlContent IconLabelFor<TModel, TValue>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            String labelText,
            String iconClass,
            Object htmlAttributes)
        {
            var label = helper.LabelFor(expression, "", htmlAttributes);
            var icon = "";
            using (var writer = new StringWriter())
            {
                var iconBuilder = new TagBuilder("i");
                iconBuilder.AddCssClass(iconClass);
                iconBuilder.WriteTo(writer, HtmlEncoder.Default);
                icon = writer.ToString();
            }
            ((TagBuilder)label).InnerHtml.AppendHtml($"{icon} {labelText}");
            if (htmlAttributes != null)
                foreach (var prop in htmlAttributes.GetType().GetProperties())
                    ((TagBuilder)label).MergeAttribute(prop.Name.Replace("_", "-"),
                        prop.GetValue(htmlAttributes)!.ToString());
            return label;
        }

        private static TagBuilder IconActionLink(
            string linkText,
            string iconClass,
            object htmlAttributes)
        {
            var icon = "";
            using (var writer = new StringWriter())
            {
                var iconBuilder = new TagBuilder("i");
                iconBuilder.AddCssClass(iconClass);
                iconBuilder.WriteTo(writer, HtmlEncoder.Default);
                icon = writer.ToString();
            }
            var span = "";
            using (var writer = new StringWriter())
            {
                var spanBuilder = new TagBuilder("span");
                spanBuilder.InnerHtml.AppendHtml(linkText);
                spanBuilder.WriteTo(writer, HtmlEncoder.Default);
                span = writer.ToString();
            }
            var linkTag = new TagBuilder("a");
            linkTag.InnerHtml.AppendHtml($"{icon} {span}");
            if (htmlAttributes != null)
                foreach (var prop in htmlAttributes.GetType().GetProperties())
                    linkTag.MergeAttribute(prop.Name.Replace("_", "-"),
                        prop.GetValue(htmlAttributes)!.ToString());
            return linkTag;
        }
    }
}
