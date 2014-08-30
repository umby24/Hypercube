using Hypercube.Client;

namespace Hypercube.Network {
    /// <summary>
    /// Interface for Classic packets.
    /// </summary>
    public interface IPacket {
        byte Id { get; }
        void Read(NetworkClient client);
        void Write(NetworkClient client);
        void Handle(NetworkClient client);
    }
}
