using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUtility.Models;

namespace WebApplication6.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IRandomNumberApiClientService _randomNumberApiClientService;

        public TestController(ILogger<TestController> logger, IRandomNumberApiClientService randomNumberApiClientService)
        {
            _logger = logger;
            _randomNumberApiClientService = randomNumberApiClientService;
        }

        /// <summary>
        /// sync with same thread
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<TestResult> TestSync()
        {
            var testStartDT = DateTime.Now;
            var serviceResult = new List<ServiceResult>();

            serviceResult.Add(_randomNumberApiClientService.GetNumber());
            serviceResult.Add(_randomNumberApiClientService.GetNumber());
            serviceResult.Add(_randomNumberApiClientService.GetNumber());

            var testResult = new TestResult(testStartDT, serviceResult);
            return Ok(testResult);
        }

        /// <summary>
        /// sync with same thread
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<TestResult> TestSync2()
        {
            var testStartDT = DateTime.Now;
            var ServiceResults = new List<ServiceResult>();

            ServiceResults.Add(_randomNumberApiClientService.GetNumberAsync().Result);
            ServiceResults.Add(_randomNumberApiClientService.GetNumberAsync().Result);
            ServiceResults.Add(_randomNumberApiClientService.GetNumberAsync().Result);

            var testResult = new TestResult(testStartDT, ServiceResults);
            return Ok(testResult);
        }

        /// <summary>
        /// sync with different thread
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<TestResult> TestSync3()
        {
            TestResult testResult = _TestSync3().Result;
            return Ok(testResult);
        }

        private async Task<TestResult> _TestSync3()
        {
            var testStartDT = DateTime.Now;
            var serviceResults = new List<ServiceResult>();

            serviceResults.Add(await _randomNumberApiClientService.GetNumberAsync());
            serviceResults.Add(await _randomNumberApiClientService.GetNumberAsync());
            serviceResults.Add(await _randomNumberApiClientService.GetNumberAsync());

            var testResult = new TestResult(testStartDT, serviceResults);
            return testResult;
        }

        /// <summary>
        /// async with different thread
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<TestResult> TestAsync1()
        {
            TestResult testResult = _TestAsync1().Result;
            return Ok(testResult);
        }

        private async Task<TestResult> _TestAsync1()
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<Task<ServiceResult>>();

            serviceTaskResults.Add(_randomNumberApiClientService.GetNumberAsync());
            serviceTaskResults.Add(_randomNumberApiClientService.GetNumberAsync());
            serviceTaskResults.Add(_randomNumberApiClientService.GetNumberAsync());

            foreach (var serviceTaskResult in serviceTaskResults)
            {
                await serviceTaskResult;
            }

            var apiResults = serviceTaskResults.Select(x => x.Result).ToList();

            var testResult = new TestResult(testStartDT, apiResults);
            return testResult;
        }

        /// <summary>
        /// async with different thread by loop
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<TestResult> TestAsyncLoop()
        {
            TestResult testResult = _TestAsync2().Result;
            return Ok(testResult);
        }

        private async Task<TestResult> _TestAsync2()
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<Task<ServiceResult>>();

            for (int i = 0; i < 3; i++)
            {
                serviceTaskResults.Add(_randomNumberApiClientService.GetNumberAsync());
            }            

            foreach (var serviceTaskResult in serviceTaskResults)
            {
                await serviceTaskResult;
            }

            var apiResults = serviceTaskResults.Select(x => x.Result).ToList();

            var testResult = new TestResult(testStartDT, apiResults);
            return testResult;
        }
    }
}
