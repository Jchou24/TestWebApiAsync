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

        private static string _GetUri(int? waitTime) => "RandomNumber" + (waitTime != null ? $"/{waitTime}" : "");

        public async Task<ServiceResult> GetNumberAsync(int? waitTime = null)
        {
            var response = await _httpClient.GetAsync(_GetUri(waitTime));
            var responseString = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<ApiResult>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return new ServiceResult(apiResult);
        }

        /// <summary>
        /// Unstable method, avoid to use .Result
        /// </summary>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public ServiceResult GetNumber(int? waitTime = null)
        {
            var response = _httpClient.GetAsync(_GetUri(waitTime)).Result; // avoid to use .Result
            var responseString = response.Content.ReadAsStringAsync().Result; // avoid to use .Result
            var apiResult = JsonSerializer.Deserialize<ApiResult>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return new ServiceResult(apiResult);
        }
    }
}
