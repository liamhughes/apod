<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

#load ".\GetEntries"
#load ".\GetImageStream"
#load ".\GetImageUrl"


async Task Main()
{
	var entries = await GetEntriesAsync();
	
	foreach(var pageUrl in entries)
	{
		var imageUrl = await GetImageUrl(pageUrl);
		
		if (imageUrl == null)
			continue;

		var imageStream = await GetImageStream(imageUrl);
		
		var image = Image.FromStream(imageStream);

		image.Dump();
	}
}

	
	imageUrls.Dump();
}
