﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web;
using SuxrobGM_Website.Core.Entities.Base;

namespace SuxrobGM_Website.Core.Entities.BlogEntities
{
    public class Blog : ArticleBase
    {
        [StringLength(64)]
        [Display(Name = "Cover Photo Path")]
        public string CoverPhotoPath { get; set; }

        [Required(ErrorMessage = "Please enter the summary of article")]
        [StringLength(200, ErrorMessage = "Characters must be less than 200")]
        [Display(Name = "Summary")]
        public string Summary { get; set; }

        [Display(Name = "View Count")]
        public int ViewCount { get; set; }

        public virtual ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public static string GetShortContent(string articleContent, int length)
        {
            var content = HttpUtility.HtmlDecode(articleContent);
            content = Regex.Replace(content, @"<(.|\n)*?>", "");            
            content = content.Substring(0, length).Trim() + "...";
            return content;
        }
    }
}