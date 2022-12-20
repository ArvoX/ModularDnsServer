namespace ModularDnsServer.Core.Binary;

public static class BitReader
{
  public static ushort ToUInt16(this IList<byte> buffer, int offset)
  {
    return buffer[offset].ToUInt16(buffer[offset + 1]);
  }

  public static ushort ToUInt16(this byte first, byte last)
  {
    return (ushort)((first << 8) + last);
  }

  public static uint ToUInt32(this IList<byte> buffer, int offset)
  {
    return buffer[offset].ToUInt32(buffer[offset + 1], buffer[offset + 2], buffer[offset + 3]);
  }

  public static uint ToUInt32(this byte first, byte second, byte third, byte last)
  {
    return (uint)((first << 24) + (second << 16) + (third << 8) + last);
  }
}
