using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BestHackerNews.Models
{
    public class NewsItem
    {
        public int Id { get; set; }
        [DisplayName("Author")]
        public string By { get; set; }
        public string  Title { get; set; }
    }
}