using ModularDnsServer.Core.Dns;
using ModularDnsServer.Core.Dns.Serialization;
using ModularDnsServer.Core.Interface;
using System.Net;
using System.Net.Sockets;

namespace ModularDnsServer.Resolvers.StaticResolvers
{
  public class ConfigurableUpstreamResolver : IActiveResolver
  {
    public ConfigurableUpstreamResolver(IPEndPoint endPoint)
    {
      EndPoint = endPoint;
    }

    public IPEndPoint EndPoint { get; }

    public async Task<Message> ResolvAsync(Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken)
    {
      using var client = new UdpClient();
      client.Connect(EndPoint);
      await client.SendAsync(orginalData, cancellationToken);
      var result = await client.ReceiveAsync(cancellationToken);
      return MessageParser.ParseMessage(result.Buffer);
    }
  }
}
