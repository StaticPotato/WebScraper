using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebScraper
{
    class WebScraper
    {
        public void Scrape(String url)
        {
            HttpClient client = new HttpClient();
            HtmlDocument document = new HtmlDocument();

            String html = client.GetStringAsync(url).Result;

            document.LoadHtml(html);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/WEBSCRAPE";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            document.Save(path + "/web.html");
        }
    }
}
