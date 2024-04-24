Short Description:

A small web scraper that sifts through HTML files of a website and recursively finds and downloads the entire web structure. This allows you to view the website in its entirety completely locally from your PC.

Thoughts:

Goals vs Outcomes: I feel that I had expectations quite similar to the outcome both in time and complexity, except for parallelism, since im not all to familira with it in C#. There were things I had not worked with before and had great fun challenging myself with that aspect.

Technical Implementation: The program I made is not very large, but I feel like I maintained a good balance between making the program general enough to be used for other websites while still completing the task at hand. A possible improvement would be to query the user for a root URL from which the scraper would scrape. As I said previously, the parallelism works, but I would have liked to spend some more time on it and refine it further. I feel that I have structured the code well enough for the time that I had. There could be some improvements like splitting the scrape function into parts where I only handle images and links separately from each other. If I had more time, I would make sure the "m_Client.GetStringAsync()" would handle situations where it doesn't fetch. I would do that by retrying x amount of times and if it failed too many times, cancel scrape and return a message like "Error: No connection to target web address".

Process of the whole program goes like this: In Main, we have a try/catch where we attempt to call the Run function from WebScraper. Run then initializes root variables like the root website and the root directory on the PC. Run then calls FetchNextHtml(), which is a recursive function that goes through the web page structure and parses the data, making it ready for scraping. While the FetchNextHtml function does that, it also calls for the scrape function to run on the side and start downloading and saving the parsed files found. In turn, scrape creates two Parallel.ForEachAsync lambdas that go through a whole HTML page and downloads all the connected files. The image loading and handles get handled right there in the scrape function, but in the part where we look for other links to HTML files and such, we call a function called "ParseLink()". Parse link then downloads and parses the inputted URL and creates directories and files to be used by the local website. Lastly, there are two helper functions "GetAbsolutePath" and "GetRelativePath". GetAbsolutePath() takes the URL and node attributes to then convert it to give out an absolute directory path of where the file should be in your system. GetRelativePath() takes in a URL and splits it and gives out the relevant part for the local file path.

Room for Improvements: For scalability, I would like to improve the way I check for duplicated HTML files since it's an ever-increasing list that needs to be iterated through.


Run instructions:

Step 1: Download Visual Studio 2022.

Step 2: Through NuGet package handler, install HtmlAgilityPack.

Step 3: Open the project with Visual Studio 2022.

Step 4: Press Run.

Step 5: After 2-4 minutes, a folder should be present on your desktop called "WEBSCRAPE" where the website will work locally.


