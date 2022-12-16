

using ModularDnsServer;
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

Message ParseMessage(byte[] buffer)
{
  var header = ParseHeader(buffer);
  var questions = ParseQuestions(buffer, out int index, header.QuestionsCount);
  var answers = ParseResourceRecords(buffer, ref index, header.AnswersCount);
  var authorities = ParseResourceRecords(buffer, ref index, header.AuthorityCount);
  var additionalRecords = ParseResourceRecords(buffer, ref index, header.AdditionalRecordsCount);

  return new Message(header, questions, answers, authorities, additionalRecords);
}

Question[] ParseQuestions(byte[] buffer, out int index, ushort questionsCount)
{
  var questions = new Question[questionsCount];
  //12 First octal after header
  index = 12;

  for (int i = 0; i < questionsCount; i++)
  {
    string name = ParseLabel(buffer, ref index);
    var qType = (QType)BitReader.ToUInt16(buffer, ++index);
    index += 2;
    var qClass = (QClass)BitReader.ToUInt16(buffer, ++index);
    index += 2;

    questions[i] = new Question(name, qType, qClass);
  }

  return questions;
}

Header ParseHeader(byte[] buffer)
{
  //Id use bytes?
  var id = BitReader.ToUInt16(buffer, 0);
  var qr = (MessageType)(buffer[2] & 0b1000_0000);
  var opcode = (Opcode)((buffer[2] & 0b0111_1000) >> 3);
  var aa = (buffer[2] & 0b0000_0100) > 0;
  var tc = (buffer[2] & 0b0000_0010) > 0;
  var rd = (buffer[2] & 0b0000_0001) > 0;
  var ra = (buffer[3] & 0b1000_0000) > 0;
  var rcode = (ResponseCode)(buffer[3] & 0b0000_1111);
  //var qdCount = (ushort)((buffer[4] << 8) + buffer[5]);
  var qdCount = BitReader.ToUInt16(buffer, 4);
  var ancount = BitReader.ToUInt16(buffer, 6);
  var nsCount = BitReader.ToUInt16(buffer, 8);
  var arCount = BitReader.ToUInt16(buffer, 10);

  return new Header(id, qr, opcode, aa, tc, rd, ra, rcode, qdCount, ancount, nsCount, arCount);
}


ResourceRecord[] ParseResourceRecords(byte[] buffer, ref int index, ushort count)
{
  ResourceRecord[] records = new ResourceRecord[count];
  for (int i = 0; i < count; i++)
  {
    var name = ParseLabel(buffer, ref index);
    index += 2;
    var type = (Type)BitReader.ToUInt16(buffer, index);
    index += 2;
    var @class = (Class)BitReader.ToUInt16(buffer, index);
    index += 2;
    var ttl = BitReader.ToUInt32(buffer, index);
    index += 4;
    var rdlength = BitReader.ToUInt16(buffer, index);
    index += 2;
    var rdata = new byte[rdlength];
    Array.Copy(buffer, index, rdata, 0, rdlength);

    index += rdlength;
    records[i] = new ResourceRecord(name, type, @class, ttl, rdata);
  }

  return records;
}


static string ParseLabel(byte[] buffer, ref int index)
{
  var nameParts = new List<string>();
  while (buffer[index] != 0)
  {
    if ((buffer[index] & 0b1100_0000) == 0b1100_0000)
    {
      int pointerIndex = BitReader.ToUInt16(buffer, index) & 0b0011_1111_1111_1111;
      string pointerValue = ParseLabel(buffer, ref pointerIndex);
      nameParts.Add(pointerValue);
      index += 2;
      break;
    }
    else
    {
      var length = buffer[index++];
      var part = Encoding.ASCII.GetString(buffer, index, length);  //BitConverter.ToString(buffer, index, length);
      nameParts.Add(part);
      index += length;
    }
  }

  return string.Join('.', nameParts);
}

public record struct ResourceRecord(string Name, Type Type, Class Class, uint TimeToLive, byte[] Data);

public record struct Question(string Domain, QType Type, QClass Class);

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
public record struct Message(Header Header, Question[] Questions, ResourceRecord[] Answers, ResourceRecord[] Authorities, ResourceRecord[] AdditionalRecords);





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