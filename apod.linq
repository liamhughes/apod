<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\GetEntries"

async Task Main()
{
	var entries = await GetEntriesAsync();
	
	entries.Dump();
}
