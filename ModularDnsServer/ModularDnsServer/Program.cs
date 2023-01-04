using ModularDnsServer.Core;
using ModularDnsServer.Core.Dns;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;

var conf = new ServerConfiguration();

await new Server(conf).RunAsync(CancellationToken.None);