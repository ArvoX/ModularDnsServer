namespace ModularDnsServer.Core.Binary;

public static class BitReader
{
  public static ushort ToUInt16(this IList<byte> buffer, ref int offset)
  {
    return buffer[offset++].ToUInt16(buffer[offset++]);
  }

  public static ushort ToUInt16(this byte first, byte last)
  {
    return (ushort)((first << 8) + last);
  }

  public static uint ToUInt32(this IList<byte> buffer, ref int offset)
  {
    return buffer[offset++].ToUInt32(buffer[offset++], buffer[offset++], buffer[offset++]);
  }

  public static uint ToUInt32(this byte first, byte second, byte third, byte last)
  {
    return (uint)((first << 24) + (second << 16) + (third << 8) + last);
  }
}

public static class BitWriter
{
  public static byte[] ToBytes(this ushort value)
  {
    return new[] { (byte)(value >> 8), (byte)value };
  }
}
