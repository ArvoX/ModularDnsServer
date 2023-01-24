namespace ModularDnsServer.Core.Interface;

public interface IPasiveResolver : IResolver
{
  public Task InitCacheAsync(IDnsCache cache, CancellationToken cancellationToken);
}
