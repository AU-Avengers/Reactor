using System.IO;

namespace Reactor.Debugger.AutoJoin.Messages;

internal readonly struct RequestJoinGameMessage : IMessage
{
    public MessageType Type => MessageType.RequestJoinGame;

    public void Serialize(BinaryWriter writer)
    {
    }

    public static RequestJoinGameMessage Deserialize(BinaryReader reader)
    {
        return default;
    }
}
