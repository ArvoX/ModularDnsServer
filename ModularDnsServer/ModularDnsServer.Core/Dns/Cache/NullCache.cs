using ModularDnsServer.Core.Dns.ResourceRecords;
using ModularDnsServer.Core.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularDnsServer.Core.Dns.Cache
{
  public class NullCache : IDnsCache
  {
    private readonly ImmutableList<IActiveResolver> ActiveResolvers;
    private readonly PasiveResolver PasiveResolverInstans;

    public NullCache(params IActiveResolver[] activeResolvers)
      : this(activeResolvers, Enumerable.Empty<IPasiveResolver>())
    { }

    public NullCache(params IPasiveResolver[] pasiveResolvers)
      : this(Enumerable.Empty<IActiveResolver>(), pasiveResolvers)
    { }

    public NullCache(IEnumerable<IActiveResolver> activeResolvers, IEnumerable<IPasiveResolver> pasiveResolvers)
    {
      PasiveResolverInstans = new(pasiveResolvers);
      ActiveResolvers = activeResolvers.ToImmutableList().Add(PasiveResolverInstans);
    }

    public async Task<IResourceRecord[]> GetRecordsAsync(Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken)
    {
      var messages = await Task.WhenAll(ActiveResolvers.Select(r => r.ResolvAsync(message, orginalData, cancellationToken)));

      return messages.SelectMany(m => m.Answers).ToArray();
    }

    public async Task Init(CancellationToken cancellationToken)
    {
      await PasiveResolverInstans.Init(cancellationToken);
    }

    private class PasiveResolver : IActiveResolver, IDnsCache
    {
      private readonly ImmutableList<IPasiveResolver> PasiveResolvers;
      private readonly Dictionary<string, List<IResourceRecord>> Records;

      public PasiveResolver(IEnumerable<IPasiveResolver> pasiveResolvers)
      {
        PasiveResolvers = pasiveResolvers.ToImmutableList();
        Records = new();
      }

      public Task<IResourceRecord[]> GetRecordsAsync(Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken)
      {
        return Task.FromResult(Records[message.Questions[0].Domain].ToArray());
      }

      public async Task Init(CancellationToken cancellationToken)
      {
        await Task.WhenAll(PasiveResolvers.Select(async r => await r.InitCacheAsync(this, cancellationToken)));
      }

      public Task<Message> ResolvAsync(Message message, ReadOnlyMemory<byte> orginalData, CancellationToken cancellationToken)
      {
        return Task.FromResult(message with
        {
          Header = message.Header with
          {
            MessageType = MessageType.Response
          },
          Answers = Records[message.Questions[0].Domain].ToArray()
        });
      }
    }
  }
}
