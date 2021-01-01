<Query Kind="Program">
  <NuGetReference>Flurl.Http</NuGetReference>
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <NuGetReference>Microsoft.Toolkit.Parsers</NuGetReference>
  <Namespace>Flurl</Namespace>
  <Namespace>Flurl.Http</Namespace>
  <Namespace>HtmlAgilityPack</Namespace>
  <Namespace>Microsoft.Toolkit</Namespace>
  <Namespace>Microsoft.Toolkit.Parsers</Namespace>
  <Namespace>Microsoft.Toolkit.Parsers.Rss</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const string FEED_URL = "https://apod.nasa.gov/apod.rss";
const string GENERAL_URL = "https://apod.nasa.gov/apod/astropix.html";

async Task Main()
{
	var entries = await GetEntriesAsync();
	entries.Dump();
}

public async Task<IEnumerable<string>> GetEntriesAsync()
{
	var rssString = await GetRssString();	
	
	var rssEntries = ParseRssString(rssString);
	
	var entryURLs = rssEntries.Select(e => RssEntryToURL(e));
	
	return entryURLs.Where(u => u != GENERAL_URL).OrderBy(u => u);
}

private async Task<string> GetRssString()
{
	var request = new Flurl.Url(FEED_URL);

	var response = await request.GetStringAsync();

	return response;
}

private IEnumerable<RssSchema> ParseRssString(string rssString)
{
	var rssParser = new Microsoft.Toolkit.Parsers.Rss.RssParser();

	return rssParser.Parse(rssString);
}

private string RssEntryToURL(RssSchema entry)
{
	var content = entry.Content;

	var doc = new HtmlDocument();
	doc.LoadHtml(content);

	var aTag = doc.DocumentNode.SelectSingleNode("//p/a");

	var hrefAttribute = aTag.Attributes["href"];

	return hrefAttribute.Value;
}
