using ModularDnsServer.Core.Dns;

namespace ModularDnsServer.Core.Interface;

public interface IDnsCache
{
  public CacheResult CacheMessage(Guid id, Message message);
  public ClearResult ClearMessage(Guid id);
}
