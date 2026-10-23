using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using zSubscription.TCPServer.KhoiNBD.Models;

namespace zSubscription.TCPServer.KhoiNBD;

internal static class Program
{
    private const string Host = "127.0.0.1";
    private const int Port = 5000;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("zSubscription TCP SERVER - KhoiNBD");

        using var shutdown = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            shutdown.Cancel();
        };

        var listener = new TcpListener(IPAddress.Parse(Host), Port);

        try
        {
            listener.Start();
            Console.WriteLine($"Server started at {Host}:{Port}");
            Console.WriteLine("Press Ctrl+C to stop the server.");

            while (!shutdown.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(shutdown.Token);
                _ = Task.Run(() => HandleClientAsync(client, shutdown.Token), shutdown.Token);
            }
        }
        catch (OperationCanceledException) when (shutdown.IsCancellationRequested)
        {
            Console.WriteLine("Server is stopping...");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Server socket error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Server error: {ex.Message}");
        }
        finally
        {
            listener.Stop();
        }
    }

    private static async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        var remoteAddress = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
        Console.WriteLine($"Client connected: {remoteAddress}");

        try
        {
            using (client)
            await using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true))
            await using (var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true)
            {
                AutoFlush = true
            })
            {
                var request = (await reader.ReadLineAsync(cancellationToken))?.Trim();
                var response = CreateResponse(request);
                await writer.WriteLineAsync(JsonSerializer.Serialize(response, JsonOptions));
                Console.WriteLine($"Processed request '{request ?? "<empty>"}' for {remoteAddress}");
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Server shutdown requested.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client error ({remoteAddress}): {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"Client disconnected: {remoteAddress}");
        }
    }

    private static object CreateResponse(string? request)
    {
        if (string.IsNullOrWhiteSpace(request))
        {
            return new
            {
                RequestType = request,
                Status = "Invalid request",
                Message = "Request must be an ID, member name, package name or ALL."
            };
        }

        var getAll = request.Equals("ALL", StringComparison.OrdinalIgnoreCase);
        var hasId = int.TryParse(request, out var membershipFeeId);

        List<MembershipFeeKhoiNbdRecord> items = DataStore.MembershipFees
            .Where(item =>
                getAll ||
                (hasId && item.MembershipFeeKhoiNbdid == membershipFeeId) ||
                (!hasId && item.MemberName.Contains(request, StringComparison.OrdinalIgnoreCase)) ||
                (!hasId && item.PackageName?.Contains(request, StringComparison.OrdinalIgnoreCase) == true))
            .ToList();

        return items.Count > 0
            ? items
            : new
            {
                RequestType = request,
                Status = "Not found",
                Message = $"No membership fee matched request '{request}'."
            };
    }
}
