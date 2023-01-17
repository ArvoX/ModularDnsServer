using ModularDnsServer.Core.Dns;
using ModularDnsServer.Core.Dns.ResourceRecords;
using ModularDnsServer.Core.Interface;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Type = ModularDnsServer.Core.Dns.Type;

namespace ModularDnsServer.Core;

public class DnsCache : IDnsCache
{
  private readonly ConcurrentDictionary<string, DomainCache> Cache = new();
  private readonly List<IPasiveReslover> PasiveResolvers;
  private readonly List<IActiveResolver> ActiveResolvers;

  public DnsCache(IResolver[] resolvers)
  {
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

  public IResourceRecord[] GetRecords(Message message)
  {
    if (message.Header.MessageType != MessageType.Query)
      return Array.Empty<IResourceRecord>(); //TODO: Throw or NoResult?

    var records = new List<IResourceRecord>();
    foreach (var question in message.Questions)
    {
      var cache = Cache.GetOrAdd(question.Domain, CacheInit);
      records.AddRange(cache.GetRecords(question.Type, message));
    }

    return records.ToArray();
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

  internal ImmutableList<IResourceRecord> GetRecords(QType type, Message message)
  {
    if (!Enum.IsDefined((Type)type))
      return ImmutableList<IResourceRecord>.Empty;

    var records = Cache.GetOrAdd((Type)type, ResolveAsync, message);

    return records;
  }

  private async Task<ImmutableList<IResourceRecord>> ResolveAsync(Type type, Message message)
  {
    var results = await Task.WhenAll(ActiveResolvers.Select(async ar => await ar.ResolvAsync(message)));

    return results;
  }
}

public static class Foo
{
  public static async Task<TValue> GetOrAddAsync<TKey, TArg, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TArg, Task<TValue>> func, TArg arg)
    where TKey : notnull
  {
    if (dictionary.TryGetValue(key, out var value))
      return value;

    return dictionary.GetOrAdd(key, await func(key, arg));
  }
}