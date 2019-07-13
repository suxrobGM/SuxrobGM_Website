﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SuxrobGM.Sdk.Pagination;
using SuxrobGM_Website.Data;
using SuxrobGM_Website.Models;

namespace SuxrobGM_Website.Pages.Blog
{
    public class BlogListModel : PageModel
    {
        private ApplicationDbContext _context;

        public BlogListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaginatedList<Article> Articles { get; set; }
        public List<Article> PopularArticles { get; set; }
        public int PageIndex { get; set; }

        public async Task OnGetAsync(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            Articles = await PaginatedList<Article>.CreateAsync(_context.Articles.OrderByDescending(i => i.CreatedTime), pageIndex, 5);
            PopularArticles = _context.Articles.OrderByDescending(i => i.ViewCount).Take(5).ToList();
        }

        public string GetShortContent(string articleContent)
        {
            articleContent = articleContent.Replace('\'', '\"').Replace("\r\n", " ");
            var re = new Regex("(src|srcset|href)=\".+?\"");
            var matchedSrc = re.Matches(articleContent).ToArray();

            foreach (var match in matchedSrc)
            {
                articleContent = articleContent.Replace(match.Value, "");
            }

            return articleContent;
        }
    }
}