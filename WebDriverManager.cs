using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Web.Drivers
{
    public class WebDriverManager
    {
        private string _chromeDriverPath;
        private string _chromeBrowserPath;

        public WebDriverManager()
        {
            _chromeDriverPath = GetChromeDriverPath();
            _chromeBrowserPath = GetChromeBrowserPath();
        }

        public string GetChromeBroswerVersion()
        {
            var latestChromeVersion = string.Empty;
            try
            {
                // Get the latest stable version of Chrome   
                latestChromeVersion = GetLatestStableChromeVersion();

                // Check if the ChromeDriver binaries are already available in the temp memory   
                if (IsChromeDriverAvailableInTempMemory(latestChromeVersion))
                {
                    return latestChromeVersion;
                }
                else
                {
                    // Download the ChromeDriver binaries   
                    DownloadAndExtractChromeDriver(latestChromeVersion);
                    // Delete older ChromeDriver binaries   
                    DeleteOlderChromeDriverBinaries();
                    return latestChromeVersion;
                }
            }
            catch
            {
                if (IsOldChromeDriverAvailableInTempMemory())
                {
                    var versionDirectory = Directory.GetDirectories(_chromeDriverPath).FirstOrDefault();

                    if (versionDirectory != null)
                    {
                        var chromeVersion = Path.GetFileName(versionDirectory);
                        return chromeVersion;
                    }
                }
                return null;
            }
        }

        private string GetLatestStableChromeVersion()
        {
            // Get the latest stable version of Chrome from the Chrome for Testing platform   
            var url = "https://googlechromelabs.github.io/chrome-for-testing/LATEST_RELEASE_STABLE";
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            var latestChromeVersion = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return latestChromeVersion;
        }

        private bool IsChromeDriverAvailableInTempMemory(string chromeVersion)
        {
            // Check if the ChromeDriver binaries are already available in the temp memory   
            var chromeDriverExeFile = Path.Combine(_chromeDriverPath, chromeVersion, "chromedriver-win64", "chromedriver.exe");
            var chromeBrowserExeFile = Path.Combine(_chromeBrowserPath, chromeVersion, "chrome-win64", "chrome.exe");

            return File.Exists(chromeDriverExeFile) && File.Exists(chromeBrowserExeFile);
        }

        private bool IsOldChromeDriverAvailableInTempMemory()
        {
            // Check if the ChromeDriver binaries are already available in the temp memory   
            //    var chromeDriverExeFile = Path.Combine(_chromeDriverPath, "chromedriver-win64", "chromedriver.exe");
            //    var chromeBrowserExeFile = Path.Combine(_chromeBrowserPath, "chrome-win64", "chrome.exe");

            return Directory.Exists(_chromeDriverPath) && Directory.Exists(_chromeBrowserPath);
        }
        private void DownloadAndExtractChromeDriver(string chromeVersion)
        {
            // Download the latest ChromeDriver binaries from the ChromeDriver website   
            var url = $"https://storage.googleapis.com/chrome-for-testing-public/{chromeVersion}/win64/chromedriver-win64.zip";
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            var chromeDriverZipFile = Path.GetTempFileName();

            // Save the ChromeDriver binaries to a temporary file   
            using (var stream = response.GetResponseStream())
            {
                using (var fileStream = new FileStream(chromeDriverZipFile, FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                }
            }

            // Create the directory for the ChromeDriver version  
            var chromeDriverVersionPath = Path.Combine(_chromeDriverPath, chromeVersion);
            if (!Directory.Exists(chromeDriverVersionPath))
            {
                Directory.CreateDirectory(chromeDriverVersionPath);
            }

            // Extract the ChromeDriver binaries   
            using (var zipArchive = ZipFile.OpenRead(chromeDriverZipFile))
            {
                foreach (var entry in zipArchive.Entries)
                {
                    var destinationPath = Path.Combine(chromeDriverVersionPath, entry.FullName);
                    // Ensure the directory exists  
                    var destinationDirectory = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    // Skip if the entry is a directory  
                    if (string.IsNullOrEmpty(entry.Name)) continue;

                    entry.ExtractToFile(destinationPath, true);
                }
            }

            // Delete the temporary zip file   
            File.Delete(chromeDriverZipFile);

            // Download the latest Chrome browser binaries from the Chrome for Testing platform   
            var chromeBrowserUrl = $"https://storage.googleapis.com/chrome-for-testing-public/{chromeVersion}/win64/chrome-win64.zip";
            var chromeBrowserRequest = WebRequest.Create(chromeBrowserUrl);
            var chromeBrowserResponse = chromeBrowserRequest.GetResponse();
            var chromeBrowserZipFile = Path.GetTempFileName();

            // Save the Chrome browser binaries to a temporary file   
            using (var stream = chromeBrowserResponse.GetResponseStream())
            {
                using (var fileStream = new FileStream(chromeBrowserZipFile, FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                }
            }

            // Create the directory for the Chrome browser version  
            var chromeBrowserVersionPath = Path.Combine(_chromeBrowserPath, chromeVersion);
            if (!Directory.Exists(chromeBrowserVersionPath))
            {
                Directory.CreateDirectory(chromeBrowserVersionPath);
            }

            // Extract the Chrome browser binaries   
            using (var zipArchive = ZipFile.OpenRead(chromeBrowserZipFile))
            {
                foreach (var entry in zipArchive.Entries)
                {
                    var destinationPath = Path.Combine(chromeBrowserVersionPath, entry.FullName);
                    // Ensure the directory exists  
                    var destinationDirectory = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    // Skip if the entry is a directory  
                    if (string.IsNullOrEmpty(entry.Name)) continue;

                    entry.ExtractToFile(destinationPath, true);
                }
            }

            // Delete the temporary zip file   
            File.Delete(chromeBrowserZipFile);
        }

        private void DeleteOlderChromeDriverBinaries()
        {
            // Get the latest stable version of Chrome   
            var latestChromeVersion = GetLatestStableChromeVersion();

            // Get the directories in the ChromeDriver path  
            var chromeDriverDirectories = Directory.GetDirectories(_chromeDriverPath);

            // Delete the directories that are not the latest version  
            foreach (var directory in chromeDriverDirectories)
            {
                if (directory != Path.Combine(_chromeDriverPath, latestChromeVersion))
                {
                    Directory.Delete(directory, true);
                }
            }

            // Get the directories in the Chrome browser path  
            var chromeBrowserDirectories = Directory.GetDirectories(_chromeBrowserPath);

            // Delete the directories that are not the latest version  
            foreach (var directory in chromeBrowserDirectories)
            {
                if (directory != Path.Combine(_chromeBrowserPath, latestChromeVersion))
                {
                    Directory.Delete(directory, true);
                }
            }
        }

        public string GetChromeDriverPath()
        {
            // Get the path to the ChromeDriver executable   
            var chromeDriverPath = Path.Combine(Path.GetTempPath(), "chromedriver");

            return chromeDriverPath;
        }

        public string GetChromeBrowserPath()
        {
            // Get the path to the Chrome browser executable   
            var chromeBrowserPath = Path.Combine(Path.GetTempPath(), "chrome");

            return chromeBrowserPath;
        }
    }
}
