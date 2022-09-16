using Calzolari.Grpc.Net.Client.Validation;
using CountryService.Client;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Compression;
using Microsoft.Extensions.Logging;
using Sample.gRPC.v1;
using static Sample.gRPC.v1.CountryService;
var loggerFactory = LoggerFactory.Create(logging =>
{
 logging.SetMinimumLevel(LogLevel.Trace);
});


var logger = loggerFactory.CreateLogger<TracerInterceptor>();
var handler = new SocketsHttpHandler
{
    KeepAlivePingDelay = TimeSpan.FromSeconds(15),
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
    // Timeout.InfiniteTimeSpan for infinite idle connection
    KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
    EnableMultipleHttp2Connections = true
};


var channel = GrpcChannel.ForAddress("https://localhost:7112", new GrpcChannelOptions {
LoggerFactory = loggerFactory,
    HttpHandler = handler,
CompressionProviders = new List<ICompressionProvider>
{
new BrotliCompressionProvider()
},
    MaxReceiveMessageSize = 6291456, // 6 MB,
    MaxSendMessageSize = 6291456 // 6 MB

});

var countryClient = new CountryServiceClient(channel.Intercept(new TracerInterceptor(logger)));
using var bidirectionnalStreamingCall = countryClient.Create();
try
{
    var countriesToCreate = new List<CountryCreationRequest>
{
new CountryCreationRequest
{
Name = "Japan", Description = "",
CreateDate =    Timestamp.FromDateTime(DateTime.SpecifyKind (DateTime.UtcNow, DateTimeKind.Utc))
}
};

    // Write
    foreach (var countryToCreate in countriesToCreate)
    {
        await bidirectionnalStreamingCall.RequestStream.WriteAsync(countryToCreate);
        Console.WriteLine($"Country {countryToCreate.Name} set for creation");
    }


    // Tells server that request streaming is done
    await bidirectionnalStreamingCall.RequestStream.CompleteAsync();
    // Read
    await foreach (var createdCountry in bidirectionnalStreamingCall.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"{createdCountry.Name} has been created with Id:    { createdCountry.Id}        ");
    }
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
{
    var errors = ex.GetValidationErrors(); 
    Console.WriteLine(ex.Message);
}
catch (RpcException ex)
{
    Console.WriteLine(ex.Message);
}
//It’s cleaner, isn’t it?.

//using var serverStreamingCall = countryClient.GetAll(new Empty()); 
//await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync())
//{
//    Console.WriteLine($"{response.Name}: {response.Description}");
//}

logger.LogDebug("Call to GetAll function ended");
channel.Dispose();
await channel.ShutdownAsync();
