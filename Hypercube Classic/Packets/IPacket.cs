using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hypercube_Classic.Packets {
    /// <summary>
    /// Interface for Classic packets.
    /// </summary>
    interface IPacket {
        byte Id { get; }
        void Read(Client.NetworkClient Client);
        void Write(Client.NetworkClient Client);
        void Handle(Client.NetworkClient Client, Hypercube Core);
    }
}
