using System.Net.Sockets;
using System.Text;
using System.Text.Json;

const string host = "127.0.0.1";
const int port = 5000;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("zSubscription TCP CLIENT - KhoiNBD");
Console.WriteLine("Start the TCP Server before sending a request.");
Console.WriteLine();

while (true)
{
    Console.Write("Enter Membership Fee ID, member/package name or ALL (EXIT to quit): ");
    var command = Console.ReadLine()?.Trim();

    if (command is null || command.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (string.IsNullOrWhiteSpace(command))
    {
        Console.WriteLine("Command must not be empty.");
        Console.WriteLine();
        continue;
    }

    try
    {
        using var client = new TcpClient();
        await client.ConnectAsync(host, port);
        Console.WriteLine($"Connected to {host}:{port}. Sending request: {command}");

        await using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        await using var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true)
        {
            AutoFlush = true
        };

        await writer.WriteLineAsync(command);
        var response = await reader.ReadLineAsync();

        Console.WriteLine("Server response:");
        PrintJson(response);
    }
    catch (SocketException)
    {
        Console.WriteLine($"Cannot connect to server at {host}:{port}.");
        Console.WriteLine("Make sure zSubscription.TCPServer.KhoiNBD is running first.");
    }
    catch (JsonException)
    {
        Console.WriteLine("The server returned invalid JSON.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
    }

    Console.WriteLine();
}

static void PrintJson(string? json)
{
    if (string.IsNullOrWhiteSpace(json))
    {
        Console.WriteLine("No response was received.");
        return;
    }

    using var document = JsonDocument.Parse(json);
    Console.WriteLine(JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions
    {
        WriteIndented = true
    }));
}
