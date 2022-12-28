namespace ModularDnsServer.Core.Dns;

public  record class Message(
  Header Header,
  Question[] Questions,
  ResourceRecord[] Answers,
  ResourceRecord[] Authorities,
  ResourceRecord[] AdditionalRecords);