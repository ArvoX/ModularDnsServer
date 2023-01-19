using System.Collections.Concurrent;

namespace ModularDnsServer.Core;

public static class AsyncExtensions
{
  public static async Task<TValue> GetOrAddAsync<TKey, TArg, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TArg, Task<TValue>> func, TArg arg)
    where TKey : notnull
  {
    //Don't invoke func if the value is avalble
    if (dictionary.TryGetValue(key, out var value))
      return value;

    return dictionary.GetOrAdd(key, await func(key, arg));
  }
}