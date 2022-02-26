using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InfoTrackTest.Application.Implementations.Models;
using InfoTrackTest.Core.Constants;

namespace InfoTrackTest.Application.Crawler
{
    public abstract class Crawler
    {
        private readonly string _keywords;

        protected Crawler(string keywords)
        {
            _keywords = keywords ?? Strings.CEOSearchKeyWords;
        }

        public HttpWebRequest InitRequest(Uri uri = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri ?? new Uri(Strings.CrimeToolWebsite));
            request.Accept = "*/*";
            request.ContentType = "application/x-www.form-urlencoded";
            request.AllowAutoRedirect = false;
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36";
            request.Timeout = 10000;
            request.Method = "GET";

            return request;
        }

        public abstract Task<List<BaseGoogleSearchResult>> Load();

        public void OnError(Exception e)
        {
            throw e;
        }
    }

    public class InfoTrackCrawler : Crawler
    {
        public override async Task<List<BaseGoogleSearchResult>> Load()
        {
            var request = InitRequest();
            List<BaseGoogleSearchResult> allMatched = new List<BaseGoogleSearchResult>();
            var firstResult = await ProcessFirstCites(request);
            allMatched.AddRange(firstResult.Item1);
            var navLinks = Regex.Matches(firstResult.Item2, "<\\s*a aria-label=\"Page[^>]*>(.*?)<\\s*/\\s*a>", RegexOptions.IgnoreCase);

            for (int i = 0; i < navLinks.Count; i++)
            {
                var link = Regex.Match(navLinks[i].Value, @"<a\s+(?:[^>]*?\s+)?href=([""'])(.*?)\1");
                var uri = new Uri("https://www.google.com" + link.Groups[2].Value);
                var requestLoadMore = InitRequest(uri);
                allMatched.AddRange(await ProcessCites(requestLoadMore, i + 1));
            }

            return allMatched.Take(100).ToList();
        }

        public async Task<List<BaseGoogleSearchResult>> ProcessCites(HttpWebRequest request, int pageIndex)
        {
            var results = await GetData(request, pageIndex);
            return results.Item1;
        }

        public async Task<Tuple<List<BaseGoogleSearchResult>, string>> ProcessFirstCites(HttpWebRequest request)
        {
            var results = await GetData(request);

            return results;
        }

        public async Task<Tuple<List<BaseGoogleSearchResult>, string>> GetData(HttpWebRequest request, int pageIndex = 0)
        {
            List<BaseGoogleSearchResult> results = new List<BaseGoogleSearchResult>();
            var response = (HttpWebResponse)await request.GetResponseAsync();
            var stream = response.GetResponseStream();

            //IWebElement lastPageElement = driver.FindElement(By.XPath("//a[@class='btn last']"));

            var reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var source = await reader.ReadToEndAsync();
            MatchCollection matchCollection = Regex.Matches(source, "Graphs", RegexOptions.IgnoreCase);
            for (int i = 0; i < matchCollection.Count; i++)
            {
                results.Add(new InfoTrackSearchResult
                {
                    Index = i + 1 + (pageIndex * 12),
                    Title = matchCollection[i].Groups[1].Value
                });
            }

            return Tuple.Create(results, source);


        }

        public InfoTrackCrawler(string keywords) : base(keywords)
        {
        }
    }
}