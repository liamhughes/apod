<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

#load ".\GetEntries"
#load ".\GetImageStream"
#load ".\GetImageUrl"

const int MINIMUM_HEIGHT = 1200;
const int MINIMUM_WIDTH = 1920;

async Task Main()
{
	var entries = await GetEntriesAsync();
	
	entries.Dump();
	
	foreach(var pageUrl in entries)
	{

		$"Page URL: {pageUrl}".Dump();
			
		var imageUrl = await GetImageUrl(pageUrl);
		
		if (imageUrl == null)
			continue;
			
		$"Image URL: {imageUrl}".Dump();

		var imageStream = await GetImageStream(imageUrl);
		
		var image = Image.FromStream(imageStream);

		image.Dump();

		$"{image.Width} x {image.Height}".Dump();
		
		if (image.Width < MINIMUM_WIDTH || image.Height < MINIMUM_HEIGHT)
		{
			$"Skipping. Too small.".Dump();
			continue;
		}
		
		var shouldSaveAndSetInput = Util.ReadLine<string>("Save and set?");
		
		if (!shouldSaveAndSetInput.StartsWith("y", ignoreCase: true, CultureInfo.InvariantCulture))
			continue;
			
		"Saving and setting".Dump();
		
		
		
		new String('-', 10).Dump();
	}
}

	
	imageUrls.Dump();
}
