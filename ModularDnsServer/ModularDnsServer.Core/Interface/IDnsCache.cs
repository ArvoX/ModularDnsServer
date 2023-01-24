using ModularDnsServer.Core.Dns;
using ModularDnsServer.Core.Dns.ResourceRecords;
using System.Data;

namespace ModularDnsServer.Core.Interface;

public interface IDnsCache
{
  public Task<IResourceRecord[]> GetRecordsAsync(Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken);
  public Task Init(CancellationToken cancellationToken);
}
