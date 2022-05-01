using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TestUtility.Models;

namespace TestUtility
{
    public class RandomNumberApiClientService: IRandomNumberApiClientService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:44308/";

        public RandomNumberApiClientService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(_baseUrl);
            this._httpClient = httpClient;

            //var maxConcurrentRequests = 100;
            //ServicePointManager.FindServicePoint(new Uri("https://localhost:44308/RandomNumber")).ConnectionLimit = maxConcurrentRequests;
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
        /// request with using approach. Parallel request will occure tcp/ip error: System.Net.Sockets.SocketException (10048)
        /// </summary>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        //public async Task<ServiceResult> GetNumberAsync(int? waitTime = null)
        //{
        //    using (var httpClient = new HttpClient())
        //    {
        //        var response = await httpClient.GetAsync(_baseUrl + _GetUri(waitTime));
        //        var responseString = await response.Content.ReadAsStringAsync();
        //        var apiResult = JsonSerializer.Deserialize<ApiResult>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        //        return new ServiceResult(apiResult);
        //    }            
        //}

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
