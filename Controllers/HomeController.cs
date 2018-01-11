using System;
using System.Collections.Generic;
using System.Web.Caching;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using BestHackerNews.Models;


namespace BestHackerNews.Controllers
{
    public class HomeController : Controller
    {

        string Baseurl = "https://hacker-news.firebaseio.com/v0/";

        public ActionResult Index(string filterString)
        {
            ViewBag.Title = "No Cache - Data read on each refresh";
            ViewBag.filterString = filterString;
            IEnumerable<NewsItem> newsItems = GetNewsItems();
            if(!String.IsNullOrEmpty(filterString))
            {
                newsItems = newsItems.Where(i => i.Title.Contains(filterString)).ToList();
            }
            return View(newsItems);
        }

        [OutputCache(Duration = 3600, VaryByParam = "filterString")]
        public ActionResult ActionCache(string filterString)
        {
            ViewBag.Title = "Action Cache - Data read on each filter";
            ViewBag.filterString = filterString;
            IEnumerable<NewsItem> newsItems = GetNewsItems();
            if (!String.IsNullOrEmpty(filterString))
            {
                newsItems = newsItems.Where(i => i.Title.Contains(filterString)).ToList();
            }
            return View("Index", newsItems);
        }

        public ActionResult DataCache(string filterString)
        {
            ViewBag.Title = "Data Cache - Data read once";
            ViewBag.filterString = filterString;
            IEnumerable<NewsItem> newsItems = GetNewsItemsCached();
            if (!String.IsNullOrEmpty(filterString))
            {
                newsItems = newsItems.Where(i => i.Title.Contains(filterString)).ToList();
            }
            return View("Index", newsItems);
        }


        private List<NewsItem> GetNewsItemsCached()
        {
            if(System.Web.HttpContext.Current.Cache["NewsItems"] == null)
            {
                System.Web.HttpContext.Current.Cache["NewsItems"] = GetNewsItems();
            }
            List<NewsItem> newsItems = System.Web.HttpContext.Current.Cache["NewsItems"] as List<NewsItem>;
            return newsItems;
        }

        private List<NewsItem> GetNewsItems()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(Baseurl + "topstories.json").GetAwaiter().GetResult();
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var idList = JsonConvert.DeserializeObject<List<int>>(result);
                List<NewsItem> lni = new List<NewsItem>();
                foreach (int id in idList)
                {
                    var uri = Baseurl + string.Format("item/{0}.json", id);
                    var detailResponse = httpClient.GetAsync(uri).GetAwaiter().GetResult();
                    var detailResult = detailResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    NewsItem ni = JsonConvert.DeserializeObject<NewsItem>(detailResult);
                    lni.Add(ni);
                }
                return lni;
            }
        }
    }
}