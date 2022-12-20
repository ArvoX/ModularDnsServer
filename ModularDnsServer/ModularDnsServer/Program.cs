using ModularDnsServer.Core;
using ModularDnsServer.Core.Binary;
using ModularDnsServer.Core.Dns;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using Type = ModularDnsServer.Core.Dns.Type;

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
