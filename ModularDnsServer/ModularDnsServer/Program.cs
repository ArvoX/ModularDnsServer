

using ModularDnsServer.Core;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;

var udpClient = new UdpClient(53);
while (true)
  ParseMessage((await udpClient.ReceiveAsync()).Buffer);


/*DnsRequest*/

void ParseMessage(byte[] buffer)
{
  var header = ParseHeader(buffer);
}

Header ParseHeader(byte[] buffer)
{
  //Id use bytes?
  var id = BitConverter.ToUInt16(buffer, 0);
  var qr = (MessageType)(buffer[2] & 0b1000_0000);
  var opcode = (Opcode)((buffer[2] & 0b0111_1000) >> 3);
  var aa = (buffer[2] & 0b0000_0100) > 0;
  var tc = (buffer[2] & 0b0000_0010) > 0;
  var rd = (buffer[2] & 0b0000_0001) > 0;
  var ra = (buffer[3] & 0b1000_0000) > 0;
  var rcode = (ResponseCode)(buffer[2] & 0b0000_1111);
  var QDCOUNT = BitConverter.ToUInt16(buffer, 4);
  var ANCOUNT = BitConverter.ToUInt16(buffer, 6);
  var NSCOUNT = BitConverter.ToUInt16(buffer, 8);
  var ARCOUNT = BitConverter.ToUInt16(buffer, 10);

  return new Header(id, qr, opcode, aa, tc, rd, ra, rcode, QDCOUNT, ANCOUNT, NSCOUNT, ARCOUNT);
}

public enum MessageType : byte { Query = 0, Response = 1 }
public enum Opcode : byte { [Description("a standard query")] QUERY = 0, [Description("  an inverse query")] IQUERY = 1, [Description("a server status request")] STATUS = 2 }
public enum ResponseCode : byte
{
  NoError = 0,
  [Description("The name server was unable to interpret the query.")]
  FormatError = 1,
  [DescriptionAttribute("The name server was unable to process this query due to a problem with the name server.")]
  ServerFailure = 2,
  [Description("Meaningful only for responses from an authoritative name server, this code signifies that the domain name referenced in the query does not exist.")]
  NameError = 3,
  [Description("The name server does not support the requested kind of query.")]
  NotImplemented = 4,
  [Description("The name server refuses to perform the specified operation for policy reasons. For example, a name server may not wish to provide the information to the particular requester, or a name server may not wish to perform a particular operation (e.g., zone transfer) for particular data.")]
  Refused = 5
}


public record struct Header(
  ushort Id,
  MessageType MessageType,
  Opcode Opcode,
  bool AuthoritativeAnswer,
  bool Truncation,
  bool RecursionDesired,
  bool RecursionAvailable,
  ResponseCode ResponseCode,
  ushort QuestionsCount,
  ushort AnswersCount,
  ushort AuthorityCount,
  ushort AdditionalRecordsCount);
public record struct Message(Header Header, Question[] Questions, Answer[] Answers, Authority[] Authorities, AdditionalInfo[] AdditionalInfos);

//void Parse(byte[] buffer)
//{
//  var name = new byte[8];
//  Array.Copy(buffer, name, 8);
//  var type = (Type)BitConverter.ToInt16(buffer, 8);
//  var @class = (Class)BitConverter.ToInt16(buffer, 10);
//  var ttl = BitConverter.ToInt32(buffer, 12);
//  var rdlength = BitConverter.ToInt16(buffer, 16);
//  var rdata = new byte[rdlength];
//  Array.Copy(buffer, 18, rdata, 0, rdlength);
//}




public enum Type : short
{
  [Description("a host address")]
  A = 1,
  [Description("an authoritative name server")]
  NS = 2,
  [Description("a mail destination")]
  [Obsolete("Use MX")]
  MD = 3,
  [Description("a mail forwarder")]
  [Obsolete("Use MX")]
  MF = 4,
  [Description("the canonical name for an ")]
  CNAME = 5,
  [Description("marks the start of a zone of authority")]
  SOA = 6,
  [Description("a mailbox domain name (EXPERIMENTAL")]
  MB = 7,
  [Description("a mail group member (EXPERIMENTAL)")]
  MG = 8,
  [Description("a mail rename domain name (EXPERIMENTAL)")]
  MR = 9,
  [Description("a null RR (EXPERIMENTAL)")]
  NULL = 10,
  [Description("a well known service description")]
  WKS = 11,
  [Description("a domain name pointer")]
  PTR = 12,
  [Description("host information")]
  HINFO = 13,
  [Description("mailbox or mail list information")]
  MINFO = 14,
  [Description("mail exchange")]
  MX = 15,
  [Description("text strings")]
  TXT = 16,
}

public enum QType : short
{
  [Description("A request for a transfer of an entire zone")]
  AXFR = 252,
  [Description("A request for mailbox-related records (MB, MG or MR)")]
  MAILB = 253,
  [Description("A request for mail agent RRs")]
  [Obsolete("See MX")]
  MAILA = 254,
  // rfc uses '*'
  [Description("A request for all records")]
  Star = 255,
}

public enum Class : short
{
  [Description("the Internet")]
  IN = 1,
  [Description("the CSNET class")]
  [Obsolete("Used only for examples in some obsolete RFCs")]
  CS = 2,
  [Description("the CHAOS class")]
  CH = 3,
  [Description("Hesiod [Dyer 87]")]
  HS = 4,
}

public enum QClass : short
{
  // rfc uses '*'
  [Description("any class")]
  Star = 255,
}