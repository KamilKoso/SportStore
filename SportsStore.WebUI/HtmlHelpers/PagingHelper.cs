﻿using System;
using System.Text;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.HtmlHelpers
{
    public static class PagingHelper
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo info, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
                for(int i=1 ; i<= info.TotalPages ; i++)
                {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                    if (i == info.CurrentPage)
                    {
                    tag.AddCssClass("selected");
                    tag.AddCssClass("btn-primary");
                    }
                tag.AddCssClass("btn btn-default");
                result.Append(tag.ToString());
                }
            return MvcHtmlString.Create(result.ToString());
        }
    }
}