using Azure.Storage.Blobs;
using DemoApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Diagnostics;

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

            Uri blobUri = new Uri("https://anushkademostore.blob.core.windows.net/democont/sample.txt");

            TokenAcquisitionTokenCredential credential = new TokenAcquisitionTokenCredential(_token);
            BlobClient blobClient = new BlobClient(blobUri, credential);

            using var ms = new MemoryStream();
            blobClient.DownloadTo(ms);
            ms.Position = 0;
            using var _reader = new StreamReader(ms);
            string str = _reader.ReadToEnd();
            ViewBag.content = str;

            return View();
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