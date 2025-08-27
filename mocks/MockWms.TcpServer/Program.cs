using System.Net;
using System.Net.Sockets;
using System.Text;

const int port = 8888;
//port the TCP server will listen on 

var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Console.WriteLine($"Mock WMS TCP Server started. Listening on port {{Port}}...");
Console.WriteLine("Waiting for connections...");

while (true)
{
    TcpClient client = await listener.AcceptTcpClientAsync();

    _ = Task.Run(() => HandleClient(client));
}

static async Task HandleClient(TcpClient client)
{
    
}