namespace ModularDnsServer.Core.Dns;

public record struct HeaderResult(
  Header Header,
  ushort Questions,
  ushort Answers,
  ushort Authorities,
  ushort AdditionalRecords);