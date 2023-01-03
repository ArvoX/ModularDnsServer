namespace ModularDnsServer.Core.Dns.Parser;

public record struct HeaderResult(
  Header Header,
  ushort Questions,
  ushort Answers,
  ushort Authorities,
  ushort AdditionalRecords);