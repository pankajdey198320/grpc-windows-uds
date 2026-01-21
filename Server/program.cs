using GrpcUdsDemo.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var socketPath = Path.Combine(Path.GetTempPath(), “socket.tmp”);

// Delete existing socket file if it exists
if (File.Exists(socketPath))
{
File.Delete(socketPath);
Console.WriteLine($“Deleted existing socket file: {socketPath}”);
}

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on Unix Domain Socket
builder.WebHost.ConfigureKestrel(serverOptions =>
{
serverOptions.ListenUnixSocket(socketPath, listenOptions =>
{
listenOptions.Protocols = HttpProtocols.Http2;
});
});

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();

app.MapGet(”/”, () => “Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909”);

Console.WriteLine($“Server listening on Unix Domain Socket: {socketPath}”);
Console.WriteLine(“Press Ctrl+C to shutdown”);

app.Run();
