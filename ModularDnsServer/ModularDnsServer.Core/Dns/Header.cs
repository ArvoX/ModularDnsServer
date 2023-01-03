namespace ModularDnsServer.Core.Dns;

public record class Header(
  ushort Id,
  MessageType MessageType,
  Opcode Opcode,
  bool AuthoritativeAnswer,
  bool Truncation,
  bool RecursionDesired,
  bool RecursionAvailable,
  ResponseCode ResponseCode);