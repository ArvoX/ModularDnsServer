namespace ModularDnsServer.Core;

//TODO shoud this be a sctruct?
public record class ServerConfiguration
{
  private ServerConfiguration()
  {
    UpdPort = 53;
    TcpPort = 53;
  }

  public static ServerConfiguration DefautConfiguration { get; } = new ServerConfiguration();

  public int TcpPort { get; init; }
  public int UpdPort { get; init; }
}
