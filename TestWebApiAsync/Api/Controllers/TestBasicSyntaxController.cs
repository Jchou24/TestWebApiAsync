using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUtility.Models;

namespace WebApplication6.Controllers
{
    /// <summary>
    /// test async method vs calling async method with .Result
    /// </summary>
    [ApiController]
    [Route("[controller]/[Action]")]
    public class TestBasicSyntaxController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IRandomNumberApiClientService _randomNumberApiClientService;

        public TestBasicSyntaxController(ILogger<TestController> logger, IRandomNumberApiClientService randomNumberApiClientService)
        {
            _logger = logger;
            _randomNumberApiClientService = randomNumberApiClientService;
        }

        #region TestSync
        // Syntax is valid
        // but the codes are unstable with .Result
        // since deadlock may occure


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
        public ActionResult<TestResult> TestSync1()
        {
            var testStartDT = DateTime.Now;
            var serviceResult = new List<ServiceResult>();

            serviceResult.Add(_randomNumberApiClientService.GetNumber(4000));
            serviceResult.Add(_randomNumberApiClientService.GetNumber(1500));
            serviceResult.Add(_randomNumberApiClientService.GetNumber(1000));

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
        #endregion

        #region TestAsync Basic
        // if the code calling path contain async/await
        // make function async
        // avoid to call .Result to prevent deadlock

        /// <summary>
        /// async with different thread, await each task( all take 3 sec )
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<TestResult>> TestAsync1()
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<Task<ServiceResult>>();

            serviceTaskResults.Add(_randomNumberApiClientService.GetNumberAsync());
            serviceTaskResults.Add(_randomNumberApiClientService.GetNumberAsync());
            serviceTaskResults.Add(_randomNumberApiClientService.GetNumberAsync());

            var apiResults = await Task.WhenAll(serviceTaskResults);
            var testResult = new TestResult(testStartDT, apiResults.ToList());
            return Ok(testResult);
        }

        /// <summary>
        /// async with different thread, await each task( each take 4, 1.5, 1 sec )
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<TestResult>> TestAsync2()
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<Task<ServiceResult>>();

            serviceTaskResults.Add(_randomNumberApiClientService.GetNumberAsync(4000));
            serviceTaskResults.Add(_randomNumberApiClientService.GetNumberAsync(1500));
            serviceTaskResults.Add(_randomNumberApiClientService.GetNumberAsync(1000));

            var apiResults = await Task.WhenAll(serviceTaskResults);
            var testResult = new TestResult(testStartDT, apiResults.ToList());
            return Ok(testResult);
        }

        #endregion
    }
}
