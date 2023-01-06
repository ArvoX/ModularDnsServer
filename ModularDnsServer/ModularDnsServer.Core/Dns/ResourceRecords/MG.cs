namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class MG(
  string Name,
  Class Class,
  uint TimeToLive,
  string Domain) : IResourceRecord;


