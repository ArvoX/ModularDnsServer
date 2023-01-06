namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class Null(
  string Name,
  Class Class,
  uint TimeToLive,
  byte[] Bytes) : IResourceRecord;


