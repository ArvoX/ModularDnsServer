namespace ModularDnsServer.Core.Dns;
public record struct Question(
  string Domain,
  QType Type,
  QClass Class);
