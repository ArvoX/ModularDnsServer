using System.ComponentModel;

namespace ModularDnsServer.Core.Dns;

public enum Opcode : byte
{
  [Description("a standard query")]
  Query = 0,
  [Description("an inverse query")]
  IQuery = 1,
  [Description("a server status request")]
  Status = 2
}
