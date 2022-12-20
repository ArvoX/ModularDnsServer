using ModularDnsServer.Core;
using ModularDnsServer.Core.Dns;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;

var udpClient = new UdpClient(53);
while (true)
  MessageParser.ParseMessage((await udpClient.ReceiveAsync()).Buffer);
