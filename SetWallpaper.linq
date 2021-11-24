<Query Kind="Program">
  <Namespace>Flurl</Namespace>
  <Namespace>Flurl.Http</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>Microsoft.Win32</Namespace>
</Query>

/* https://stackoverflow.com/questions/1061678/change-desktop-wallpaper-using-code-in-net */

void Main()
{
	
}

[DllImport("user32.dll", CharSet = CharSet.Auto)]
static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

const int SPI_SETDESKWALLPAPER = 20;
const int SPIF_UPDATEINIFILE = 0x01;
const int SPIF_SENDWININICHANGE = 0x02;

public void SetWallpaper(string filePath)
{
	var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
	
	key.SetValue(@"WallpaperStyle", 6.ToString());
	key.SetValue(@"TileWallpaper", 0.ToString());
	
	SystemParametersInfo(SPI_SETDESKWALLPAPER,
		0,
		filePath,
		SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
				
	var lockScreenKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\PersonalizationCSP");
	
	lockScreenKey.SetValue("LockScreenImagePath", filePath);	
}