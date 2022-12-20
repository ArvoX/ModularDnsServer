using System.ComponentModel;

namespace ModularDnsServer.Core.Dns;

public enum Class : short
{
  [Description("the Internet")]
  IN = 1,
  [Description("the CSNET class")]
  [Obsolete("Used only for examples in some obsolete RFCs")]
  CS = 2,
  [Description("the CHAOS class")]
  CH = 3,
  [Description("Hesiod [Dyer 87]")]
  HS = 4,
}