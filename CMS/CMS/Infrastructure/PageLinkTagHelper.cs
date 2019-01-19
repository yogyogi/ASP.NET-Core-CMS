using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using System.Dynamic;

namespace CMS.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int? Status { get; set; }

        public string SearchText { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        /*Accepts all attributes that are page-other-* like page-other-category="@Model.allTotal" page-other-some="@Model.allTotal"*/
        [HtmlAttributeName(DictionaryAttributePrefix = "page-other-")]
        public Dictionary<string, object> PageOtherValues { get; set; } = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;

        public string PageClass { get; set; }

        public string PageClassNormal { get; set; }

        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            string anchorInnerHtml = "";

            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                anchorInnerHtml = AnchorInnerHtml(i, PageModel);

                if (anchorInnerHtml == "..")
                    tag.Attributes["href"] = "#";
                else if (PageOtherValues.Keys.Count != 0)
                    tag.Attributes["href"] = urlHelper.Action(PageAction, AddDictionaryToQueryString(i));
                else
                    tag.Attributes["href"] = urlHelper.Action(PageAction, new { id = i, status = Status, searchText = SearchText });

                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : "");
                }
                tag.InnerHtml.Append(anchorInnerHtml);
                if (anchorInnerHtml != "")
                    result.InnerHtml.AppendHtml(tag);
                /*PageUrlValues["productPage"] = i;
                tag.Attributes["href"] = urlHelper.Action(PageAction,new { id = i });
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);*/
            }
            output.Content.AppendHtml(result.InnerHtml);
        }

        public IDictionary<string, object> AddDictionaryToQueryString(int i)
        {
            //object link = new { id = i, status = Status, searchText = SearchText };

            //string link = "";
            //foreach (string key in PageOtherValues.Keys)
            //    link = link + key + "=" + PageOtherValues[key] + ",";
            //link = link.Substring(0, link.Length - 1);
            //return link;

            object routeValues = null;
            var dict = (routeValues != null) ? new RouteValueDictionary(routeValues) : new RouteValueDictionary();
            dict.Add("id", i);
            dict.Add("status", Status);
            dict.Add("searchText", SearchText);
            foreach (string key in PageOtherValues.Keys)
            {
                dict.Add(key, PageOtherValues[key]);
            }

            var expandoObject = new ExpandoObject();
            var expandoDictionary = (IDictionary<string, object>)expandoObject;
            foreach (var keyValuePair in dict)
            {
                expandoDictionary.Add(keyValuePair);
            }

            return expandoDictionary;
        }

        public static string AnchorInnerHtml(int i, PagingInfo pagingInfo)
        {
            string anchorInnerHtml = "";
            if (pagingInfo.TotalPages <= 10)
                anchorInnerHtml = i.ToString();
            else
            {
                if (pagingInfo.CurrentPage <= 5)
                {
                    if ((i <= 8) || (i == pagingInfo.TotalPages))
                        anchorInnerHtml = i.ToString();
                    else if (i == pagingInfo.TotalPages - 1)
                        anchorInnerHtml = "..";
                }
                else if ((pagingInfo.CurrentPage > 5) && (pagingInfo.TotalPages - pagingInfo.CurrentPage >= 5))
                {
                    if ((i == 1) || (i == pagingInfo.TotalPages) || ((pagingInfo.CurrentPage - i >= -3) && (pagingInfo.CurrentPage - i <= 3)))
                        anchorInnerHtml = i.ToString();
                    else if ((i == pagingInfo.CurrentPage - 4) || (i == pagingInfo.CurrentPage + 4))
                        anchorInnerHtml = "..";
                }
                else if (pagingInfo.TotalPages - pagingInfo.CurrentPage < 5)
                {
                    if ((i == 1) || (pagingInfo.TotalPages - i <= 7))
                        anchorInnerHtml = i.ToString();
                    else if (pagingInfo.TotalPages - i == 8)
                        anchorInnerHtml = "..";
                }
            }
            return anchorInnerHtml;
        }
    }
}
