using ModularDnsServer.Core.Dns;
using ModularDnsServer.Core.Interface;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Type = ModularDnsServer.Core.Dns.Type;

namespace ModularDnsServer.Core;

internal class DnsCache : IDnsCache
{
  private ConcurrentDictionary<string, DomainCache> Cache = new();


  public ResourceRecord[] GetRecords(Message message)
  {
    if (message.Header.MessageType != MessageType.Query)
      return Array.Empty<ResourceRecord>(); //TODO: Throw or NoResult?

    return message.Questions.SelectMany(GetRecordsFromCache).ToArray();
  }

  private ImmutableList<ResourceRecord> GetRecordsFromCache(Question question)
  {
    if (!Cache.TryGetValue(question.Domain, out DomainCache? cache))
      return ImmutableList<ResourceRecord>.Empty;

    return cache.GetRecords(question.Type);
  }
}

internal class DomainCache
{
  private ConcurrentDictionary<Type, ImmutableList<ResourceRecord>> Cache = new();

  internal ImmutableList<ResourceRecord> GetRecords(QType type)
  {
    if (!Enum.IsDefined((Type)type))
      return ImmutableList<ResourceRecord>.Empty;

    if (!Cache.TryGetValue((Type)type, out ImmutableList<ResourceRecord>? records))
      return ImmutableList<ResourceRecord>.Empty;

    return records;
  }
}