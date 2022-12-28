namespace ModularDnsServer.Core.Dns;

public record class ResourceRecord(
  string Domain,
  Type Type,
  Class Class,
  uint TimeToLive,
  byte[] Data);
