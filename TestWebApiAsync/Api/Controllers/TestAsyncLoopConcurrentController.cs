using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestUtility.Models;

namespace WebApplication6.Controllers
{
    /// <summary>
    /// Add buffering mechanism to handle huge request
    /// </summary>
    [ApiController]
    [Route("[controller]/[Action]")]
    public class TestAsyncLoopConcurrentController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IRandomNumberApiClientService _randomNumberApiClientService;

        public TestAsyncLoopConcurrentController(ILogger<TestController> logger, IRandomNumberApiClientService randomNumberApiClientService)
        {
            _logger = logger;
            _randomNumberApiClientService = randomNumberApiClientService;
        }

        class CountGenerator
        {            
            int _sleepTime = 1;
            int _maxSleepTime = 4;

            public int GetCount()
            {
                var time = _sleepTime * 1000;
                _sleepTime += 1;
                if (_sleepTime == _maxSleepTime)
                {
                    _sleepTime = 1;
                }
                return time;
            }
        }

        /// <summary>
        /// async call with amount: "count", batch size
        /// </summary>
        /// <returns></returns>
        [HttpGet("{count}/{batchSize}/{isShowDetail}")]
        public async Task<ActionResult<TestResult>> TestAsyncLoopCountBatch(int count, int batchSize, bool isShowDetail = false)
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<ServiceResult>();

            var serviceTasks = Enumerable
                .Repeat<Func<int?, Task<ServiceResult>>>(_randomNumberApiClientService.GetNumberAsync, count)
                .ToList();

            var countGenerator = new CountGenerator();
            var currentTask = new List<Task<ServiceResult>>();
            var doneTasks = new List<Task<ServiceResult>>();
            while (serviceTasks.Any())
            {
                
                {
                    var newTasks = serviceTasks.Take(batchSize).Select(x => x(countGenerator.GetCount()));
                    currentTask.AddRange(newTasks);
                    serviceTasks.RemoveRange(0, batchSize);
                }

                await Task.WhenAll(currentTask);
                doneTasks.AddRange(currentTask);
                currentTask.Clear();
            }

            await Task.WhenAll(currentTask);
            doneTasks.AddRange(currentTask);

            var testResult = new TestResult(testStartDT, (await Task.WhenAll(doneTasks)).ToList());
            if (!isShowDetail)
            {
                testResult.ServiceResults = new List<ServiceResult>();
            }
            return Ok(testResult);
        }

        /// <summary>
        /// async call with amount: "count", conccurent
        /// </summary>
        /// <returns></returns>
        [HttpGet("{count}/{conccurent}/{isShowDetail}")]
        public async Task<ActionResult<TestResult>> TestAsyncLoopCountConccurent(int count, int conccurent, bool isShowDetail = false)
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<ServiceResult>();

            var serviceTasks = Enumerable
                .Repeat<Func<int?, Task<ServiceResult>>>(_randomNumberApiClientService.GetNumberAsync, count)
                .ToList();

            var countGenerator = new CountGenerator();
            var currentTask = new List<Task<ServiceResult>>();
            var doneTasks = new List<Task<ServiceResult>>();
            while (serviceTasks.Any())
            {
                var idleCount = conccurent - currentTask.Count;
                if (idleCount > 0)
                {
                    var newTasks = serviceTasks.Take(idleCount).Select(x => x(countGenerator.GetCount()));
                    currentTask.AddRange(newTasks);
                    serviceTasks.RemoveRange(0, idleCount);
                }

                await Task.WhenAny(currentTask);
                var firstDoneTaskId = currentTask
                    .Where(x => x.IsCompleted)
                    .Select((x, idx) => idx)
                    .First();
                
                doneTasks.Add(currentTask[firstDoneTaskId]);
                currentTask.RemoveAt(firstDoneTaskId);
            }

            await Task.WhenAll(currentTask);
            doneTasks.AddRange(currentTask);

            var testResult = new TestResult(testStartDT, (await Task.WhenAll(doneTasks)).ToList());
            if (!isShowDetail)
            {
                testResult.ServiceResults = new List<ServiceResult>();
            }
            return Ok(testResult);
        }

        /// <summary>
        /// async call with amount: "count", conccurent. Each task running time set to zero to observe thread scheduling time.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{count}/{conccurent}/{isShowDetail}")]
        public async Task<ActionResult<TestResult>> TestAsyncLoopCountConccurent_WithZeroRunningTimeTask(int count, int conccurent, bool isShowDetail = false)
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<ServiceResult>();

            var serviceTasks = Enumerable
                .Repeat<Func<int?, Task<ServiceResult>>>(_randomNumberApiClientService.GetNumberAsync, count)
                .ToList();

            //var countGenerator = new CountGenerator();
            var currentTask = new List<Task<ServiceResult>>();
            var doneTasks = new List<Task<ServiceResult>>();
            while (serviceTasks.Any())
            {
                var idleCount = conccurent - currentTask.Count;
                if (idleCount > 0)
                {
                    var newTasks = serviceTasks.Take(idleCount).Select(x => x(0));
                    currentTask.AddRange(newTasks);
                    serviceTasks.RemoveRange(0, idleCount);
                }

                await Task.WhenAny(currentTask);
                var firstDoneTaskId = currentTask
                    .Where(x => x.IsCompleted)
                    .Select((x, idx) => idx)
                    .First();

                doneTasks.Add(currentTask[firstDoneTaskId]);
                currentTask.RemoveAt(firstDoneTaskId);
            }

            await Task.WhenAll(currentTask);
            doneTasks.AddRange(currentTask);

            var testResult = new TestResult(testStartDT, (await Task.WhenAll(doneTasks)).ToList());
            if (!isShowDetail)
            {
                testResult.ServiceResults = new List<ServiceResult>();
            }
            return Ok(testResult);
        }

        /// <summary>
        /// async call with amount: "count", conccurent
        /// </summary>
        /// <returns></returns>
        [HttpGet("{count}/{conccurent}/{isShowDetail}")]
        public async Task<ActionResult<TestResult>> TestAsyncLoopCountConccurent_bySemaphoreSlim(int count, int conccurent, bool isShowDetail = false)
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<ServiceResult>();
            var semaphoreSlim = new SemaphoreSlim(conccurent, conccurent);

            var countGenerator = new CountGenerator();
            var serviceTasks = Enumerable
                .Repeat<Func<int?, Task<ServiceResult>>>(_randomNumberApiClientService.GetNumberAsync, count)
                .Select(async GetNumberAsync => {
                    try
                    {
                        await semaphoreSlim.WaitAsync();

                        return await GetNumberAsync(countGenerator.GetCount());
                    }
                    catch
                    {
                        // Ignore any exception to continue loading other URLs.
                        // You should definitely log the exception in a real
                        // life application.
                        //return new HttpResponseMessage();
                        return new ServiceResult();
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                })
                .ToList();

            var testResult = new TestResult(testStartDT, (await Task.WhenAll(serviceTasks)).ToList());
            if (!isShowDetail)
            {
                testResult.ServiceResults = new List<ServiceResult>();
            }
            return Ok(testResult);
        }

        /// <summary>
        /// async call with amount: "count", conccurent
        /// </summary>
        /// <returns></returns>
        [HttpGet("{count}/{conccurent}/{isShowDetail}")]
        public async Task<ActionResult<TestResult>> TestAsyncLoopCountConccurent_WithZeroRunningTimeTask_bySemaphoreSlim(int count, int conccurent, bool isShowDetail = false)
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<ServiceResult>();
            var semaphoreSlim = new SemaphoreSlim(conccurent, conccurent);

            var countGenerator = new CountGenerator();
            var serviceTasks = Enumerable
                .Repeat<Func<int?, Task<ServiceResult>>>(_randomNumberApiClientService.GetNumberAsync, count)
                .Select(async GetNumberAsync => {
                    try
                    {
                        await semaphoreSlim.WaitAsync();

                        //return await GetNumberAsync(countGenerator.GetCount());
                        return await GetNumberAsync(0);
                    }
                    catch
                    {
                        // Ignore any exception to continue loading other URLs.
                        // You should definitely log the exception in a real
                        // life application.
                        //return new HttpResponseMessage();
                        return new ServiceResult();
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                })
                .ToList();

            var testResult = new TestResult(testStartDT, (await Task.WhenAll(serviceTasks)).ToList());
            if (!isShowDetail)
            {
                testResult.ServiceResults = new List<ServiceResult>();
            }
            return Ok(testResult);
        }

        /// <summary>
        /// async call with amount: "count", conccurent, different task with same Semaphore
        /// </summary>
        /// <returns></returns>
        [HttpGet("{count}/{conccurent}/{isShowDetail}")]
        public async Task<ActionResult<TestResult>> TestAsyncLoopCountConccurent_WithZeroRunningTimeTask_bySemaphoreSlim_SharedSemaphore(int count, int conccurent, bool isShowDetail = false)
        {
            var testStartDT = DateTime.Now;
            var serviceTaskResults = new List<ServiceResult>();
            var semaphoreSlim = new SemaphoreSlim(conccurent, conccurent);

            var countGenerator = new CountGenerator();
            var serviceTasks1 = Enumerable
                .Repeat<Func<int?, Task<ServiceResult>>>(_randomNumberApiClientService.GetNumberAsync, count / 2)
                .Select(async GetNumberAsync => {
                    try
                    {
                        await semaphoreSlim.WaitAsync();

                        //return await GetNumberAsync(countGenerator.GetCount());
                        return await GetNumberAsync(0);
                    }
                    catch
                    {
                        // Ignore any exception to continue loading other URLs.
                        // You should definitely log the exception in a real
                        // life application.
                        //return new HttpResponseMessage();
                        return new ServiceResult();
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                })
                .ToList();

            var serviceTasks2 = Enumerable
                .Repeat<Func<int?, Task<ServiceResult>>>(_randomNumberApiClientService.GetNumberAsync, count - (count / 2))
                .Select(async GetNumberAsync => { // 定義一個 task
                    try
                    {
                        // 執行前固定等待
                        // 當 semaphor 有空閒資源，就會離開下一行，否則在下一行等待
                        await semaphoreSlim.WaitAsync();

                        // 真正要做的事
                        //return await GetNumberAsync(countGenerator.GetCount());
                        return await GetNumberAsync(0);
                    }
                    catch
                    {
                        // Ignore any exception to continue loading other URLs.
                        // You should definitely log the exception in a real
                        // life application.
                        //return new HttpResponseMessage();
                        return new ServiceResult();
                    }
                    finally
                    {
                        // 做完後 release semaphor
                        semaphoreSlim.Release();
                    }
                })
                .ToList();

            var serviceTasks1Result = (await Task.WhenAll(serviceTasks1)).ToList();
            var serviceTasks2Result = (await Task.WhenAll(serviceTasks2)).ToList();
            var serviceTasks = serviceTasks1Result;
            serviceTasks.AddRange(serviceTasks2Result);

            //var serviceTasks = serviceTasks1;
            //serviceTasks.AddRange(serviceTasks2);

            var testResult = new TestResult(testStartDT, serviceTasks);
            //var testResult = new TestResult(testStartDT, (await Task.WhenAll(serviceTasks)).ToList());
            if (!isShowDetail)
            {
                testResult.ServiceResults = new List<ServiceResult>();
            }
            return Ok(testResult);
        }
    }
}
