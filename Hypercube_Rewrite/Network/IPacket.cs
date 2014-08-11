using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hypercube.Network {
    /// <summary>
    /// Interface for Classic packets.
    /// </summary>
    public interface IPacket {
        byte Id { get; }
        void Read(Client.NetworkClient client);
        void Write(Client.NetworkClient client);
        void Handle(Client.NetworkClient client, Hypercube core);
    }
}
