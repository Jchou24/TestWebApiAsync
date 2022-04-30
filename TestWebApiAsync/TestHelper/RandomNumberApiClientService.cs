using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TestUtility.Models;

namespace TestUtility
{
    public class RandomNumberApiClientService: IRandomNumberApiClientService
    {
        private readonly HttpClient _httpClient;

        public RandomNumberApiClientService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://localhost:44308/");
            this._httpClient = httpClient;
        }

        public async Task<ServiceResult> GetNumberAsync()
        {
            var response = await _httpClient.GetAsync("RandomNumber");
            var responseString = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<ApiResult>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return new ServiceResult(apiResult);
        }

        public ServiceResult GetNumber()
        {
            var response = _httpClient.GetAsync("RandomNumber").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            var apiResult = JsonSerializer.Deserialize<ApiResult>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return new ServiceResult(apiResult);
        }
    }
}
