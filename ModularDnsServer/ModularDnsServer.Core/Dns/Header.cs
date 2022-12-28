namespace ModularDnsServer.Core.Dns;

public record class Header(
  ushort Id,
  MessageType MessageType,
  Opcode Opcode,
  bool AuthoritativeAnswer,
  bool Truncation,
  bool RecursionDesired,
  bool RecursionAvailable,
  ResponseCode ResponseCode,
  //TODO Abstract away (Just create arrays in the correct size)
  ushort QuestionsCount,
  ushort AnswersCount,
  ushort AuthorityCount,
  ushort AdditionalRecordsCount);