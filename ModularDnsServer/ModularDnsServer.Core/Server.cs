﻿using ModularDnsServer.Core.Dns.Cache;
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
  private readonly IDnsCache Cache;

  //TODO use factory
  public Server(ServerConfiguration configuration, IDnsCache dnsCache)
  {
    Cache = dnsCache;
    UdpClient = new UdpClient(configuration.UpdPort);
    TcpListener = new TcpListener(configuration.TcpPort);
  }

  public async Task RunAsync(CancellationToken cancellationToken)
  {
    await Cache.Init(cancellationToken);
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
      ListenTcp(tasks, cancellationToken),
      ListenUdp(tasks, cancellationToken),
      Cleanup(tasks, cancellationToken)
    );

    await Task.WhenAll(tasks);
  }

  private Task<Task> ListenTcp(BlockingCollection<Task> tasks, CancellationToken cancellationToken)
  {
    return Task.Factory.StartNew(async () =>
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        var client = await TcpListener.AcceptTcpClientAsync(cancellationToken);
        tasks.Add(Task.Run(new TcpMessageHandler(client, cancellationToken, new MessageHandler(Cache)).HandleAsync, cancellationToken));
      }
    }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
  }

  private Task<Task> ListenUdp(BlockingCollection<Task> tasks, CancellationToken cancellationToken)
  {
    return (Task<Task>)Task.Factory.StartNew(async () =>
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        var received = await UdpClient.ReceiveAsync();
        tasks.Add(Task.Run(new UdpMessageHandler(received, cancellationToken, new MessageHandler(Cache)).HandleAsync, cancellationToken));
      }
    }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
  }

  private static Task<Task> Cleanup(BlockingCollection<Task> tasks, CancellationToken cancellationToken)
  {
    return (Task<Task>)Task.Factory.StartNew(async () =>
      {
        while (!cancellationToken.IsCancellationRequested)
        {
          await tasks.Take(cancellationToken);
        }
    }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
  }
}
