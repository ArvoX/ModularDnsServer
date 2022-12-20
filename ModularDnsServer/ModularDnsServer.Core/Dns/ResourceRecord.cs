namespace ModularDnsServer.Core.Dns;

public record struct ResourceRecord(
  string Name,
  Type Type,
  Class Class,
  uint TimeToLive,
  byte[] Data);
