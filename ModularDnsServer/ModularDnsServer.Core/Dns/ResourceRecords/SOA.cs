namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class SOA(
  string Name,
  Class Class,
  uint TimeToLive,
  string MName,
  string RName,
  uint Serial,
  TimeSpan Refresh,
  TimeSpan Retry,
  TimeSpan Expire,
  uint Minimum) : IResourceRecord;


