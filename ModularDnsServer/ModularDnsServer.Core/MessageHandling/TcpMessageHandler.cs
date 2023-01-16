using ModularDnsServer.Core.Binary;
using System.Net.Sockets;

namespace ModularDnsServer.Core.MessageHandling;

public class TcpMessageHandler
{
  private readonly TcpClient Client;
  private readonly CancellationToken CancellationToken;
  private readonly MessageHandler MessageHandler;

  public TcpMessageHandler(TcpClient client, CancellationToken cancellationToken, MessageHandler messageHandler)
  {
    Client = client;
    CancellationToken = cancellationToken;
    MessageHandler = messageHandler;
  }

  public async Task HandleAsync()
  {
    using var _ = Client;
    var stream = Client.GetStream();

    var buffer = new byte[2];

    if (await stream.ReadAsync(buffer, CancellationToken) != buffer.Length)
      throw new Exception();

    var index = 0;
    ushort messageLength = buffer.ToUInt16(ref index);
    if (messageLength != Client.Available)
      throw new Exception();

    buffer = new byte[messageLength];

    if (await stream.ReadAsync(buffer, CancellationToken) != buffer.Length)
      throw new Exception();

    buffer = await MessageHandler.ParseHandleSerialize(buffer);

    await stream.WriteAsync(((ushort)buffer.Length).ToBytes(), CancellationToken);
    await stream.WriteAsync(buffer, CancellationToken);
  }
}