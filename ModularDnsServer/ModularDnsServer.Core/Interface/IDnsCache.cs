using ModularDnsServer.Core.Dns;

namespace ModularDnsServer.Core.Interface;

public interface IDnsCache<T>
  where T : IRecordData
{
  //public CacheResult CacheMessage(Guid id, Message message);
  //public ClearResult ClearMessage(Guid id);
}
