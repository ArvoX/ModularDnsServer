namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class HInfo(
  string Name,
  Class Class,
  uint TimeToLive,
  string Cpu,
  string Os) : IResourceRecord;


