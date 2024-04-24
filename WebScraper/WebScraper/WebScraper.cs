using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Collections.Concurrent;
namespace WebScraper
{

    struct WebData 
    {
        public string m_Html;
        public string m_Url;
    }
    class WebScraper
    {
        string m_RootPath = "";
        string m_RootUrl = "";
        HttpClient m_Client = new HttpClient();
        ConcurrentQueue<string> m_ParsedUrls = new ConcurrentQueue<string>();
        bool m_CanFinish = false;


        public void Init(string rootDirectory, string rootUrl)
        {
            m_RootPath = rootDirectory;
            m_RootUrl = rootUrl;
        }

        public void Run() 
        {
            FetchNextHtml(m_RootUrl);

            m_CanFinish = true;
        }

        async public Task FetchNextHtml(string url) 
        {
            if (m_ParsedUrls.Contains(url))
            {
                return;
            }
            String html = m_Client.GetStringAsync(url).Result;

            WebData data = new WebData();

            data.m_Html = html;
            data.m_Url = url;

            m_ParsedUrls.Enqueue(url);

            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(html);
            Scrape(data);
            var links = document.DocumentNode.SelectNodes("//@href");

            foreach (var link in links)
            {
                string linkUrl = GetAbsolutePath("href", url, link);

                if (linkUrl.Contains(".html"))
                {
                   await FetchNextHtml(linkUrl);
                }
            }
        }

        async public Task Scrape(WebData webData)
        {
            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(webData.m_Html);

            var links = document.DocumentNode.SelectNodes("//@href");

            ConcurrentBag<HtmlNode> concurrentLinksList = new ConcurrentBag<HtmlNode>(links);

            Parallel.ForEachAsync(concurrentLinksList, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (currentNode, _) =>
            {
                string linkUrl = GetAbsolutePath("href", webData.m_Url, currentNode);
                string relativePath = GetRelativePath(linkUrl);

                if (!File.Exists(m_RootPath + "/" + relativePath))
                {
                    await ParseLink(relativePath);
                }
            });
            var images = document.DocumentNode.SelectNodes("//img/@src");
            
            ConcurrentBag<HtmlNode> concurrentImagesList = new ConcurrentBag<HtmlNode>(images);
            
            Parallel.ForEachAsync(concurrentImagesList, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (currentNode, _) =>
            {
                string imageUrl = GetAbsolutePath("src", webData.m_Url, currentNode);
            
                var res = await m_Client.GetAsync(imageUrl);
            
                string savePath = m_RootPath + "/" + GetRelativePath(imageUrl);
            
                byte[] imageBytes = await res.Content.ReadAsByteArrayAsync();
            
                string saveDirectory = savePath.Substring(0, savePath.LastIndexOf("/"));
            
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }
                if (!File.Exists(savePath))
                {
                    Console.WriteLine("Scraping image: " + imageUrl);
                    File.WriteAllBytes(savePath, imageBytes);
                }
            });
        }

        async public Task ParseLink(String url)
        {
            String path = m_RootPath + "/" + url;
            string outputPath = path;
            path = path.Substring(0, path.LastIndexOf("/"));

            String rootUrl = m_RootUrl + url;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            String html = m_Client.GetStringAsync(rootUrl).Result;

            if (!File.Exists(outputPath))
            {
                using (StreamWriter outputFile = new StreamWriter(outputPath))
                {
                    Console.WriteLine("Scraping file: " + rootUrl);
                    await outputFile.WriteAsync(html);
                }
            }
        }

        private string GetAbsolutePath(string attribute, string url, HtmlNode htmlNode) 
        {
            string absolutePath = htmlNode.Attributes[attribute].Value;

            if (!Uri.IsWellFormedUriString(absolutePath, UriKind.Absolute))
            {
                Uri baseUri = new Uri(url);
                Uri absoluteUri = new Uri(baseUri, absolutePath);
                absolutePath = absoluteUri.ToString();
            }

            return absolutePath;
        }

        private string GetRelativePath(string url)
        {
            string[] splitUrl = url.Split(m_RootUrl);

            if (splitUrl.Length > 1)
            {
                return splitUrl[1];
            }

            return url;
        }
    }
}
