namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class A(
  string Name,
  Class Class,
  uint TimeToLive,
  uint Ip) : IResourceRecord;


