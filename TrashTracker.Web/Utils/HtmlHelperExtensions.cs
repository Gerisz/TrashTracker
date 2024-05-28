using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Linq.Expressions;
using System.Text.Encodings.Web;

namespace TrashTracker.Web.Utils
{
    /// <summary>
    /// A <see langword="static"/> extension <see langword="class"/>
    /// for classes implementing the <see cref="IHtmlHelper"/> <see langword="interface"/>,
    /// containing several helper functions to create HTML elements to show in views.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Creates an action link for the specified <paramref name="href"/>
        /// with the given <paramref name="iconClass"/> and <paramref name="linkText"/>,
        /// giving it <paramref name="htmlAttributes"/>'s attributes.
        /// </summary>
        /// <param name="linkText">The text of the action link.</param>
        /// <param name="iconClass">
        /// The icon's class to render before the <paramref name="linkText"/>.
        /// </param>
        /// <param name="href">The URL that the action link should point to.</param>
        /// <param name="htmlAttributes">
        /// The html attributes given to the action link's tags (e. g. css styles and/or classes).
        /// </param>
        /// <returns>
        /// A <see cref="TagBuilder"/> containing the HTML code for the action link.
        /// </returns>
        public static IHtmlContent IconActionLink(
            this IHtmlHelper helper,
            String linkText,
            String iconClass,
            String href,
            Object htmlAttributes)
        {
            var link = IconActionLink(linkText, iconClass, htmlAttributes);
            link.MergeAttribute("href", href);
            if (htmlAttributes != null)
                foreach (var prop in htmlAttributes.GetType().GetProperties())
                    link.MergeAttribute(prop.Name.Replace("_", "-"),
                        prop.GetValue(htmlAttributes)!.ToString());
            return link;
        }

        /// <summary>
        /// Creates an action link for the specified
        /// <paramref name="controllerName"/>'s <paramref name="actionName"/> action
        /// with the given <paramref name="iconClass"/> and <paramref name="linkText"/>,
        /// giving it <paramref name="htmlAttributes"/>'s attributes.
        /// </summary>
        /// <param name="linkText">The text of the action link.</param>
        /// <param name="iconClass">
        /// The icon's class to render before the <paramref name="linkText"/>.
        /// </param>
        /// <param name="actionName">
        /// Name of the action that the action link should point to.
        /// </param>
        /// <param name="controllerName">
        /// Name of the controller that the action link should point to.
        /// </param>
        /// <param name="routeValues">
        /// Additional route values, that should be contained in the action link's href.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes given to the action link's tags (e. g. css styles and/or classes).
        /// </param>
        /// <returns>
        /// A <see cref="TagBuilder"/> containing the HTML code for the action link.
        /// </returns>
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

        /// <summary>
        /// Creates a label with the specified
        /// <paramref name="iconClass"/> and <paramref name="labelText"/>,
        /// giving it <paramref name="htmlAttributes"/>'s attributes.
        /// </summary>
        /// <param name="labelText">The text of the label.</param>
        /// <param name="iconClass">
        /// The icon's class to render before the <paramref name="labelText"/>.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes given to the label's tags (e. g. css styles and/or classes).
        /// </param>
        /// <returns>A <see cref="TagBuilder"/> containing the HTML code for the label.</returns>
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

        /// <summary>
        /// Creates a label for the <paramref name="expression"/> with the specified
        /// <paramref name="iconClass"/> and <paramref name="labelText"/>,
        /// giving it <paramref name="htmlAttributes"/>'s attributes.
        /// </summary>
        /// <param name="expression">
        /// An <see cref="Expression"/> containing a <see cref="Func{TModel, TValue}"/>,
        /// selecting the value to create the label for.
        /// </param>
        /// <param name="labelText">The text of the label.</param>
        /// <param name="iconClass">
        /// The icon's class to render before the <paramref name="labelText"/>.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes given to the label's tags (e. g. css styles and/or classes).
        /// </param>
        /// <param name="breakLine">
        /// Optional logical value,
        /// if the icon and text should be in separate lines (defaults to <see langword="false"/>).
        /// </param>
        /// <returns>A <see cref="TagBuilder"/> containing the HTML code for the label.</returns>
        public static IHtmlContent IconLabelFor<TModel, TValue>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            String labelText,
            String iconClass,
            Object htmlAttributes,
            Boolean breakLine = false)
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
            ((TagBuilder)label).InnerHtml.AppendHtml($"{icon}{(breakLine ? "<br>" : "")} {labelText}");
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
