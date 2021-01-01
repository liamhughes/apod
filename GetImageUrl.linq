<Query Kind="Program">
  <NuGetReference>Flurl.Http</NuGetReference>
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <Namespace>Flurl</Namespace>
  <Namespace>Flurl.Http</Namespace>
  <Namespace>HtmlAgilityPack</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

private const string EXAMPLE_PAGE_URL = "https://apod.nasa.gov/apod/ap201225.html";

async Task Main()
{
	var imageUrl = await GetImageUrl(EXAMPLE_PAGE_URL);
	
	imageUrl.Dump();
}

public async Task<string> GetImageUrl(string pageUrl)
{
	var html = await GetHtml(pageUrl);
	
	var relativeImageUrl = GetRelativeImageUrlFromHtml(html);
	
	if (relativeImageUrl == null)
	{
		Console.WriteLine($"Cannot find image URL from {pageUrl}.");
		return null;
	}
	
	var absoluteImageUrl = new Flurl.Url(pageUrl).RemovePathSegment().AppendPathSegment(relativeImageUrl);
	
	return absoluteImageUrl;
}

private async Task<string> GetHtml(string pageUrl)
{
	var request = new Flurl.Url(pageUrl);

	var response = await request.GetStringAsync();

	return response;
}

private string GetRelativeImageUrlFromHtml(string html)
{
	var doc = new HtmlDocument();
	doc.LoadHtml(html);

	var aTags = doc.DocumentNode.SelectNodes("//a");

	foreach(var aTag in aTags)
	{
		var children = aTag.ChildNodes;
		if (!children.Any(n => n.Name.Equals("img", StringComparison.InvariantCultureIgnoreCase)))
			continue;
		return aTag.Attributes["href"].Value;
	}

	return null;
}
