using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace ServiceFunctionApp
{
    public static class Function1
    {
        [FunctionName("BackendProcess")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ClaimsPrincipal principal,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            StringBuilder principalInfoBuilder = new StringBuilder($"Principal carries following Claims:\r\n");
            if (principal?.Claims != null)
            {
                foreach (Claim claim in principal.Claims)
                {
                    principalInfoBuilder.Append($"\tClaim of type '{claim.Type}' issued by '{claim.Issuer}': '{claim.Value}'\r\n");
                }
            }
            else
            {
                principalInfoBuilder.Append("none found!");
            }
            log.LogInformation(principalInfoBuilder.ToString());

            string responseMessage = $"Here is your backend response ! - {name}";

            return new OkObjectResult(responseMessage);
        }
    }
}
