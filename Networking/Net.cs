using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Net
{
    // Shared HttpClient instance (recommended for performance)
    private static readonly HttpClient client = new HttpClient();

    // GET request method
    public static async Task PerformGetRequest(string url)
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Throws if not 2xx

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("GET Response:");
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"GET request error: {e.Message}");
        }
    }

    // POST request method
    public static async Task PerformPostRequest<T>(string url, T data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("POST Response:");
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"POST request error: {e.Message}");
        }
    }
}
