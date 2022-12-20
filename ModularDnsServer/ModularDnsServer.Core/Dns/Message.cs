namespace ModularDnsServer.Core.Dns;

public record struct Message(
  Header Header,
  Question[] Questions,
  ResourceRecord[] Answers,
  ResourceRecord[] Authorities,
  ResourceRecord[] AdditionalRecords);