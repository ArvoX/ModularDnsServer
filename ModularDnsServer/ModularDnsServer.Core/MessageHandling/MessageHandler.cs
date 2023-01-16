using ModularDnsServer.Core.Dns;
using ModularDnsServer.Core.Dns.Serialization;
using ModularDnsServer.Core.Dns.ResourceRecords;
using ModularDnsServer.Core.Interface;

namespace ModularDnsServer.Core.MessageHandling;

public class MessageHandler
{
  private readonly DnsCache Cache;
  private readonly List<IActiveResolver> ActiveResolvers;

  public MessageHandler(DnsCache cache, List<IActiveResolver> activeResolvers)
  {
    Cache = cache;
    ActiveResolvers = activeResolvers;
  }

  public async Task<byte[]> ParseHandleSerialize(byte[] bufferIn)
  {
    var message = MessageParser.ParseMessage(bufferIn);
    var result = await HandleResultAsync(message);
    var buffer = MessageSerializer.Serialize(result);
    return buffer;
  }


  public async Task<Message> HandleResultAsync(Message message)
  {
    var records = Cache.GetRecords(message);
    if (records.Any())
      return Response(message, records);

    //TODO async?
    return await CombineAsync(ActiveResolvers.Select(async r => await r.ResolvAsync(message)));

  }

  private async Task<Message> CombineAsync(IEnumerable<Task<Message>> messagesTasks)
  {
    var messages = await Task.WhenAll(messagesTasks);

    throw new NotImplementedException();
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

