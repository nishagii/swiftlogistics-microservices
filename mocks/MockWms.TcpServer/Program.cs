using System.Net;
using System.Net.Sockets;
using System.Text;

const int port = 8888;
//port the TCP server will listen on 

var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Console.WriteLine($"Mock WMS TCP Server started. Listening on port {port}...");
Console.WriteLine("Waiting for connections...");

while (true)
{
    // "await" pauses the execution of the while loop until the AcceptTcpClientAsync() method completes
    TcpClient client = await listener.AcceptTcpClientAsync();

    //handle multiple clients concurrently
    // way to run code on a background thread,this offload work from the main thread
    _ = Task.Run(() => HandleClient(client));
}
//client handling logic
static async Task HandleClient(TcpClient client)
{
    var clientEndpoint = client.Client.RemoteEndPoint?.ToString();
    Console.WriteLine($"\n--- New connection from {clientEndpoint} ---");

    try
    {
        await using NetworkStream stream = client.GetStream();

        //reading the incoming message
        byte[] buffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        string requestMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"Received message: \"{requestMessage.Trim()}\"");


        Console.WriteLine("Simulating warehouse process...");

        await SendMessage(stream, "ACK:PACKAGE_DETAILS_RECEIVED");
        await Task.Delay(1000); // Simulate finding the package

        await SendMessage(stream, "STATUS:PACKAGE_LOCATED_IN_BIN_A34");
        await Task.Delay(1500); // Simulate moving package to loading bay

        await SendMessage(stream, "STATUS:PACKAGE_LOADED_ONTO_VEHICLE");
        await SendMessage(stream, "END"); // Signal that the conversation is over
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error handling client {clientEndpoint}: {ex.Message}");
    }
    finally
    {
        Console.WriteLine($"--- Connection from {clientEndpoint} closed ---");
        client.Close();
    }
}

static async Task SendMessage(NetworkStream stream, string message)
{
    byte[] messageBytes = Encoding.UTF8.GetBytes(message + "\n"); // Add newline as a delimiter
    await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
    Console.WriteLine($"Sent message: \"{message.Trim()}\"");
}

//tested using TELNET - allows to connect to and communicate with remote computers over a transmission control protocol/Internet protocol (TCP/IP) network
// test using telnet : telnet localhost 8888 -> ADD_PACKAGE:ORD-789