using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestUtility.Models;

namespace ApiB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RandomNumberController : ControllerBase
    {
        

        private readonly ILogger<RandomNumberController> _logger;

        public RandomNumberController(ILogger<RandomNumberController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<ApiResult> Get()
        {
            var startDateTime = DateTime.Now;
            Thread.Sleep(3000);

            return new ApiResult(startDateTime);
        }
    }
}
