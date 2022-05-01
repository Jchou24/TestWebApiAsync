TestAsyncLoopCountConccurent
    10萬筆
        吞吐量 100
            一次處理100筆
        單次處理時間 3 秒
        處理時間須約 1 小時
            ( 100000 / 100 ) * 3 = 3000(秒) = 50 (分鐘)
        
        10萬筆，吞吐量50，跑 2185 秒 = 36 分
    5000筆
        吞吐量 100
        變動時間task(分別是 1,2,3 秒)
        134 秒 = 2 分鐘

每個 task 都不 sleep(讓等候時間為0)
    TestAsyncLoopCountConccurent_WithZeroRunningTimeTask
    5000筆
        吞吐量 1
            31 秒
            CPU 使用率會變超高
        吞吐量 10
            27 秒
            CPU 使用率會變超高
        吞吐量 100
            29 秒
            CPU 使用率會變超高
        
    10萬筆
        吞吐量 100
        620 秒 = 10 分鐘
        CPU 使用率會變超高

發restful 故意用using發
    5000筆
        吞吐量 100
            8.6 秒
            CPU 使用率會變超高

    10萬筆
        吞吐量 100
        會出現連線已用盡的 error
            錯誤訊息
                System.Net.Http.HttpRequestException: 一次只能用一個通訊端位址 (通訊協定/網路位址/連接埠)。 (localhost:44308)
                ---> System.Net.Sockets.SocketException (10048): 一次只能用一個通訊端位址 (通訊協定/網路位址/連接埠)。
                at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.ThrowException(SocketError error, CancellationToken cancellationToken)
                at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.System.Threading.Tasks.Sources.IValueTaskSource.GetResult(Int16 token)
                at System.Net.Sockets.Socket.<ConnectAsync>g__WaitForConnectWithCancellation|283_0(AwaitableSocketAsyncEventArgs saea, ValueTask connectTask, CancellationToken cancellationToken)
                at System.Net.Http.HttpConnectionPool.DefaultConnectAsync(SocketsHttpConnectionContext context, CancellationToken cancellationToken)
                at System.Net.Http.ConnectHelper.ConnectAsync(Func`3 callback, DnsEndPoint endPoint, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
                --- End of inner exception stack trace ---
                at System.Net.Http.ConnectHelper.ConnectAsync(Func`3 callback, DnsEndPoint endPoint, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
                at System.Net.Http.HttpConnectionPool.ConnectAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
                at System.Net.Http.HttpConnectionPool.CreateHttp11ConnectionAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
                at System.Net.Http.HttpConnectionPool.GetHttpConnectionAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
                at System.Net.Http.HttpConnectionPool.SendWithRetryAsync(HttpRequestMessage request, Boolean async, Boolean doRequestAuth, CancellationToken cancellationToken)
                at System.Net.Http.RedirectHandler.SendAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
                at System.Net.Http.DiagnosticsHandler.SendAsyncCore(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
                at System.Net.Http.HttpClient.SendAsyncCore(HttpRequestMessage request, HttpCompletionOption completionOption, Boolean async, Boolean emitTelemetryStartStop, CancellationToken cancellationToken)
                at TestUtility.RandomNumberApiClientService.GetNumberAsync(Nullable`1 waitTime) in D:\Programming_Related\Global_on_Github\Project\TestWebApiAsync\TestWebApiAsync\TestHelper\RandomNumberApiClientService.cs:line 35
                at WebApplication6.Controllers.TestAsyncLoopConcurrentController.TestAsyncLoopCountConccurent_WithZeroRunningTimeTask(Int32 count, Int32 conccurent, Boolean isShowDetail) in D:\Programming_Related\Global_on_Github\Project\TestWebApiAsync\TestWebApiAsync\Api\Controllers\TestAsyncLoopConcurrentController.cs:line 176
                at lambda_method5(Closure , Object )
                at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.AwaitableObjectResultExecutor.Execute(IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
                at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeActionMethodAsync>g__Awaited|12_0(ControllerActionInvoker invoker, ValueTask`1 actionResultValueTask)
                at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeNextActionFilterAsync>g__Awaited|10_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
                at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)
                at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
                at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeInnerFilterAsync>g__Awaited|13_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
                at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|19_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
                at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
                at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)
                at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
                at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)
                at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
                at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)

                HEADERS
                =======
                Accept: text/plain
                Accept-Encoding: gzip, deflate, br
                Accept-Language: zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7
                Connection: close
                Cookie: G_AUTHUSER_H=0
                Host: localhost:44351
                Referer: https://localhost:44351/swagger/index.html
                User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36
                sec-ch-ua: " Not A;Brand";v="99", "Chromium";v="100", "Google Chrome";v="100"
                sec-ch-ua-mobile: ?0
                sec-ch-ua-platform: "Windows"
                sec-fetch-site: same-origin
                sec-fetch-mode: cors
                sec-fetch-dest: empty
            相關討論
                https://stackoverflow.com/questions/1339142/wcf-system-net-socketexception-only-one-usage-of-each-socket-address-protoco
            解法
                用 HttpClientFactory，重用連線

將 Conccurent 的實作用 SemaphoreSlim 改寫
    每個 task 都不 sleep(讓等候時間為0)
        5000筆
            吞吐量 100
                25 ~ 30 秒
                CPU 使用率會變超高
    變動時間task(分別是 1,2,3 秒)
        5000筆
            吞吐量 100
                146 秒 = 2.5 分鐘
                136 秒 = 2 分鐘