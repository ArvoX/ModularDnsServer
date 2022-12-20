using ModularDnsServer.Core.Binary;
using System.Text;

namespace ModularDnsServer.Core.Dns;

public static class MessageParser
{
  public static Message ParseMessage(byte[] buffer)
  {
    var header = ParseHeader(buffer);
    var questions = ParseQuestions(buffer, out int index, header.QuestionsCount);
    var answers = ParseResourceRecords(buffer, ref index, header.AnswersCount);
    var authorities = ParseResourceRecords(buffer, ref index, header.AuthorityCount);
    var additionalRecords = ParseResourceRecords(buffer, ref index, header.AdditionalRecordsCount);

    return new Message(header, questions, answers, authorities, additionalRecords);
  }

  public static Question[] ParseQuestions(byte[] buffer, out int index, ushort questionsCount)
  {
    var questions = new Question[questionsCount];
    //12 First octal after header
    index = 12;

    for (int i = 0; i < questionsCount; i++)
    {
      string name = ParseLabel(buffer, ref index);
      var qType = (QType)buffer.ToUInt16(index);
      index += 2;
      var qClass = (QClass)buffer.ToUInt16(index);
      index += 2;

      questions[i] = new Question(name, qType, qClass);
    }

    return questions;
  }

  public static Header ParseHeader(byte[] buffer)
  {
    //Id use bytes?
    var id = buffer.ToUInt16(0);
    var qr = (MessageType)((buffer[2] & 0b1000_0000) >> 7);
    var opcode = (Opcode)((buffer[2] & 0b0111_1000) >> 3);
    var aa = (buffer[2] & 0b0000_0100) > 0;
    var tc = (buffer[2] & 0b0000_0010) > 0;
    var rd = (buffer[2] & 0b0000_0001) > 0;
    var ra = (buffer[3] & 0b1000_0000) > 0;
    var rcode = (ResponseCode)(buffer[3] & 0b0000_1111);
    var qdCount = buffer.ToUInt16(4);
    var ancount = buffer.ToUInt16(6);
    var nsCount = buffer.ToUInt16(8);
    var arCount = buffer.ToUInt16(10);

    return new Header(id, qr, opcode, aa, tc, rd, ra, rcode, qdCount, ancount, nsCount, arCount);
  }


  public static ResourceRecord[] ParseResourceRecords(byte[] buffer, ref int index, ushort count)
  {
    ResourceRecord[] records = new ResourceRecord[count];
    for (int i = 0; i < count; i++)
    {
      var name = ParseLabel(buffer, ref index);
      index += 2;
      var type = (Type)buffer.ToUInt16(index);
      index += 2;
      var @class = (Class)buffer.ToUInt16(index);
      index += 2;
      var ttl = buffer.ToUInt32(index);
      index += 4;
      var rdlength = buffer.ToUInt16(index);
      index += 2;
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
        int pointerIndex = buffer.ToUInt16(index) & 0b0011_1111_1111_1111;
        string pointerValue = ParseLabel(buffer, ref pointerIndex);
        nameParts.Add(pointerValue);
        index += 2;
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

    return string.Join('.', nameParts);
  }
}