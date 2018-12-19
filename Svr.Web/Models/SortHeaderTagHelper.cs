﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models
{
    public class SortHeaderTagHelper : TagHelper
    {
        //public string CurrentFilterName { get; set; }// фильтрация
        public string SearchString { get; set; }// фильтрация
        public string CurrentFilterOwner { get; set; }

        public SortState Property { get; set; } // значение текущего свойства, для которого создается тег
        public SortState Current { get; set; }  // значение активного свойства, выбранного для сортировки
        public string Action { get; set; }  // действие контроллера, на которое создается ссылка
        public bool Up { get; set; }    // сортировка по возрастанию или убыванию

        private IUrlHelperFactory urlHelperFactory;
        #region конструктор
        public SortHeaderTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        #endregion
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "a";
            string url = urlHelper.Action(Action, new { sortOrder = Property, searchString = SearchString, owner = CurrentFilterOwner, });
            output.Attributes.SetAttribute("href", url);
            // если текущее свойство имеет значение CurrentSort
            if (((Current == SortState.CodeAsc) && (Property == SortState.CodeDesc)) || ((Current == SortState.CodeDesc) && (Property == SortState.CodeAsc)) ||
                ((Current == SortState.NameAsc) && (Property == SortState.NameDesc)) || ((Current == SortState.NameDesc) && (Property == SortState.NameAsc)) || 
                ((Current == SortState.DescriptionAsc) && (Property == SortState.DescriptionDesc)) || ((Current == SortState.DescriptionDesc) && (Property == SortState.DescriptionAsc)) ||
                ((Current == SortState.CreatedOnUtcAsc) && (Property == SortState.CreatedOnUtcDesc)) || ((Current == SortState.CreatedOnUtcDesc) && (Property == SortState.CreatedOnUtcAsc)) ||
                ((Current == SortState.UpdatedOnUtcAsc) && (Property == SortState.UpdatedOnUtcDesc)) || ((Current == SortState.UpdatedOnUtcDesc) && (Property == SortState.UpdatedOnUtcAsc)) ||
                ((Current == SortState.OwnerAsc) && (Property == SortState.OwnerDesc)) || ((Current == SortState.OwnerDesc) && (Property == SortState.OwnerAsc))
                )
            {
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("glyphicon");

                if (Up == true)   // если сортировка по возрастанию
                    tag.AddCssClass("glyphicon-chevron-up");
                else   // если сортировка по убыванию
                    tag.AddCssClass("glyphicon-chevron-down");

                output.PreContent.AppendHtml(tag);
            }
        }
    }
}