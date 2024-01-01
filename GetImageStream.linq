<Query Kind="Program">
  <NuGetReference>Flurl.Http</NuGetReference>
  <Namespace>Flurl</Namespace>
  <Namespace>Flurl.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

private const string EXAMPLE_IMAGE_URL = "https://apod.nasa.gov/apod/image/2012/WinterSceneBlock.jpg";

async Task Main()
{
	var imageStream = await GetImageStream(EXAMPLE_IMAGE_URL);
	
	var image = Image.FromStream(imageStream);
	
	image.Dump();
}

public async Task<Stream> GetImageStream(string imageUrl)
{
	return await new Flurl.Url(imageUrl).WithTimeout(TimeSpan.FromMinutes(10)).GetStreamAsync();
}
