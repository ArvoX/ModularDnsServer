namespace ModularDnsServer.Core.Dns.ResourceRecords;

public record class MInfo(
  string Name,
  Class Class,
  uint TimeToLive,
  string RMAILBX,
  string EMAILBX) : IResourceRecord;


