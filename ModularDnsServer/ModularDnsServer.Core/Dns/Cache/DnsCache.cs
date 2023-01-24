using ModularDnsServer.Core.Dns;
using ModularDnsServer.Core.Dns.ResourceRecords;
using ModularDnsServer.Core.Interface;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Type = ModularDnsServer.Core.Dns.Type;

namespace ModularDnsServer.Core.Dns.Cache;

public class DnsCache : IDnsCache
{
  private readonly ConcurrentDictionary<string, DomainCache> Cache = new();
  private readonly List<IPasiveResolver> PasiveResolvers;
  private readonly List<IActiveResolver> ActiveResolvers;

  public DnsCache(IResolver[] resolvers)
  {
    PasiveResolvers = new List<IPasiveResolver>();
    ActiveResolvers = new List<IActiveResolver>();
    foreach (var resolver in resolvers)
    {
      if (resolver is IPasiveResolver pasiveResolver)
        PasiveResolvers.Add(pasiveResolver);
      else if (resolver is IActiveResolver activeResolver)
        ActiveResolvers.Add(activeResolver);
    }
  }

  public async Task<IResourceRecord[]> GetRecordsAsync(Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken)
  {
    if (message.Header.MessageType != MessageType.Query)
      return Array.Empty<IResourceRecord>(); //TODO: Throw or NoResult?

    var records = new List<IResourceRecord>();
    foreach (var question in message.Questions)
    {
      var cache = Cache.GetOrAdd(question.Domain, CacheInit);
      records.AddRange(await cache.GetRecordsAsync(question.Type, (message, orginalData, cancellationToken)));
    }

    return records.ToArray();
  }

  public async Task Init(CancellationToken cancellationToken)
  {
    await Task.WhenAll(PasiveResolvers.Select(resolver => resolver.InitCacheAsync(this, cancellationToken)));
  }

  private DomainCache CacheInit(string domain)
  {
    return new DomainCache(ActiveResolvers.AsReadOnly());
  }
}

internal class DomainCache
{
  private readonly ConcurrentDictionary<Type, ImmutableList<IResourceRecord>> Cache = new();
  private readonly ReadOnlyCollection<IActiveResolver> ActiveResolvers;

  public DomainCache(ReadOnlyCollection<IActiveResolver> activeResolvers)
  {
    ActiveResolvers = activeResolvers;
  }

  internal async Task<ImmutableList<IResourceRecord>> GetRecordsAsync(QType type, (Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken) data)
  {
    if (!Enum.IsDefined((Type)type))
      return ImmutableList<IResourceRecord>.Empty;

    var records = await Cache.GetOrAddAsync((Type)type, ResolveAsync, data);

    return records;
  }

  private async Task<ImmutableList<IResourceRecord>> ResolveAsync(Type type, (Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken) data)
  {
    (Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken) = data;
    var results = await Task.WhenAll(ActiveResolvers.Select(async activeResolver => await activeResolver.ResolvAsync(message, orginalData, cancellationToken)));

    return results.SelectMany(message => message.Answers.Concat(message.Authorities).Concat(message.AdditionalRecords)).ToImmutableList();
  }
}
