using System.ComponentModel;

namespace ModularDnsServer.Core.Dns;

public enum Type : short
{
  [Description("a host address")]
  A = 1,
  [Description("an authoritative name server")]
  NS = 2,
  [Description("a mail destination")]
  [Obsolete("Use MX")]
  MD = 3,
  [Description("a mail forwarder")]
  [Obsolete("Use MX")]
  MF = 4,
  [Description("the canonical name for an ")]
  CNAME = 5,
  [Description("marks the start of a zone of authority")]
  SOA = 6,
  [Description("a mailbox domain name (EXPERIMENTAL")]
  MB = 7,
  [Description("a mail group member (EXPERIMENTAL)")]
  MG = 8,
  [Description("a mail rename domain name (EXPERIMENTAL)")]
  MR = 9,
  [Description("a null RR (EXPERIMENTAL)")]
  NULL = 10,
  [Description("a well known service description")]
  WKS = 11,
  [Description("a domain name pointer")]
  PTR = 12,
  [Description("host information")]
  HINFO = 13,
  [Description("mailbox or mail list information")]
  MINFO = 14,
  [Description("mail exchange")]
  MX = 15,
  [Description("text strings")]
  TXT = 16,
}
