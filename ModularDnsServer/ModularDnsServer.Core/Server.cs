using ModularDnsServer.Core.Interface;
using ModularDnsServer.Core.MessageHandling;
using System.Collections.Concurrent;
using System.Net;
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

    Cache = new DnsCache();

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

  public async Task RunAsync(CancellationToken cancellationToken)
  {
    await Task.WhenAll(PasiveResolvers.Select(async r => await r.InitCacheAsync(Cache)));
    if (cancellationToken.IsCancellationRequested)
      return;

    TcpListener.Start();
    //TODO Handle result
    //Messages sent over TCP connections use server port 53 (decimal).  The
    //message is prefixed with a two byte length field which gives the message
    //length, excluding the two byte length field.  This length field allows
    //the low-level processing to assemble a complete message before beginning
    //to parse it.
    BlockingCollection<Task> tasks = new();

    await Task.WhenAll(
    Task.Factory.StartNew(async () =>
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        var client = await TcpListener.AcceptTcpClientAsync(cancellationToken);
        tasks.Add(Task.Run(new TcpMessageHandler(client, cancellationToken, new MessageHandler(Cache, ActiveResolvers)).HandleAsync, cancellationToken));
      }
    }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current),
    Task.Factory.StartNew(async () =>
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        var received = await UdpClient.ReceiveAsync();
        tasks.Add(Task.Run(new UdpMessageHandler(received, cancellationToken, new MessageHandler(Cache, ActiveResolvers)).HandleAsync, cancellationToken));
      }
    }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current),
    Task.Factory.StartNew(async () =>
      {
        while (!cancellationToken.IsCancellationRequested)
        {
          await tasks.Take(cancellationToken);
        }
      }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current)
    );

    await Task.WhenAll(tasks);
  }
}
