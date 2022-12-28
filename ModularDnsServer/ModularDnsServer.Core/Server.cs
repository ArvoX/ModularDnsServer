using ModularDnsServer.Core.Dns;
using ModularDnsServer.Core.Interface;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;

namespace ModularDnsServer.Core;



public class Server
{
  private readonly UdpClient UdpClient;
  private readonly TcpListener TcpListener;
  private readonly List<IPasiveReslover> PasiveResolvers;
  private readonly List<IActiveResolver> ActiveResolvers;
  private readonly DnsCache Cache;

  //TODO use factory
  public Server(ServerConfiguration configuration, params IResolver[] resolvers)
  {
    UdpClient = new UdpClient(configuration.UpdPort);
    TcpListener = new TcpListener(IPAddress.Any, configuration.TcpPort);

    Cache= new DnsCache();

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

  //public Server(params IResolver[] resolvers) : this (ServerConfiguration.Default...

  //public Server(params Func<ResolverConfiguration, IResolver>[] factories)

  public async Task RunAsync()
  {
    await Task.WhenAll(PasiveResolvers.Select(async r => await r.InitCacheAsync(Cache)));

    TcpListener.Start();
    //TODO Handle result
    await Task.WhenAny(TcpListener.AcceptTcpClientAsync(), UdpClient.ReceiveAsync());
  }

  public async Task<Message> HandleResultAsync(Message message)
  {
    var records = Cache.GetRecords(message);
    if(records.Any()) 
      return Response(message, records);

    //TODO async?
    return Combine(ActiveResolvers.Select(r => r.Resolv(message)));

  }

  private Message Combine(IEnumerable<Message> messages)
  {
    throw new NotImplementedException();
  }

  private Message Response(Message message, ResourceRecord[] records)
  {
    return message with
    {
      Header = message.Header with
      {
        MessageType = MessageType.Response,
        AnswersCount = (ushort)records.Length
      },
      Answers = records
    };
  }
}