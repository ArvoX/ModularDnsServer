using System.Net;

namespace ModularDnsServer.Core;

public record class ServerConfiguration
{
  public ServerConfiguration()
  {
    UpdPort = new IPEndPoint(IPAddress.Any, 53);
    TcpPort = new IPEndPoint(IPAddress.Any, 53);
  }

  public IPEndPoint TcpPort { get; init; }
  public IPEndPoint UpdPort { get; init; }
}
