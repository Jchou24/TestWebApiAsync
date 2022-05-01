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
        /// async call with amount: "count", conccurent, batch size
        /// </summary>
        /// <returns></returns>
        [HttpGet("{count}/{batchSize}")]
        public async Task<ActionResult<TestResult>> TestAsyncLoopCountConccurentBatch(int count, int batchSize)
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<ServiceResult>();
            int numberOfBatches = (int)Math.Ceiling((double)count / batchSize);


            //var serviceTasks = new List<Func<int?, Task<ServiceResult>>>();
            //for (int i = 0; i < count; i++)
            //{
            //    serviceTasks.Add(_randomNumberApiClientService.GetNumberAsync);
            //}
            var serviceTasks = Enumerable
                .Repeat<Func<int?, Task<ServiceResult>>>(_randomNumberApiClientService.GetNumberAsync, count)
                .ToList();

            int sleepTime = 1;
            int Counter()
            {
                var time = sleepTime * 1000;
                sleepTime += 1;
                if (sleepTime == 4)
                {
                    sleepTime = 1;
                }
                return time;
            }

            var currentTask = new List<Task<ServiceResult>>();
            var doneTasks = new List<Task<ServiceResult>>();
            var maxCurrentTaskCount = 10;
            while (serviceTasks.Any()) {
                var idleCount = batchSize - currentTask.Count;
                if (idleCount > 0)
                {
                    var newTasks = serviceTasks.Take(idleCount).Select(x => x(Counter()) );
                    currentTask.AddRange(newTasks);
                    serviceTasks.RemoveRange(0, idleCount);
                }

                await Task.WhenAny(currentTask);
                //await Task.WhenAll(currentTask);

                //var isUpdateCurrentTask = currentTask.Count(x => x.IsCompleted) > maxCurrentTaskCount || serviceTasks.Count < maxCurrentTaskCount;
                //if (!isUpdateCurrentTask)
                //{
                //    continue;
                //}


                //var doneTaskIds = currentTask
                //    .Where(x => x.IsCompleted)
                //    .Select((x, idx) => idx)
                //    .OrderByDescending(x => x)
                //    .ToList();
                //doneTasks.AddRange(currentTask.Where((x, idx) => doneTaskIds.Contains(idx)));
                //foreach (var doneTaskId in doneTaskIds)
                //{
                //    currentTask.RemoveAt(doneTaskId);
                //}

                var firstDoneTaskId = currentTask
                    .Where(x => x.IsCompleted)
                    .Select((x, idx) => idx)
                    .First();

                doneTasks.Add(currentTask[firstDoneTaskId]);
                currentTask.RemoveAt(firstDoneTaskId);
            }

            await Task.WhenAll(currentTask);
            doneTasks.AddRange(currentTask);

            var doneTaskResults = Task.WhenAll(doneTasks);
            var serviceResults = await doneTaskResults;

            //var testResult = new TestResult(testStartDT, doneTasks.Select(x => x.Result).ToList());
            var testResult = new TestResult(testStartDT, serviceResults.ToList());
            //testResult.ServiceResults = new List<ServiceResult>();
            return Ok(testResult);
        }
    }
}
