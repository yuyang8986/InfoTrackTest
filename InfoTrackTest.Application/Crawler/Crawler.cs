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
            var request = (HttpWebRequest)WebRequest.Create(uri ?? new Uri(Strings.GoogleSearchUrl + _keywords));
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
            var firstResult = ProcessFirstCites(request);
            allMatched.AddRange(firstResult.Item1);
            var navLinks = Regex.Matches(firstResult.Item2, "<\\s*a aria-label=\"Page[^>]*>(.*?)<\\s*/\\s*a>", RegexOptions.IgnoreCase);

            foreach (Match navLink in navLinks)
            {
                var link = Regex.Match(navLink.Value, @"<a\s+(?:[^>]*?\s+)?href=([""'])(.*?)\1");
                var uri = new Uri("https://www.google.com" + link.Groups[2].Value);
                var requestLoadMore = InitRequest(uri);
                allMatched.AddRange(await ProcessCites(requestLoadMore));
            }

            return allMatched.Take(100).ToList();
        }

        public async Task<List<BaseGoogleSearchResult>> ProcessCites(HttpWebRequest request)
        {
            List<BaseGoogleSearchResult> results = new List<BaseGoogleSearchResult>();
            var response = (HttpWebResponse)await request.GetResponseAsync();
            var stream = response.GetResponseStream();

            var reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var source = await reader.ReadToEndAsync();
            MatchCollection matchCollection = Regex.Matches(source, "<\\s*cite[^>]*>(.*?)<\\s*/\\s*cite>", RegexOptions.IgnoreCase);
            foreach (Match match in matchCollection)
            {
                results.Add(new InfoTrackSearchResult
                {
                    Index = match.Index,
                    Title = match.Groups[1].Value
                });
            }

            return results;
        }

        public Tuple<List<BaseGoogleSearchResult>, string> ProcessFirstCites(HttpWebRequest request)
        {
            List<BaseGoogleSearchResult> results = new List<BaseGoogleSearchResult>();
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();

            var reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var source = reader.ReadToEnd();
            MatchCollection matchCollection = Regex.Matches(source, "<\\s*cite[^>]*>(.*?)<\\s*/\\s*cite>", RegexOptions.IgnoreCase);
            foreach (Match match in matchCollection)
            {
                results.Add(new InfoTrackSearchResult
                {
                    Index = match.Index,
                    Title = match.Value
                });
            }

            return Tuple.Create(results, source);
        }

        public InfoTrackCrawler(string keywords) : base(keywords)
        {
        }
    }
}