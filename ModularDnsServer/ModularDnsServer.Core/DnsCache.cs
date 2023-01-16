using ModularDnsServer.Core.Dns;
using ModularDnsServer.Core.Dns.ResourceRecords;
using ModularDnsServer.Core.Interface;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Type = ModularDnsServer.Core.Dns.Type;

namespace ModularDnsServer.Core;

public class DnsCache : IDnsCache
{
  private readonly ConcurrentDictionary<string, DomainCache> Cache = new();


  public IResourceRecord[] GetRecords(Message message)
  {
    if (message.Header.MessageType != MessageType.Query)
      return Array.Empty<IResourceRecord>(); //TODO: Throw or NoResult?

    return message.Questions.SelectMany(GetRecordsFromCache).ToArray();
  }

  private ImmutableList<IResourceRecord> GetRecordsFromCache(Question question)
  {
    if (!Cache.TryGetValue(question.Domain, out DomainCache? cache))
      return ImmutableList<IResourceRecord>.Empty;

    return cache.GetRecords(question.Type);
  }
}

internal class DomainCache
{
  private ConcurrentDictionary<Type, ImmutableList<IResourceRecord>> Cache = new ();

  internal ImmutableList<IResourceRecord> GetRecords(QType type)
  {
    if (!Enum.IsDefined((Type)type))
      return ImmutableList<IResourceRecord>.Empty;

    if (!Cache.TryGetValue((Type)type, out ImmutableList<IResourceRecord>? records))
      return ImmutableList<IResourceRecord>.Empty;

    return records;
  }
}