using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhishAnalyzer.Helpers
{
    public class GetScreenshot
    {
         

        public static void GetScreenshotStart( String url, int counter)
        {
            var beginCaptureTask = BeginCapture(url, "1200x800", "true", "firefox", "true");
            string key = beginCaptureTask.Result;

            if (key == null)
            {
                Console.WriteLine("No Key returned, make sure you specified the correct API_KEY");
                return;
            }
            beginCaptureTask = BeginCapture(url, "1200x800", "true", "firefox", "true");
            key = beginCaptureTask.Result;
            int timeout = 30;
            int tCounter = 0;
            int tCountIncr = 3;

            while (true)
            {
                var tryRetrieveTask = TryRetrieve(key);
                var result = tryRetrieveTask.Result;

                if (result.Success)
                {
                    Console.WriteLine("downloaded screenshot for key: " + key);
                    File.WriteAllBytes("wwwroot/images/" +counter+ ".png", result.Bytes);
                    break;
                }

                tCounter += tCountIncr;
                System.Threading.Thread.Sleep(tCountIncr * 1000);
                Console.WriteLine("Screenshot not yet ready.. waiting for: " + tCountIncr.ToString() + " seconds.");

                if (tCounter > timeout)
                {
                    Console.WriteLine("timed out while downloading: " + key + " " + DateTime.Now.ToShortTimeString());
                    break;
                }
            }

        }

        public class RetrieveResult
        {
            public bool Success { get; set; }
            public byte[] Bytes { get; set; }
        }

        const string API_KEY = "a67f319a-0fb2-4b58-b65a-e5d22ed24e8d";

        public static async Task<RetrieveResult> TryRetrieve(string key)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("apikey", API_KEY);
                string serverUrl = "https://api.screenshotapi.io/retrieve?key=" + key;
                Console.WriteLine("Downloading:" + serverUrl);
                var response = await httpClient.GetAsync(serverUrl).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var jsonResults = JsonConvert.DeserializeObject<dynamic>(result);

                    RetrieveResult retResult = new RetrieveResult();
                    //{ "status":"ready","imageUrl":"http://screenshotapi.s3.amazonaws.com/captures/f469a4c54b4852b046c6f210935679ae.png"}
                    if (jsonResults.status == "ready")
                    {
                        retResult.Success = true;
                        WebClient client = new WebClient();
                        string downloadUrl = jsonResults.imageUrl;
                        retResult.Bytes = client.DownloadData(downloadUrl);
                    }
                    else
                    {
                        retResult.Success = false;
                    }
                     
                    return retResult;
                }
                else
                {
                    Console.WriteLine("Invalid Status Received:" + response.StatusCode);
                }
            }

            return null;
        }


        public static async Task<string> BeginCapture(string url, string viewport, string fullpage, string webdriver, string javascript)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("apikey", API_KEY);
                var serverUrl = "https://api.screenshotapi.io/capture";

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("url",  WebUtility.UrlEncode(url)),
                    new KeyValuePair<string, string>("viewport",  viewport),
                    new KeyValuePair<string, string>("fullpage",  fullpage),
                    new KeyValuePair<string, string>("webdriver",  webdriver),
                    new KeyValuePair<string, string>("javascript",  javascript)
                });


                var response = await httpClient.PostAsync(serverUrl, formContent).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var jsonResults = JsonConvert.DeserializeObject<dynamic>(result);
                    string key = jsonResults.key;
                    Console.WriteLine(key);
                    return key;
                }
                else
                {
                    Console.WriteLine("Invalid Status Received:" + response.StatusCode);
                }
            }
            return null;
        }

    }
}