namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class TXT(
  string Name,
  Class Class,
  uint TimeToLive,
  params string[] Texts) : IResourceRecord;


