 // Get the ChromeDriver version  
 var chromeBroswerVersion = webDriverManager.GetChromeBroswerVersion();
 var chromedriverWin64Directory = Directory.EnumerateDirectories(webDriverManager.GetChromeDriverPath(), "chromedriver-win64", SearchOption.AllDirectories).FirstOrDefault();
                                 

 if (chromedriverWin64Directory != null)
 {
     chromeDriverExePath = Path.Combine(chromedriverWin64Directory, "chromedriver.exe");
     // Use the chromeBrowserExePath variable to launch the ChromeBrowser  
 }
 var options = GetChromeOptions(deviceType);
 cService = ChromeDriverService.CreateDefaultService(chromedriverWin64Directory);
 cService.HideCommandPromptWindow = true;

 // Print the ChromeDriver version  
 Console.WriteLine($"ChromeDriver version: {chromeBroswerVersion}");

 // Create a new instance of the ChromeDriver  
 var driver = new ChromeDriver(cService, options);
