using System.Net;
using System.Net.Sockets;

namespace ModularDnsServer.Core.MessageHandling;

public class UdpMessageHandler
{
  private readonly byte[] Buffer;
  private readonly IPEndPoint Client;
  private readonly MessageHandler MessageHandler;
  private readonly CancellationToken CancellationToken;

  public UdpMessageHandler(UdpReceiveResult received, CancellationToken cancellationToken, MessageHandler messageHandler)
  {
    Buffer = received.Buffer;
    Client = received.RemoteEndPoint;
    MessageHandler = messageHandler;
    CancellationToken = cancellationToken;
  }

  public async Task HandleAsync()
  {
    if (Buffer.Length > 512)
      throw new Exception();

    byte[] buffer = await MessageHandler.ParseHandleSerialize(Buffer, CancellationToken);

    using var client = new UdpClient();
    client.Connect(Client);
    await client.SendAsync(buffer, CancellationToken);
  }
}
