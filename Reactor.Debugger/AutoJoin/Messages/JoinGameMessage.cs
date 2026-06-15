using System.IO;
using InnerNet;

namespace Reactor.Debugger.AutoJoin.Messages;

internal readonly struct JoinGameMessage : IMessage
{
    public JoinGameMessage(string address, ushort port, int gameCode)
    {
        Address = address;
        Port = port;
        GameCode = gameCode;
    }

    public string Address { get; }

    public ushort Port { get; }

    public int GameCode { get; }

    public MessageType Type => MessageType.JoinGame;

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Address);
        writer.Write(Port);
        writer.Write(GameCode);
    }

    public static JoinGameMessage Deserialize(BinaryReader reader)
    {
        return new JoinGameMessage(reader.ReadString(), reader.ReadUInt16(), reader.ReadInt32());
    }

    public static JoinGameMessage From(InnerNetClient innerNetClient)
    {
        return new JoinGameMessage(innerNetClient.networkAddress, (ushort) innerNetClient.networkPort, innerNetClient.GameId);
    }

    public void Deconstruct(out string address, out ushort port, out int gameCode)
    {
        address = Address;
        port = Port;
        gameCode = GameCode;
    }
}
