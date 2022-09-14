using Calzolari.Grpc.AspNetCore.Validation;
using Grpc.Net.Compression;
using GrpcService;
using GrpcService.Compression;
using GrpcService.Interceptors;
using GrpcService.Services;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CountryManagementService>();
// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc(
options =>
{
    options.EnableDetailedErrors = true; // Enabling error details
    options.IgnoreUnknownServices = true;
    options.MaxReceiveMessageSize = 6291456; // 6 MB
    options.MaxSendMessageSize = 6291456; // 6 MB options.CompressionProviders = new List<ICompression Provider>
    options.CompressionProviders = new List<ICompressionProvider>{
        new GzipCompressionProvider(CompressionLevel.Optimal), // gzip
        new BrotliCompressionProvider(CompressionLevel.Optimal) // br
    };
    options.ResponseCompressionAlgorithm = "br"; // grpc- accept-encoding, and must match the compression provider declared in CompressionProviders collection
    options.ResponseCompressionLevel = CompressionLevel.Optimal; // compression level used if not set on the provider
    options.Interceptors.Add<ExceptionInterceptor>();
    options.EnableMessageValidation();// Register custom ExceptionInterceptor interceptor
});
builder.Services.AddGrpcValidation(); builder.Services.AddValidator<CountryCreateRequestValidator>();
builder.Services.AddGrpcReflection();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcReflectionService();
app.MapGrpcService<CountryGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
