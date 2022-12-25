using ModularDnsServer.Core.Dns;

namespace ModularDnsServer.Core.Interface;

public interface IPasiveReslover : IResolver
{
}

public interface IActiveResolver : IResolver
{
  public Message Resolv(Message message);
}