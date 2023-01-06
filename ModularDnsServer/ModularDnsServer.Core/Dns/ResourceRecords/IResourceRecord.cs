namespace ModularDnsServer.Core.Dns.ResourceRecords;


public interface IResourceRecord
{
  string Name { get; }
  Class Class { get; }
  uint TimeToLive { get; }
}


