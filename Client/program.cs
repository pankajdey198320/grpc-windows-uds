using System.Net;
using System.Net.Sockets;
using Grpc.Net.Client;
using GrpcUdsDemo;

var socketPath = Path.Combine(Path.GetTempPath(), “socket.tmp”);

Console.WriteLine($“Connecting to Unix Domain Socket: {socketPath}”);

// Create channel using Unix Domain Socket
using var channel = CreateChannel(socketPath);

// Create gRPC client
var client = new Greeter.GreeterClient(channel);

// Make multiple calls to test the connection
for (int i = 1; i <= 5; i++)
{
try
{
var reply = await client.SayHelloAsync(new HelloRequest { Name = $“User{i}” });
Console.WriteLine($“Response {i}: {reply.Message}”);
}
catch (Exception ex)
{
Console.WriteLine($“Error on call {i}: {ex.Message}”);
}


await Task.Delay(1000); // Wait 1 second between calls

}

Console.WriteLine(”\nPress any key to exit…”);
Console.ReadKey();

static GrpcChannel CreateChannel(string socketPath)
{
var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
var connectionFactory = new UnixDomainSocketsConnectionFactory(udsEndPoint);
var socketsHttpHandler = new SocketsHttpHandler
{
ConnectCallback = connectionFactory.ConnectAsync
};

return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
{
    HttpHandler = socketsHttpHandler
});


}

public class UnixDomainSocketsConnectionFactory
{
private readonly EndPoint endPoint;


public UnixDomainSocketsConnectionFactory(EndPoint endPoint)
{
    this.endPoint = endPoint;
}

public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _,
    CancellationToken cancellationToken = default)
{
    var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

    try
    {
        await socket.ConnectAsync(this.endPoint, cancellationToken).ConfigureAwait(false);
        return new NetworkStream(socket, true);
    }
    catch
    {
        socket.Dispose();
        throw;
    }
}


}
