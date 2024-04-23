using System.Net.Http;
using System;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WebScraper;

namespace WebScraper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            String url = "https://books.toscrape.com/";

            WebScraper scraper = new WebScraper();

            try
            {
                scraper.Scrape(url);
            }
            catch (Exception)
            {
                Console.WriteLine("ERROR: Could not scrape");
                throw;
            }

            
        }

    }
}
