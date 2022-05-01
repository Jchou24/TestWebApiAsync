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
    /// test for looping call random number api, since there are no scheduling mechanism, big "loop count" will occure exception
    /// </summary>
    [ApiController]
    [Route("[controller]/[Action]")]
    public class TestAsyncLoopController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IRandomNumberApiClientService _randomNumberApiClientService;

        public TestAsyncLoopController(ILogger<TestController> logger, IRandomNumberApiClientService randomNumberApiClientService)
        {
            _logger = logger;
            _randomNumberApiClientService = randomNumberApiClientService;
        }

        private async Task<TestResult> _TestAsyncLoop(int loopCount)
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = Enumerable
                .Range(0, loopCount)
                .Select(x => _randomNumberApiClientService.GetNumberAsync());

            var apiResults = await Task.WhenAll(serviceTaskResults);
            var testResult = new TestResult(testStartDT, apiResults.ToList());
            return testResult;
        }

        /// <summary>
        /// async with different thread by loop, await each task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<TestResult>> TestBasicAsyncLoop()
        {
            var testResult = _TestAsyncLoop(3);
            return Ok(testResult);
        }

        #region
        /// <summary>
        /// async call with amount: "count"
        /// </summary>
        /// <returns></returns>
        [HttpGet("{count}")]
        public async Task<ActionResult<TestResult>> TestAsyncLoopCount(int count)
        {
            var testResult = await _TestAsyncLoop(count);
            testResult.ServiceResults = new List<ServiceResult>();
            return Ok(testResult);
        }

        /// <summary>
        /// async call with amount: "count", with Task.Result, big amount has probability to occur exception
        /// </summary>
        /// <returns></returns>
        [HttpGet("{count}")]
        public ActionResult<TestResult> TestAsyncLoopCountWithResult(int count)
        {
            TestResult testResult = _TestAsyncLoop(count).Result;
            testResult.ServiceResults = new List<ServiceResult>();
            return Ok(testResult);
        }
        #endregion
    }
}
