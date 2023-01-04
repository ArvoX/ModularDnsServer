namespace ModularDnsServer.Core;

public record class ServerConfiguration
{
  public ServerConfiguration()
  {
    UpdPort = 53;
    TcpPort = 53;
  }

  public int TcpPort { get; init; }
  public int UpdPort { get; init; }
}
