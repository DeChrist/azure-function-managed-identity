using Azure.Core;
using Azure.Identity;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClientFunctionApp
{
    public static class Function1
    {
        private const string ServiceFunctionUrl = "https://fa-mi-service-001.azurewebsites.net/api/BackendProcess";
        private const string ServiceAppId = "f8cd3da6-979b-4136-8bce-2068d16cd62e";

        [FunctionName("FrontendProcess")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var managedIdentity = new ManagedIdentityCredential();

            string[] scopes = { $"api://{ServiceAppId}" };

            var accessToken = await managedIdentity.GetTokenAsync(new TokenRequestContext(scopes, null));
            
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);

            var response = await client.GetAsync(ServiceFunctionUrl + $"?name={name}");

            string content = await response.Content.ReadAsStringAsync();

            string responseMessage = $"{name} - Service returned: [{content}]";

            return new OkObjectResult(responseMessage);
        }
    }
}
