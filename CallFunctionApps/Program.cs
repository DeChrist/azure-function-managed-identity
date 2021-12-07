const string ClientFunctionAppUrl = "https://fa-mi-client-001.azurewebsites.net/api/FrontendProcess";

using var client = new HttpClient();

var response = await client.GetAsync(ClientFunctionAppUrl + $"?name={DateTime.UtcNow}" );

string content = await response.Content.ReadAsStringAsync();

Console.WriteLine(content);
