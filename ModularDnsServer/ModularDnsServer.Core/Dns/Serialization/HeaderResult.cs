namespace ModularDnsServer.Core.Dns.Serialization;

public record struct HeaderResult(
  Header Header,
  ushort Questions,
  ushort Answers,
  ushort Authorities,
  ushort AdditionalRecords);