using Microsoft.Extensions.Hosting;
using ModularDnsServer.Core;
using ModularDnsServer.Core.Dns.Cache;
using ModularDnsServer.Resolvers.StaticResolvers;
using System.Net;

var builder = Host.CreateDefaultBuilder(args);
using var host = builder.Build();
var conf = new ServerConfiguration()
{
  UpdPort = IPEndPoint.Parse("127.0.0.5:53")
};
var cache = new NullCache(new ConfigurableUpstreamResolver(IPEndPoint.Parse("8.8.8.8:53")));

await new Server(conf, cache).RunAsync(CancellationToken.None);
await host.RunAsync();