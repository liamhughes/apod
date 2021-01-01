<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\GetEntries"
#load ".\GetImageUrl"

async Task Main()
{
	var entries = await GetEntriesAsync();
	
	var imageUrls = entries.Select(async e => await GetImageUrl(e));
	
	imageUrls.Dump();
}
