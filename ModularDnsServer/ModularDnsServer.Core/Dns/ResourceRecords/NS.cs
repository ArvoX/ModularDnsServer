namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class NS(
  string Name,
  Class Class,
  uint TimeToLive,
  string Domain) : IResourceRecord;


