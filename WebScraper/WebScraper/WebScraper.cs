using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
namespace WebScraper
{
    class WebScraper
    {
        string m_RootPath = "";
        string m_RootUrl = "";
        HttpClient m_Client = new HttpClient();

        public void Init(string rootDirectory, string rootUrl)
        {
            m_RootPath = rootDirectory;
            m_RootUrl = rootUrl;
        }

        async public Task Scrape(String url)
        {
            Console.WriteLine(url);
            String html = m_Client.GetStringAsync(url).Result;

            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(html);

            var links = document.DocumentNode.SelectNodes("//@href");
            var images = document.DocumentNode.SelectNodes("//img/@src");

            if (images != null)
            {
                foreach(HtmlNode image in images) 
                {
                    string imageUrl = GetAbsolutePath("src",url,image);

                    var res = await m_Client.GetAsync(imageUrl);

                    string savePath = m_RootPath + "/" + GetRelativePath(imageUrl);

                    byte[] imageBytes = await res.Content.ReadAsByteArrayAsync();

                    string saveDirectory = savePath.Substring(0, savePath.LastIndexOf("/"));

                    if (!Directory.Exists(saveDirectory))
                    {
                        Directory.CreateDirectory(saveDirectory);
                    }

                    File.WriteAllBytes(savePath, imageBytes);
                }
            }

            if (links != null)
            {
                foreach (HtmlNode link in links)
                {
                    string linkUrl = GetAbsolutePath("href",url,link);

                    string relativePath = GetRelativePath(linkUrl);

                    if (!File.Exists(m_RootPath + "/" + relativePath))
                    {
                        await ParseLink(relativePath);
                        if (linkUrl.Contains(".html"))
                        {
                            await Scrape(linkUrl);
                        }
                    }
                }
            }
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
