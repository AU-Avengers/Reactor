using System.IO;

namespace Reactor.Debugger.AutoJoin.Messages;

internal static class Extensions
{
    public static void Write<T>(this BinaryWriter writer, in T message) where T : struct, IMessage
    {
        writer.Write((byte) message.Type);
        message.Serialize(writer);
    }
}
