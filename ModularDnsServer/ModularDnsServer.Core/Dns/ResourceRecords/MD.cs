namespace ModularDnsServer.Core.Dns.ResourceRecords;

[Obsolete]
public record class MD(
  string Name,
  Class Class,
  uint TimeToLive,
  string Domain) : IResourceRecord;


