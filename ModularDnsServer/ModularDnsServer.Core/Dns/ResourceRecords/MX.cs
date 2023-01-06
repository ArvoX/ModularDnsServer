namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class MX(
  string Name,
  Class Class,
  uint TimeToLive,
  short Preference,
  string Exchange) : IResourceRecord;


