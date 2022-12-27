namespace ModularDnsServer.Core.Interface;

public interface IPasiveReslover : IResolver
{
  public Task InitCacheAsync(IDnsCache cache);
}
