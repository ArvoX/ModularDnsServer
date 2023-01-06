namespace ModularDnsServer.Core.Dns;


public interface IRecordData { }

public record class ResourceRecord<T>(
  string Domain,
  Type Type,
  Class Class,
  uint TimeToLive,
  T Data) where T: IRecordData;


public record class CName(string Domain) : IRecordData;
public record class HInfo(string Cpu, string Os) : IRecordData;
public record class MB(string Domain) : IRecordData;
[Obsolete]
public record class MD(string Domain) : IRecordData;
[Obsolete]
public record class MF(string Domain) : IRecordData;
public record class MG(string Domain) : IRecordData;
public record class MInfo(string RMAILBX, string EMAILBX) : IRecordData;
public record class MR(string Domain) : IRecordData;
public record class MX(short Preference, string Exchange) : IRecordData;
public record class Null(byte[] Bytes) : IRecordData;
public record class NS(string Domain) : IRecordData;
public record class PTR(string Domain) : IRecordData;
public record class SOA(string MName, string RName, uint Serial, TimeSpan Refresh, TimeSpan Retry, TimeSpan Expire, uint Minimum) : IRecordData;
public record class TXT(params string[] Texts) : IRecordData;

public record class A(uint Ip) : IRecordData;
public record class WKS(uint Ip, byte Protocol, byte[] BitMap) : IRecordData;


