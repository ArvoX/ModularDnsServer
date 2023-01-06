namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class MR(
  string Name,
  Class Class,
  uint TimeToLive,
  string Domain) : IResourceRecord;


