using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.Identity.Web;
using System.Text;

namespace DemoApp.Controllers
{
    public class FeatureController : Controller
    {
        private readonly IFeatureManager _featureManager;
        readonly ITokenAcquisition _token;

        public FeatureController(IFeatureManagerSnapshot featureManager, ITokenAcquisition token)
        {
            _featureManager = featureManager;
            _token = token;
        }
        [FeatureGate(Feature.staging)]
        public async Task<IActionResult> IndexAsync()
        {
            Uri queueUri = new Uri("https://az204storage1.queue.core.windows.net/azure-demo-queue");

            TokenAcquisitionTokenCredential credential = new TokenAcquisitionTokenCredential(_token);
            QueueClient queueClient = new QueueClient(queueUri, credential);

            if (await queueClient.ExistsAsync())
            {
                var response = await queueClient.PeekMessagesAsync(1);
                var peekedMessages = response.Value;
                foreach (var msg in peekedMessages)
                {
                    try
                    {
                        var messageBodyByteArray = FromBase64Bytes(msg.Body.ToArray());
                        var messageBody = Encoding.UTF8.GetString(messageBodyByteArray);

                        var data1 = System.Text.Json.JsonSerializer.Deserialize<Root>(messageBodyByteArray);
                        ViewBag.content = $"Msg {msg.MessageId}: created @ {msg.InsertedOn}, expires @ {msg.ExpiresOn}\n\t- {data1.data.Hello}\n";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                }
            }
            return View();
        }

        byte[] FromBase64Bytes(byte[] base64Bytes)
        {
            string base64String = Encoding.UTF8.GetString(base64Bytes, 0, base64Bytes.Length);
            return Convert.FromBase64String(base64String);
        }
    }
}
