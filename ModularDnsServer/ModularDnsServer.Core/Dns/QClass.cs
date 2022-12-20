using System.ComponentModel;

namespace ModularDnsServer.Core.Dns;

public enum QClass : short
{
  #region From Class
  [Description("the Internet")]
  IN = Class.IN,
  [Description("the CSNET class")]
  [Obsolete("Used only for examples in some obsolete RFCs")]
  CS = Class.CS,
  [Description("the CHAOS class")]
  CH = Class.CH,
  [Description("Hesiod [Dyer 87]")]
  HS = Class.HS,
  #endregion

  // rfc uses '*'
  [Description("any class")]
  Star = 255,
}