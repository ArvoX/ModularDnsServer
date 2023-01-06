namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class WKS(
  string Name,
  Class Class,
  uint TimeToLive,
  uint Ip,
  byte Protocol,
  byte[] BitMap) : IResourceRecord;


