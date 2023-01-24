using ModularDnsServer.Core.Dns;

namespace ModularDnsServer.Core.Interface;

public interface IActiveResolver : IResolver
{
  public Task<Message> ResolvAsync(Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken);
}