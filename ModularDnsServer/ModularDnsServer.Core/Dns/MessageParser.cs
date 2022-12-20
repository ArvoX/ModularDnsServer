using ModularDnsServer.Core.Binary;
using System.ComponentModel;
using System.Text;

namespace ModularDnsServer.Core.Dns;

public static class MessageParser
{
  public static Message ParseMessage(byte[] buffer)
  {
    var header = ParseHeader(buffer, out int index);
    var questions = ParseQuestions(buffer, ref index, header.QuestionsCount);
    var answers = ParseResourceRecords(buffer, ref index, header.AnswersCount);
    var authorities = ParseResourceRecords(buffer, ref index, header.AuthorityCount);
    var additionalRecords = ParseResourceRecords(buffer, ref index, header.AdditionalRecordsCount);

    return new Message(header, questions, answers, authorities, additionalRecords);
  }

  public static Header ParseHeader(byte[] buffer, out int index)
  {
    index = 0;
    //Id use bytes?
    var id = buffer.ToUInt16(ref index);
    var qr = (MessageType)((buffer[index] & 0b1000_0000) >> 7);
    var opcode = (Opcode)((buffer[index] & 0b0111_1000) >> 3);
    var aa = (buffer[index] & 0b0000_0100) > 0;
    var tc = (buffer[index] & 0b0000_0010) > 0;
    var rd = (buffer[index] & 0b0000_0001) > 0;
    index++;
    var ra = (buffer[index] & 0b1000_0000) > 0;
    var rcode = (ResponseCode)(buffer[index] & 0b0000_1111);
    index++;
    var qdCount = buffer.ToUInt16(ref index);
    var ancount = buffer.ToUInt16(ref index);
    var nsCount = buffer.ToUInt16(ref index);
    var arCount = buffer.ToUInt16(ref index);

    return new Header(id, qr, opcode, aa, tc, rd, ra, rcode, qdCount, ancount, nsCount, arCount);
  }

  public static Question[] ParseQuestions(byte[] buffer, ref int index, ushort questionsCount)
  {
    var questions = new Question[questionsCount];

    for (int i = 0; i < questionsCount; i++)
    {
      string name = ParseLabel(buffer, ref index);
      var qType = (QType)buffer.ToUInt16(ref index);
      var qClass = (QClass)buffer.ToUInt16(ref index);

      questions[i] = new Question(name, qType, qClass);
    }

    return questions;
  }

  public static ResourceRecord[] ParseResourceRecords(byte[] buffer, ref int index, ushort count)
  {
    ResourceRecord[] records = new ResourceRecord[count];
    for (int i = 0; i < count; i++)
    {
      var name = ParseLabel(buffer, ref index);
      var type = (Type)buffer.ToUInt16(ref index);
      var @class = (Class)buffer.ToUInt16(ref index);
      var ttl = buffer.ToUInt32(ref index);
      var rdlength = buffer.ToUInt16(ref index);
      var rdata = new byte[rdlength];
      Array.Copy(buffer, index, rdata, 0, rdlength);

      index += rdlength;
      records[i] = new ResourceRecord(name, type, @class, ttl, rdata);
    }

    return records;
  }

  public static string ParseLabel(byte[] buffer, ref int index)
  {
    var nameParts = new List<string>();
    while (buffer[index] != 0)
    {
      if ((buffer[index] & 0b1100_0000) == 0b1100_0000)
      {
        int pointerIndex = buffer.ToUInt16(ref index) & 0b0011_1111_1111_1111;
        string pointerValue = ParseLabel(buffer, ref pointerIndex);
        nameParts.Add(pointerValue);
        break;
      }
      else
      {
        var length = buffer[index++];
        var part = Encoding.ASCII.GetString(buffer, index, length);
        nameParts.Add(part);
        index += length;
      }
    }

    index++;
    return string.Join('.', nameParts);
  }
}