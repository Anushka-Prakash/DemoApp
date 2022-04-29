using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using DemoApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.WindowsAzure.Storage;
using System.Diagnostics;
using System.Text;

namespace DemoApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly ITokenAcquisition _token;

        public HomeController(ILogger<HomeController> logger, ITokenAcquisition token)
        {
            _logger = logger;
            _token = token;
        }

        public async Task<IActionResult> IndexAsync()
        {
            /*string[] scopes = new string[] { "https://storage.azure.com/user_impersonation" };
            string accesstoken = await _token.GetAccessTokenForUserAsync(scopes);
            ViewBag.Accesstoken = accesstoken;*/

            TokenAcquisitionTokenCredential credential = new TokenAcquisitionTokenCredential(_token);

            Uri blobUri = new Uri("https://anushkademostore.blob.core.windows.net/democont/sample.txt");
            
            BlobClient blobClient = new BlobClient(blobUri, credential);

            using var ms = new MemoryStream();
            blobClient.DownloadTo(ms);
            ms.Position = 0;
            using var _reader = new StreamReader(ms);
            string str = _reader.ReadToEnd();
            ViewBag.blobst = str;

            return View();
        }

        byte[] FromBase64Bytes(byte[] base64Bytes)
        {
            string base64String = Encoding.UTF8.GetString(base64Bytes, 0, base64Bytes.Length);
            return Convert.FromBase64String(base64String);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}