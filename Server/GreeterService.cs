using Grpc.Core;
using GrpcUdsDemo;

namespace GrpcUdsDemo.Services;

public class GreeterService : Greeter.GreeterBase
{
private readonly ILogger<GreeterService> _logger;


public GreeterService(ILogger<GreeterService> logger)
{
    _logger = logger;
}

public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
{
    _logger.LogInformation($"Received greeting request from: {request.Name}");
    
    return Task.FromResult(new HelloReply
    {
        Message = $"Hello {request.Name}! (via Unix Domain Socket)"
    });
}


}
