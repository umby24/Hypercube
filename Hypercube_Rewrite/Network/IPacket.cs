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
        void Read(Client.NetworkClient Client);
        void Write(Client.NetworkClient Client);
        void Handle(Client.NetworkClient Client, Hypercube Core);
    }
}
