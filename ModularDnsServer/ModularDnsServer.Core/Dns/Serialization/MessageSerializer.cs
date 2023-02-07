using ModularDnsServer.Core.Binary;
using ModularDnsServer.Core.Dns.ResourceRecords;
using System.Text;

namespace ModularDnsServer.Core.Dns.Serialization;

public static class MessageSerializer
{
  public static byte[] Serialize(Message message)
  {
    var buffer = new List<byte>();
    SerializeHeader(buffer, message.Header, message.Questions.Length, message.Answers.Length, message.Authorities.Length, message.AdditionalRecords.Length);
    SerializeQuestions(buffer, message.Questions);
    SerializeResourceRecords(buffer, message.Answers);
    SerializeResourceRecords(buffer, message.Authorities);
    SerializeResourceRecords(buffer, message.AdditionalRecords);

    return buffer.ToArray();
  }

  #region Header

  private static void SerializeHeader(List<byte> buffer, Header header, int questions, int answers, int authorities, int additionalRecords)
  {
    buffer.AddRange(header.Id.ToBytes());
    //buffer.Add((byte)header.MessageType);
    var flags = (byte)((byte)header.MessageType << 7 +
      (byte)header.Opcode << 3);
    if (header.AuthoritativeAnswer)
      flags += 0b0000_0100;
    if (header.Truncation)
      flags += 0b0000_0010;
    if (header.RecursionDesired)
      flags += 0b0000_0001;
    buffer.Add(flags);
    flags = (byte)header.ResponseCode;
    if (header.RecursionAvailable)
      flags += 0b1000_0000;
    buffer.Add(flags);
    buffer.AddRange(((ushort)questions).ToBytes());
    buffer.AddRange(((ushort)answers).ToBytes());
    buffer.AddRange(((ushort)authorities).ToBytes());
    buffer.AddRange(((ushort)additionalRecords).ToBytes());
  }

  #endregion

  #region Questions

  private static void SerializeQuestions(List<byte> buffer, Question[] questions)
  {
    foreach (var question in questions)
    {
      SerializeQuestion(buffer, question);
    }
  }

  private static void SerializeQuestion(List<byte> buffer, Question question)
  {
    SerializeLabel(buffer, question.Domain);
    buffer.AddRange(((ushort)question.Type).ToBytes());
    buffer.AddRange(((ushort)question.Class).ToBytes());
  }

  #endregion

  #region Records

  private static void SerializeResourceRecords(List<byte> buffer, IResourceRecord[] records)
  {
    foreach (var record in records)
    {
      SerializeResourceRecord(buffer, record);
    }
  }

  private static void SerializeResourceRecord(List<byte> buffer, IResourceRecord record)
  {
    SerializeLabel(buffer, record.Name);
    buffer.AddRange(((ushort)GetType(record)).ToBytes());
    buffer.AddRange(((ushort)record.Class).ToBytes());
    buffer.AddRange(record.TimeToLive.ToBytes());
    var dataSizeIndex = buffer.Count;
    buffer.Add(0);
    buffer.Add(0);
    var dataSize = (ushort)SerializeRecordData(buffer, record);
    var dataSizeBuffer = dataSize.ToBytes();
    buffer[dataSizeIndex] = dataSizeBuffer[0];
    buffer[dataSizeIndex + 1] = dataSizeBuffer[1];

  }

  //TODO: return added
  private static int SerializeRecordData(List<byte> buffer, IResourceRecord record)
  {
    return record switch
    {
      A a => SerializeA(buffer, a),
      NS ns => SerializeLabel(buffer, ns.Domain),
      MD md => SerializeLabel(buffer, md.Domain),
      MF mf => SerializeLabel(buffer, mf.Domain),
      CName cName => SerializeLabel(buffer, cName.Domain),
      SOA soa => SerializeSOA(buffer, soa),
      MB mb => SerializeLabel(buffer, mb.Domain),
      MG mg => SerializeLabel(buffer, mg.Domain),
      MR mr => SerializeLabel(buffer, mr.Domain),
      Null @null => SerializeNull(buffer, @null),
      WKS wks => SerializeWKS(buffer, wks),
      PTR ptr => SerializeLabel(buffer, ptr.Domain),
      HInfo hInfo => SerializeHInfo(buffer, hInfo),
      MInfo mInfo => SerializeMInfo(buffer, mInfo),
      MX mx => SerializeMX(buffer, mx),
      TXT txt => SerializeTXT(buffer, txt),
      _ => throw new Exception(),
    };

    static int SerializeA(List<byte> buffer, A a)
    {
      buffer.AddRange(a.Ip.ToBytes());
      return 4;
    }

    static int SerializeNull(List<byte> buffer, Null @null)
    {
      buffer.AddRange(@null.Bytes);
      return @null.Bytes.Length;
    }
  }

  private static int SerializeTXT(List<byte> buffer, TXT txt)
  {
    int added = 0;
    foreach (var text in txt.Texts)
    {
      var bytes = Encoding.ASCII.GetBytes(text);
      buffer.Add((byte)bytes.Length);
      buffer.AddRange(bytes);
      added += bytes.Length + 1;
    }

    return added;
  }

  private static int SerializeHInfo(List<byte> buffer, HInfo hInfo)
  {
    // 2x length fields
    int added = 2;

    var cpu = Encoding.ASCII.GetBytes(hInfo.Cpu);
    buffer.Add((byte)cpu.Length);
    buffer.AddRange(cpu);
    added += cpu.Length;

    var os = Encoding.ASCII.GetBytes(hInfo.Os);
    buffer.Add((byte)os.Length);
    buffer.AddRange(os);
    added += os.Length;

    return added;
  }

  private static int SerializeWKS(List<byte> buffer, WKS wks)
  {
    buffer.AddRange(wks.Ip.ToBytes());
    buffer.Add(wks.Protocol);
    buffer.AddRange(wks.BitMap);

    return 4 + 1 + wks.BitMap.Length;
  }

  private static int SerializeSOA(List<byte> buffer, SOA soa)
  {
    // 3x TimeSpan (int) + uint
    int bytes = 16;

    bytes += SerializeLabel(buffer, soa.MName);
    bytes += SerializeLabel(buffer, soa.RName);
    buffer.AddRange(soa.Serial.ToBytes());
    buffer.AddRange(soa.Refresh.ToBytes());
    buffer.AddRange(soa.Retry.ToBytes());
    buffer.AddRange(soa.Expire.ToBytes());

    return bytes;
  }

  private static int SerializeMInfo(List<byte> buffer, MInfo mInfo)
  {
    return
      SerializeLabel(buffer, mInfo.RMAILBX) +
      SerializeLabel(buffer, mInfo.EMAILBX);
  }

  private static int SerializeMX(List<byte> buffer, MX mx)
  {
    buffer.AddRange(mx.Preference.ToBytes());
    return 2 + SerializeLabel(buffer, mx.Exchange);
  }

  private static Type GetType(IResourceRecord record)
  {
    return record switch
    {
      A _ => Type.A,
      NS _ => Type.NS,
      MD _ => Type.MD,
      MF _ => Type.MF,
      CName _ => Type.CName,
      SOA _ => Type.SOA,
      MB _ => Type.MB,
      MG _ => Type.MG,
      MR _ => Type.MR,
      Null _ => Type.Null,
      WKS _ => Type.WKS,
      PTR _ => Type.PTR,
      HInfo _ => Type.HInfo,
      MInfo _ => Type.MInfo,
      MX _ => Type.MX,
      TXT _ => Type.TXT,
      _ => throw new Exception()
    };
  }

  #endregion

  private static int SerializeLabel(List<byte> buffer, string domain)
  {
    //The null byte at the end
    int added = 1;
    var domainParts = domain.Split('.');
    foreach (var label in domainParts)
    {
      var bytes = Encoding.ASCII.GetBytes(label);
      buffer.Add((byte)bytes.Length);
      buffer.AddRange(bytes);
      added += bytes.Length + 1;
    }
    buffer.Add(0);
    return added;
  }
}
