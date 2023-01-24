using ModularDnsServer.Core.Dns;
using ModularDnsServer.Core.Dns.Serialization;
using ModularDnsServer.Core.Dns.ResourceRecords;
using ModularDnsServer.Core.Interface;
using ModularDnsServer.Core.Dns.Cache;

namespace ModularDnsServer.Core.MessageHandling;

public class MessageHandler
{
  private readonly IDnsCache Cache;

  public MessageHandler(IDnsCache cache)
  {
    Cache = cache;
  }

  public async Task<byte[]> ParseHandleSerialize(byte[] bufferIn, CancellationToken cancellationToken)
  {
    var message = MessageParser.ParseMessage(bufferIn);
    var result = await HandleResultAsync(message, bufferIn, cancellationToken);
    var buffer = MessageSerializer.Serialize(result);
    return buffer;
  }


  public async Task<Message> HandleResultAsync(Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken)
  {
    var records = await Cache.GetRecordsAsync(message, orginalData, cancellationToken);
    //if (records.Any())
      return Response(message, records);
  }

  private Message Response(Message message, IResourceRecord[] records)
  {
    return message with
    {
      Header = message.Header with
      {
        MessageType = MessageType.Response
      },
      Answers = records
    };
  }
}

