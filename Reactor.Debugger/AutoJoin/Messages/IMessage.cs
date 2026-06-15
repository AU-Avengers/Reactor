using System.IO;

namespace Reactor.Debugger.AutoJoin.Messages;

internal interface IMessage
{
    MessageType Type { get; }

    void Serialize(BinaryWriter writer);
}
