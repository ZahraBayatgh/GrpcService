using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountryService.Client
{
    public class TracerInterceptor : Interceptor
    {
        private readonly ILogger<TracerInterceptor> _logger;
        public TracerInterceptor(ILogger<TracerInterceptor> logger)
        {
            _logger = logger;
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext
        <TRequest, TResponse> context, AsyncClientStreamingCallContinuation
        <TRequest, TResponse> continuation)


        where TRequest : class where TResponse : class
        {
_logger.LogDebug($"Executing {context.Method.Name} {context. Method.Type} method on server {context.Host}");
return continuation(context);
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext
    <TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation
    <TRequest, TResponse> continuation) where TRequest : class
    where TResponse : class
    {
_logger.LogDebug($"Executing {context.Method.Name} {context. Method.Type} method on service {context.Method.ServiceName}"); return continuation(context);
}

public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext < TRequest, TResponse > context, AsyncServerStreamingCallContinuation < TRequest, TResponse > continuation)
where TRequest : class where TResponse: class
{
_logger.LogDebug($"Executing {context.Method.Name} {context.Method.Type} method on service {context.Method.ServiceName}"); return continuation(request, context);
}

public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
where TRequest : class


where TResponse : class
{
    _logger.LogDebug($"Executing {context.Method.Name} {context.Method.Type} method on service {context.Method.ServiceName}"); return continuation(request, context);
}
}

}
