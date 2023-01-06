using System.ComponentModel;

namespace ModularDnsServer.Core.Dns;

public enum QType : short
{
  #region From Type
  [Description("a host address")]
  A = Type.A,
  [Description("an authoritative name server")]
  NS = Type.NS,
  [Description("a mail destination")]
  [Obsolete("Use MX")]
  MD = Type.MD,
  [Description("a mail forwarder")]
  [Obsolete("Use MX")]
  MF = Type.MF,
  [Description("the canonical name for an ")]
  CName = Type.CName,
  [Description("marks the start of a zone of authority")]
  SOA = Type.SOA,
  [Description("a mailbox domain name (EXPERIMENTAL")]
  MB = Type.MB,
  [Description("a mail group member (EXPERIMENTAL)")]
  MG = Type.MG,
  [Description("a mail rename domain name (EXPERIMENTAL)")]
  MR = Type.MR,
  [Description("a null RR (EXPERIMENTAL)")]
  Null = Type.Null,
  [Description("a well known service description")]
  WKS = Type.WKS,
  [Description("a domain name pointer")]
  PTR = Type.PTR,
  [Description("host information")]
  HInfo = Type.HInfo,
  [Description("mailbox or mail list information")]
  MInfo = Type.MInfo,
  [Description("mail exchange")]
  MX = Type.MX,
  [Description("text strings")]
  TXT = Type.TXT,
  #endregion

  [Description("A request for a transfer of an entire zone")]
  AXFR = 252,
  [Description("A request for mailbox-related records (MB, MG or MR)")]
  MAILB = 253,
  [Description("A request for mail agent RRs")]
  [Obsolete("See MX")]
  MAILA = 254,
  // rfc uses '*'
  [Description("A request for all records")]
  Star = 255,
}