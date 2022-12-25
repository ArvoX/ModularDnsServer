using System.Net;
using System.Net.Sockets;

namespace ModularDnsServer.Core
{
  public class Server
  {
    private readonly UdpClient UdpClient;
    private readonly TcpListener TcpListener;

    public Server()
    {
      UdpClient = new UdpClient(53);
      TcpListener = new TcpListener(IPAddress.Any, 53);
    }

    public async Task RunAsync()
    {
      TcpListener.Start();
      //TODO Handle result
      await Task.WhenAny(TcpListener.AcceptTcpClientAsync(), UdpClient.ReceiveAsync());
    }
  }
}