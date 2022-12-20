using ModularDnsServer.Core;
using ModularDnsServer.Core.Dns;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;

var udpClient = new UdpClient(53);
while (true)
{
  var message = MessageParser.ParseMessage((await udpClient.ReceiveAsync()).Buffer);
  Console.WriteLine($"Incomming {message.Header.MessageType}: {message.Questions[0]}");
}