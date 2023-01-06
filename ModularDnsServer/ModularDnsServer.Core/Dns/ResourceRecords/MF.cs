namespace ModularDnsServer.Core.Dns.ResourceRecords;

[Obsolete]
public record class MF(
  string Name,
  Class Class,
  uint TimeToLive,
  string Domain) : IResourceRecord;


