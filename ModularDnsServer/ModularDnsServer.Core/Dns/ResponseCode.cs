using System.ComponentModel;

namespace ModularDnsServer.Core.Dns;

public enum ResponseCode : byte
{
  NoError = 0,
  [Description("The name server was unable to interpret the query.")]
  FormatError = 1,
  [Description("The name server was unable to process this query due to a problem with the name server.")]
  ServerFailure = 2,
  [Description("Meaningful only for responses from an authoritative name server, this code signifies that the domain name referenced in the query does not exist.")]
  NameError = 3,
  [Description("The name server does not support the requested kind of query.")]
  NotImplemented = 4,
  [Description("The name server refuses to perform the specified operation for policy reasons. For example, a name server may not wish to provide the information to the particular requester, or a name server may not wish to perform a particular operation (e.g., zone transfer) for particular data.")]
  Refused = 5
}
