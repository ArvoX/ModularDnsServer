namespace ModularDnsServer.Core.Dns;

public record struct Header(
  ushort Id,
  MessageType MessageType,
  Opcode Opcode,
  bool AuthoritativeAnswer,
  bool Truncation,
  bool RecursionDesired,
  bool RecursionAvailable,
  ResponseCode ResponseCode,
  ushort QuestionsCount,
  ushort AnswersCount,
  ushort AuthorityCount,
  ushort AdditionalRecordsCount);