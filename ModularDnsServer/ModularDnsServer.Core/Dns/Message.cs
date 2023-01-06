using ModularDnsServer.Core.Dns.ResourceRecords;

namespace ModularDnsServer.Core.Dns;

public record class Message(
  Header Header,
  Question[] Questions,
  IResourceRecord[] Answers,
  IResourceRecord[] Authorities,
  IResourceRecord[] AdditionalRecords);