using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

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

            if (links != null)
            {
                foreach (HtmlNode link in links)
                {
                    string linkUrl = link.Attributes["href"].Value;

                    Uri baseUri = new Uri(url);

                    if (!Uri.IsWellFormedUriString(linkUrl, UriKind.Absolute))
                    {
                        Uri absoluteUri = new Uri(baseUri, linkUrl);
                        linkUrl = absoluteUri.ToString();
                    }

                    string[] splitUrl = linkUrl.Split(m_RootUrl);

                    if (splitUrl.Length > 1 && !File.Exists(m_RootPath + "/" + splitUrl[1]))
                    {
                        await ParseLink(splitUrl[1]);
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
    }
}
