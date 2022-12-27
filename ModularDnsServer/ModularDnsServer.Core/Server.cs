using ModularDnsServer.Core.Interface;
using System.Net;
using System.Net.Sockets;

namespace ModularDnsServer.Core
{
  public class Server
  {
    private readonly UdpClient UdpClient;
    private readonly TcpListener TcpListener;
    private readonly List<IPasiveReslover> PasiveResolvers;
    private readonly List<IActiveResolver> ActiveResolvers;
    private readonly IDnsCache Cache;

    //TODO use factory
    public Server(params IResolver[] resolvers)
    {
      UdpClient = new UdpClient(53);
      TcpListener = new TcpListener(IPAddress.Any, 53);
      PasiveResolvers = new List<IPasiveReslover>();
      ActiveResolvers = new List<IActiveResolver>();
      foreach (var resolver in resolvers)
      {
        if (resolver is IPasiveReslover pasiveReslover)
          PasiveResolvers.Add(pasiveReslover);
        else if (resolver is IActiveResolver activeResolver)
          ActiveResolvers.Add(activeResolver);
      }
    }

    public async Task RunAsync()
    {
      await Task.WhenAll(PasiveResolvers.Select(async r => await r.InitCacheAsync(Cache)));

      TcpListener.Start();
      //TODO Handle result
      await Task.WhenAny(TcpListener.AcceptTcpClientAsync(), UdpClient.ReceiveAsync());
    }
  }
}