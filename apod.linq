<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Security.Principal</Namespace>
</Query>

#load ".\GetEntries"
#load ".\GetImageStream"
#load ".\GetImageUrl"
#load ".\SetWallpaper"

const string HISTORY_FILE_NAME = "history.json";
const int MINIMUM_HEIGHT = 1200;
const int MINIMUM_WIDTH = 1920;

async Task Main()
{
	if (!IsAdmin())
	{
		"Not admin.".Dump();
		return;
	}
	
	var history = LoadHistory();
	
	Directory.CreateDirectory(GetImageDirectoryPath());
		
	var entries = await GetEntriesAsync();
	
	entries.Dump();
	
	foreach(var pageUrl in entries)
	{
		$"Page URL: {pageUrl}".Dump();
		
		var historyEntry = GetOrCreateHistoryEntry(history, pageUrl);
		
		if (historyEntry.Processed)
		{
			"Already processed.".Dump();
			OutputBreakAndSetProcessed(historyEntry);
			continue;
		}

		var imageUrl = await GetImageUrl(pageUrl);
		
		if (imageUrl == null)
		{
			OutputBreakAndSetProcessed(historyEntry);
			continue;
		}
		
		$"Image URL: {imageUrl}".Dump();
				
		historyEntry.ImageURL = imageUrl;

		var imageStream = await GetImageStream(imageUrl);
		
		var image = Image.FromStream(imageStream);

		image.Dump();

		$"{image.Width} x {image.Height}".Dump();
		
		if (image.Width < MINIMUM_WIDTH || image.Height < MINIMUM_HEIGHT)
		{
			$"Skipping. Too small.".Dump();
			OutputBreakAndSetProcessed(historyEntry);
			continue;
		}
		
		var shouldSaveAndSetInput = Util.ReadLine<string>("Save and set?");
		
		historyEntry.Processed = true;
		
		if (!shouldSaveAndSetInput.StartsWith("y", ignoreCase: true, CultureInfo.InvariantCulture))
		{
			OutputBreakAndSetProcessed(historyEntry);
			continue;
		}
		
		var imageFilePath = SaveImage(imageUrl, imageStream);			
				
		SetWallpaper(imageFilePath);
		
		OutputBreakAndSetProcessed(historyEntry);
		
		break;
	}
	
	SaveHistory(history);
}

string GetFullHistoryPath()
	=> Path.Combine(GetScriptDirectoryPath(), HISTORY_FILE_NAME);
	
string GetImageDirectoryPath()
	=> Path.Combine(GetScriptDirectoryPath(), "images");
	
HistoryEntry GetOrCreateHistoryEntry(List<HistoryEntry> history, string pageUrl)
{
	var historyEntry = history.SingleOrDefault(e => e.PageURL == pageUrl);
			
	if (historyEntry == null)
	{
		historyEntry = new HistoryEntry
		{
			PageURL = pageUrl
		};

		history.Add(historyEntry);
	}

	return historyEntry;
}

string GetScriptDirectoryPath() => Path.GetDirectoryName(Util.CurrentQueryPath);

public static bool IsAdmin()
{
	var identity = WindowsIdentity.GetCurrent();
	var principal = new WindowsPrincipal(identity);
	return principal.IsInRole(WindowsBuiltInRole.Administrator);
}

List<HistoryEntry> LoadHistory()
{
	var historyPath = GetFullHistoryPath();
	
	if (!File.Exists(historyPath))
		return new List<HistoryEntry>();
	
	var historyJson = File.ReadAllText(historyPath);
	
	return JsonConvert.DeserializeObject<List<HistoryEntry>>(historyJson);
}

void OutputBreakAndSetProcessed(HistoryEntry historyEntry)
{
	new String('-', 10).Dump();
	historyEntry.Processed = true;
}

void SaveHistory(List<HistoryEntry> history)
{
	var historyPath = GetFullHistoryPath();

	var historyJson = JsonConvert.SerializeObject(history, Newtonsoft.Json.Formatting.Indented);

	File.WriteAllText(historyPath, historyJson);
}

string SaveImage(string imageUrl, Stream imageStream)
{
	var imageFileName = new Flurl.Url(imageUrl).PathSegments[^1];
	
	imageFileName.Dump();
		
	var imageFullPath = Path.Combine(GetImageDirectoryPath(), imageFileName);

	using (var fileStream = new FileStream(imageFullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
	{
		imageStream.Seek(0, SeekOrigin.Begin);
		imageStream.CopyTo(fileStream);
	}
	
	return imageFullPath;
}

class HistoryEntry
{
	public string PageURL { get; set; }
	public string ImageURL { get; set; }
	public bool Processed { get; set; }
}
