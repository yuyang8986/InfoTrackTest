using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using InfoTrackTest.Core.Constants;

namespace InfoTrackTest.Core.Crawler
{
    public abstract class Crawler
    {
        public HttpWebRequest HttpWebRequest { get; set; }

        public void Init()
        {
            HttpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(Strings.CEOSearchKeyWordsUrl));
            HttpWebRequest.Accept = "*/*";
            HttpWebRequest.ContentType = "application/x-www.form-urlencoded";
            HttpWebRequest.AllowAutoRedirect = false;
            HttpWebRequest.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36";
            HttpWebRequest.Timeout = 10000;
            HttpWebRequest.Method = "GET";
        }

        public abstract void Load(Uri uri);

        public abstract void Complete(string pageSource);

        public void OnError(Exception e)
        {
            throw e;
        }
    }

    public class InfoTrackCrawler : Crawler
    {
        public override async void Load(Uri uri)
        {
            List<string> allMatched = new List<string>();
            var response = (HttpWebResponse)HttpWebRequest.GetResponse();
            var stream = response.GetResponseStream();

            var reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var source = reader.ReadToEnd();
            MatchCollection citesFirst = Regex.Matches(source, "<\\s*cite[^>]*>(.*?)<\\s*/\\s*cite>", RegexOptions.IgnoreCase);
            allMatched.AddRange(ProcessCites(citesFirst));
            var navLinks = Regex.Matches(source, "<\\s*a aria-label=\"Page[^>]*>(.*?)<\\s*/\\s*a>", RegexOptions.IgnoreCase);

            for (int i = 0; i < UPPER; i++)
            {
            }
            reader.Close();
            stream.Close();
            HttpWebRequest.Abort();
            response.Close();
            Complete(source);
        }

        public List<BaseGoogleSearchResult> ProcessCites(MatchCollection matchCollection)
        {
            List<BaseGoogleSearchResult> values = new List<BaseGoogleSearchResult>();
            foreach (Match match in matchCollection)
            {
                values.Add(match.Value);
            }

            return values;
        }

        public override void Complete(string pageSource)
        {
        }
    }
}