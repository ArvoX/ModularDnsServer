using ModularDnsServer.Core;
using ModularDnsServer.Core.Dns;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;

var conf = ServerConfiguration.DefautConfiguration;

await new Server(conf).RunAsync();