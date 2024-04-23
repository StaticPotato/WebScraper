using System.Net.Http;
using System;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WebScraper;

namespace WebScraper
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            String url = "https://books.toscrape.com/";

            WebScraper scraper = new WebScraper();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/WEBSCRAPE";
            scraper.Init(path, url);
            try
            {
                await scraper.Scrape(url);
            }
            catch (Exception)
            {
                Console.WriteLine("ERROR: Could not scrape");
                throw;
            }
            Console.WriteLine("Finished");
            
        }

    }
}
