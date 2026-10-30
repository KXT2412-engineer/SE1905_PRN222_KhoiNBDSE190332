using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using zSubscription.Entities.KhoiNBD.Models;

namespace zSubscription.WorkerService.KhoiNBD;

public sealed class MembershipFeeSnapshotWriter
{
    private readonly WorkerOptions _options;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public MembershipFeeSnapshotWriter(IOptions<WorkerOptions> options)
        => _options = options.Value;

    public string FilePath => Path.GetFullPath(_options.FilePath, AppContext.BaseDirectory);

    public async Task WriteAsync(
        IEnumerable<MembershipFeeKhoiNbd> membershipFees,
        CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(membershipFees, JsonOptions);
        var line = $"{DateTimeOffset.Now:O}: {json}{Environment.NewLine}";
        await File.AppendAllTextAsync(FilePath, line, Encoding.UTF8, cancellationToken);
    }
}
