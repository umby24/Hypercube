using Hypercube_Classic.Client;
using Hypercube_Classic.Core;

namespace Hypercube_Classic.Packets {
    // -- Contains structures for every supported packet.

    public struct Handshake : IPacket {

        public byte Id { get { return 0; } }
        public byte ProtocolVersion { get; set; }
        public string Name { get; set; }
        public string MOTD { get; set; }
        public byte Usertype { get; set; }

        public void Read(NetworkClient Client) {
            ProtocolVersion = Client.wSock.ReadByte();
            Name = Client.wSock.ReadString();
            MOTD = Client.wSock.ReadString();
            Usertype = Client.wSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(ProtocolVersion);
                Client.wSock.WriteString(Name);
                Client.wSock.WriteString(MOTD);
                Client.wSock.WriteByte(Usertype);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            Client.CS.LoginName = Name;
            Client.CS.MPPass = MOTD;

            if (ProtocolVersion != 7) {
                Core.Logger._Log("Info", "Handshake", "Disconnecting client '" + Name + "'. Unsupported protocol verison (" + ProtocolVersion.ToString() + ")");

                var DisconnectPack = new Disconnect();
                DisconnectPack.Reason = "Unsupported protocol version.";
                DisconnectPack.Write(Client);
            }

            if (!Core.ClassicubeHeartbeat.VerifyClientName(Client)) {
                Core.Logger._Log("Info", "Handshake", "Disconnecting client '" + Name + "'. Failed to verify name.");

                var DisconnectPack = new Disconnect();
                DisconnectPack.Reason = "Name verification incorrect.";
                DisconnectPack.Write(Client);
            }

            if (Libraries.Text.StringMatches(Name)) {
                Core.Logger._Log("Info", "Handshake", "Disconnecting Client '" + Name + "'. Invalid characters in name.");

                var DisconnectPack = new Disconnect();
                DisconnectPack.Reason = "Invalid characters in name.";
                DisconnectPack.Write(Client);
            }

            if (Name == "") {
                Core.Logger._Log("Info", "Handshake", "Disconnecting Client '" + Name + "'. Invalid characters in name.");

                var DisconnectPack = new Disconnect();
                DisconnectPack.Reason = "Invalid characters in name.";
                DisconnectPack.Write(Client);
            }

            if (Core.OnlinePlayers > Core.nh.MaxPlayers) {
                var DisconnectPack = new Disconnect();
                DisconnectPack.Reason = "Server is full.";
                DisconnectPack.Write(Client);
            }

            Client.CS.MPPass = MOTD;
            Client.CS.LastActive = System.DateTime.UtcNow;

            if (Usertype == 66) {
                // -- CPE Client
                Core.Logger._Log("Info", "Handshake", "CPE Client Detected.");
                Client.CS.CPE = true;
                CPE.CPEHandshake(Client);
            } else {
                // -- Normal Client.
                Client.CS.CPE = false;
                Client.Login();
            }
        }
    }

    public struct Ping : IPacket {
        public byte Id { get { return 1; } }

        public void Read(NetworkClient Client) {

        }
        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.Purge();
            }
        }
        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct LevelInit : IPacket {
        public byte Id { get { return 2; } }

        public void Read(NetworkClient Client) {

        }
        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.Purge();
            }
        }
        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct LevelChunk : IPacket {
        public byte Id { get { return 3; } }
        public short Length { get; set; }
        public byte[] Data { get; set; }
        public byte Percent { get; set; }

        public void Read(NetworkClient Client) {
            Length = Client.wSock.ReadShort();
            Data = Client.wSock.ReadByteArray();
            Percent = Client.wSock.ReadByte();
        }
        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteShort(Length);
                Client.wSock.WriteByteArray(Data);
                Client.wSock.WriteByte(Percent);
                Client.wSock.Purge();
            }
        }
        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct LevelFinalize : IPacket {
        public byte Id { get { return 4; } }

        public short SizeX { get; set; }
        public short SizeY { get; set; }
        public short SizeZ { get; set; }

        public void Read(NetworkClient Client) {
            SizeX = Client.wSock.ReadShort();
            SizeZ = Client.wSock.ReadShort();
            SizeY = Client.wSock.ReadShort();
        }
        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteShort(SizeX);
                Client.wSock.WriteShort(SizeZ);
                Client.wSock.WriteShort(SizeY);
                Client.wSock.Purge();
            }
        }
        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct SetBlock : IPacket {
        public byte Id { get { return 5; } }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte Mode { get; set; }
        public byte Block { get; set; }

        public void Read(NetworkClient Client) {
            X = Client.wSock.ReadShort();
            Z = Client.wSock.ReadShort();
            Y = Client.wSock.ReadShort();
            Mode = Client.wSock.ReadByte();
            Block = Client.wSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteShort(X);
                Client.wSock.WriteShort(Z);
                Client.wSock.WriteShort(Y);
                Client.wSock.WriteByte(Mode);
                Client.wSock.WriteByte(Block);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            Client.CS.MyEntity.HandleBuildmode(X, Y, Z, Mode, Block);
        }
    }

    public struct SetBlockServer : IPacket {
        public byte Id { get { return 6; } }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte Block { get; set; }

        public void Read(NetworkClient Client) {
            X = Client.wSock.ReadShort();
            Z = Client.wSock.ReadShort();
            Y = Client.wSock.ReadShort();
            Block = Client.wSock.ReadByte();
        }
        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteShort(X);
                Client.wSock.WriteShort(Z);
                Client.wSock.WriteShort(Y);
                Client.wSock.WriteByte(Block);
                Client.wSock.Purge();
            }
        }
        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct SpawnPlayer : IPacket {
        public byte Id { get { return 7; } }
        public sbyte PlayerID { get; set; }
        public string PlayerName { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }

        public void Read(NetworkClient Client) {
            PlayerID = Client.wSock.ReadSByte();
            PlayerName = Client.wSock.ReadString();
            X = Client.wSock.ReadShort();
            Z = Client.wSock.ReadShort();
            Y = Client.wSock.ReadShort();
            Yaw = Client.wSock.ReadByte();
            Pitch = Client.wSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteSByte(PlayerID);
                Client.wSock.WriteString(PlayerName);
                Client.wSock.WriteShort(X);
                Client.wSock.WriteShort(Z);
                Client.wSock.WriteShort(Y);
                Client.wSock.WriteByte(Yaw);
                Client.wSock.WriteByte(Pitch);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct PlayerTeleport : IPacket {
        public byte Id { get { return 8; } }
        public sbyte PlayerID { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte yaw { get; set; }
        public byte pitch { get; set; }

        public void Read(NetworkClient Client) {
            PlayerID = Client.wSock.ReadSByte();
            X = Client.wSock.ReadShort();
            Z = Client.wSock.ReadShort();
            Y = Client.wSock.ReadShort();
            yaw = Client.wSock.ReadByte();
            pitch = Client.wSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteSByte(PlayerID);
                Client.wSock.WriteShort(X);
                Client.wSock.WriteShort(Z);
                Client.wSock.WriteShort(Y);
                Client.wSock.WriteByte(yaw);
                Client.wSock.WriteByte(pitch);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            if (X != Client.CS.MyEntity.X || Y != Client.CS.MyEntity.Y || Z != Client.CS.MyEntity.Z || yaw != Client.CS.MyEntity.Rot || pitch != Client.CS.MyEntity.Look) {
                Client.CS.MyEntity.X = X;
                Client.CS.MyEntity.Y = Y;
                Client.CS.MyEntity.Z = Z;
                Client.CS.MyEntity.Rot = yaw;
                Client.CS.MyEntity.Look = pitch;
                Client.CS.MyEntity.Changed = true;
            }

            if (Client.CS.CPEExtensions.ContainsKey("HeldBlock")) {
                //TODO: Heldblock.
            }
        }
    }

    public struct PosAndOrient : IPacket {
        public byte Id { get { return 9; } }
        public sbyte PlayerID { get; set; }
        public short ChangeX { get; set; }
        public short ChangeY { get; set; }
        public short ChangeZ { get; set; }
        public byte yaw { get; set; }
        public byte pitch { get; set; }

        public void Read(NetworkClient Client) {
            PlayerID = Client.wSock.ReadSByte();
            ChangeX = Client.wSock.ReadShort();
            ChangeZ = Client.wSock.ReadShort();
            ChangeY = Client.wSock.ReadShort();
            yaw = Client.wSock.ReadByte();
            pitch = Client.wSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteSByte(PlayerID);
                Client.wSock.WriteShort(ChangeX);
                Client.wSock.WriteShort(ChangeZ);
                Client.wSock.WriteShort(ChangeY);
                Client.wSock.WriteByte(yaw);
                Client.wSock.WriteByte(pitch);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct PositionUpdate : IPacket {
        public byte Id { get { return 10; } }
        public sbyte PlayerID { get; set; }
        public short ChangeX { get; set; }
        public short ChangeY { get; set; }
        public short ChangeZ { get; set; }

        public void Read(NetworkClient Client) {
            PlayerID = Client.wSock.ReadSByte();
            ChangeX = Client.wSock.ReadShort();
            ChangeZ = Client.wSock.ReadShort();
            ChangeY = Client.wSock.ReadShort();
        }

        public void Write(NetworkClient Client) {

        }
        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct OrientationUpdate : IPacket {
        public byte Id { get { return 11; } }
        public sbyte PlayerID { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }

        public void Read(NetworkClient Client) {
            PlayerID = Client.wSock.ReadSByte();
            Yaw = Client.wSock.ReadByte();
            Pitch = Client.wSock.ReadByte();
        }

        public void Write(NetworkClient Client) {

        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct DespawnPlayer : IPacket {
        public byte Id { get { return 12; } }
        public sbyte PlayerID;

        public void Read(NetworkClient Client) {
            PlayerID = Client.wSock.ReadSByte();
        }

        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteSByte(PlayerID);
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct Message : IPacket {
        public byte Id { get { return 13; } }
        public sbyte PlayerID { get; set; }
        public string Text { get; set; }

        public void Read(NetworkClient Client) {
            PlayerID = Client.wSock.ReadSByte();
            Text = Client.wSock.ReadString();
        }

        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteSByte(PlayerID);
                Client.wSock.WriteString(Text);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            Chat.HandleIncomingChat(Client, Text);
        }
    }

    public struct Disconnect : IPacket {
        public byte Id { get { return 14; } }
        public string Reason { get; set; }

        public void Read(NetworkClient Client) {
            Reason = Client.wSock.ReadString();
        }

        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteString(Reason);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct UpdateRank : IPacket {
        public byte Id { get { return 14; } }
        public byte Rank { get; set; }

        public void Read(NetworkClient Client) {
            Rank = Client.wSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(Rank);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct ExtInfo : IPacket {
        public byte Id { get { return 16; } }
        public string AppName { get; set; }
        public short ExtensionCount { get; set; }

        public void Read(Client.NetworkClient Client) {
            AppName = Client.wSock.ReadString();
            ExtensionCount = Client.wSock.ReadShort();
        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteString(AppName);
                Client.wSock.WriteShort(ExtensionCount);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            Core.Logger._Log("Info", "CPE", "Client " + Client.CS.IP + " is running on " + AppName + ", which supports " + ExtensionCount.ToString() + " extensions.");
            Client.CS.Appname = AppName;
            Client.CS.Extensions = ExtensionCount;
        }
    }

    public struct ExtEntry : IPacket {
        public byte Id { get { return 17; } }
        public string ExtName { get; set; }
        public int Version { get; set; }

        public void Read(Client.NetworkClient Client) {
            ExtName = Client.wSock.ReadString();
            Version = Client.wSock.ReadInt();
        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteString(ExtName);
                Client.wSock.WriteInt(Version);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            Client.CS.CPEExtensions.Add(ExtName, Version);

            if (Client.CS.CPEExtensions.Keys.Count == Client.CS.Extensions)
                CPE.CPEPackets(Client);
        }
    }

    public struct SetClickDistance : IPacket {
        public byte Id { get { return 18; } }
        public short Distance { get; set; }

        public void Read(Client.NetworkClient Client) {
            Distance = Client.wSock.ReadShort();
        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteShort(Distance);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct CustomBlockSupportLevel : IPacket {
        public byte Id { get { return 19; } }
        public byte SupportLevel { get; set; }

        public void Read(Client.NetworkClient Client) {
            SupportLevel = Client.wSock.ReadByte();
        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(SupportLevel);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            Client.CS.CustomBlocksLevel = SupportLevel;
            Client.Login();
        }
    }

    public struct HoldThis : IPacket {
        public byte Id { get { return 20; } }
        public byte BlockToHold { get; set; }
        public byte PreventChange { get; set; }

        public void Read(Client.NetworkClient Client) {
            BlockToHold = Client.wSock.ReadByte();
            PreventChange = Client.wSock.ReadByte();
        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(BlockToHold);
                Client.wSock.WriteByte(PreventChange);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct SetTextHotKey : IPacket {
        public byte Id { get { return 21; } }
        public string Label { get; set; }
        public string Action { get; set; }
        public int KeyCode { get; set; }
        public byte KeyMods { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteString(Label);
                Client.wSock.WriteString(Action);
                Client.wSock.WriteInt(KeyCode);
                Client.wSock.WriteByte(KeyMods);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct ExtAddPlayerName : IPacket {
        public byte Id { get { return 22; } }
        public short NameID { get; set; }
        public string PlayerName { get; set; }
        public string ListName { get; set; }
        public string GroupName { get; set; }
        public byte GroupRank { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteShort(NameID);
                Client.wSock.WriteString(PlayerName);
                Client.wSock.WriteString(ListName);
                Client.wSock.WriteString(GroupName);
                Client.wSock.WriteByte(GroupRank);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct ExtAddEntity : IPacket {
        public byte Id { get { return 23; } }
        public byte EntityID { get; set; }
        public string InGameName { get; set; }
        public string SkinName { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(EntityID);
                Client.wSock.WriteString(InGameName);
                Client.wSock.WriteString(SkinName);
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }
    public struct ExtRemovePlayerName : IPacket {
        public byte Id { get { return 24; } }
        public short NameID { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteShort(NameID);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct EnvSetColor : IPacket {
        public byte Id { get { return 25; } }
        public byte ColorType { get; set; }
        public short Red { get; set; }
        public short Green { get; set; }
        public short Blue { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(ColorType);
                Client.wSock.WriteShort(Red);
                Client.wSock.WriteShort(Green);
                Client.wSock.WriteShort(Blue);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct MakeSelection : IPacket {
        public byte Id { get { return 26; } }
        public byte SelectionID { get; set; }
        public string Label { get; set; }
        public short StartX { get; set; }
        public short StartY { get; set; }
        public short StartZ { get; set; }
        public short EndX { get; set; }
        public short EndY { get; set; }
        public short EndZ { get; set; }
        public short Red { get; set; }
        public short Green { get; set; }
        public short Blue { get; set; }
        public short Opacity { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(SelectionID);
                Client.wSock.WriteString(Label);
                Client.wSock.WriteShort(StartX);
                Client.wSock.WriteShort(StartZ);
                Client.wSock.WriteShort(StartY);
                Client.wSock.WriteShort(EndX);
                Client.wSock.WriteShort(EndZ);
                Client.wSock.WriteShort(EndY);
                Client.wSock.WriteShort(Red);
                Client.wSock.WriteShort(Green);
                Client.wSock.WriteShort(Blue);
                Client.wSock.WriteShort(Opacity);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct RemoveSelection : IPacket {
        public byte Id { get { return 27; } }
        public byte SelectionID { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(SelectionID);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct SetBlockPermissions : IPacket {
        public byte Id { get { return 28; } }
        public byte BlockType { get; set; }
        public byte AllowPlacement { get; set; }
        public byte AllowDeletion { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(BlockType);
                Client.wSock.WriteByte(AllowPlacement);
                Client.wSock.WriteByte(AllowDeletion);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct ChangeModel : IPacket {
        public byte Id { get { return 29; } }
        public byte EntityID { get; set; }
        public string ModelName { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(EntityID);
                Client.wSock.WriteString(ModelName);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }
    public struct EnvSetMapAppearance : IPacket {
        public byte Id { get { return 30; } }
        public string TextureURL { get; set; }
        public byte SideBlock { get; set; }
        public byte EdgeBlock { get; set; }
        public short SideLevel { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteString(TextureURL);
                Client.wSock.WriteByte(SideBlock);
                Client.wSock.WriteByte(EdgeBlock);
                Client.wSock.WriteShort(SideLevel);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }
    public struct EnvSetWeatherType : IPacket {
        public byte Id { get { return 31; } }
        public byte WeatherType { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(WeatherType);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }
    public struct HackControl : IPacket {
        public byte Id { get { return 32; } }
        public byte Flying { get; set; }
        public byte NoClip { get; set; }
        public byte Speeding { get; set; }
        public byte SpawnControl { get; set; }
        public byte ThirdPerson { get; set; }
        public short JumpHeight { get; set; }

        public void Read(Client.NetworkClient Client) {

        }

        public void Write(Client.NetworkClient Client) {
            lock (Client.ServerCore.nh.WriteLock) {
                Client.wSock.WriteByte(Id);
                Client.wSock.WriteByte(Flying);
                Client.wSock.WriteByte(NoClip);
                Client.wSock.WriteByte(Speeding);
                Client.wSock.WriteByte(SpawnControl);
                Client.wSock.WriteByte(ThirdPerson);
                Client.wSock.WriteShort(JumpHeight);
                Client.wSock.Purge();
            }
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }
}