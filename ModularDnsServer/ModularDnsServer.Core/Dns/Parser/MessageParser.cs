using ModularDnsServer.Core.Binary;
using ModularDnsServer.Core.Dns.ResourceRecords;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;

namespace ModularDnsServer.Core.Dns.Parser;

public static class MessageParser
{
  public static Message ParseMessage(byte[] buffer)
  {
    var headerResult = ParseHeader(buffer, out int index);
    var questions = ParseQuestions(buffer, ref index, headerResult.Questions);
    var answers = ParseResourceRecords(buffer, ref index, headerResult.Answers);
    var authorities = ParseResourceRecords(buffer, ref index, headerResult.Authorities);
    var additionalRecords = ParseResourceRecords(buffer, ref index, headerResult.AdditionalRecords);

    return new Message(headerResult.Header, questions, answers, authorities, additionalRecords);
  }

  public static HeaderResult ParseHeader(byte[] buffer, out int index)
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

    return new HeaderResult(new Header(id, qr, opcode, aa, tc, rd, ra, rcode), qdCount, ancount, nsCount, arCount);
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

  public static IResourceRecord[] ParseResourceRecords(byte[] buffer, ref int index, ushort count)
  {
    IResourceRecord[] records = new IResourceRecord[count];
    for (int i = 0; i < count; i++)
    {
      var name = ParseLabel(buffer, ref index);
      var type = (Type)buffer.ToUInt16(ref index);
      var @class = (Class)buffer.ToUInt16(ref index);
      var ttl = buffer.ToUInt32(ref index);
      var rdlength = buffer.ToUInt16(ref index);
      records[i] = type switch
      {
        Type.A => new A(name, @class, ttl, buffer.ToUInt32(ref index)),
        Type.NS => new NS(name, @class, ttl, ParseLabel(buffer, ref index)),
        Type.MD => new MD(name, @class, ttl, ParseLabel(buffer, ref index)),
        Type.MF => new MF(name, @class, ttl, ParseLabel(buffer, ref index)),
        Type.CName => new CName(name, @class, ttl, ParseLabel(buffer, ref index)),
        Type.SOA => new SOA(
          name,
          @class,
          ttl,
          ParseLabel(buffer, ref index),
          ParseLabel(buffer, ref index),
          buffer.ToUInt32(ref index),
          TimeSpan.FromSeconds(buffer.ToInt32(ref index)),
          TimeSpan.FromSeconds(buffer.ToInt32(ref index)),
          TimeSpan.FromSeconds(buffer.ToInt32(ref index)),
          buffer.ToUInt32(ref index)),
        Type.MB => new MB(name, @class, ttl, ParseLabel(buffer, ref index)),
        Type.MG => new MG(name, @class, ttl, ParseLabel(buffer, ref index)),
        Type.MR => new MR(name, @class, ttl, ParseLabel(buffer, ref index)),
        Type.Null => new Null(name, @class, ttl, buffer.ToArray(ref index, rdlength)),
        Type.WKS => new WKS(name, @class, ttl, buffer.ToUInt32(ref index), buffer[index++], buffer.ToArray(ref index, rdlength - 3)),
        Type.PTR => new PTR(name, @class, ttl, ParseLabel(buffer, ref index)),
        Type.HInfo => new HInfo(name, @class, ttl, buffer.ToString(ref index), buffer.ToString(ref index)),
        Type.MInfo => new MInfo(name, @class, ttl, buffer.ToString(ref index), buffer.ToString(ref index)),
        Type.MX => new MX(name, @class, ttl, buffer.ToInt16(ref index), buffer.ToString(ref index)),
        Type.TXT => new TXT(name, @class, ttl, buffer.ToStrings(ref index, rdlength)),
        _ => throw new ArgumentException()
      };

      index += rdlength;
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
