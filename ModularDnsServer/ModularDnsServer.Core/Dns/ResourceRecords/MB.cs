namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class MB(
  string Name,
  Class Class,
  uint TimeToLive,
  string Domain) : IResourceRecord;


