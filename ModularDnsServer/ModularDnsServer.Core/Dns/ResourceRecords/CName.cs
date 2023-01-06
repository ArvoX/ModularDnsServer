namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class CName(
  string Name,
  Class Class,
  uint TimeToLive,
  string Domain) : IResourceRecord;


