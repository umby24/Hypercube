using Hypercube.Client;
using Hypercube.Core;

namespace Hypercube.Network {
    // -- Contains structures for every supported packet.

    public struct Handshake : IPacket {

        public byte Id { get { return 0; } }
        public byte ProtocolVersion { get; set; }
        public string Name { get; set; }
        public string MOTD { get; set; }
        public byte Usertype { get; set; }

        public void Read(NetworkClient Client) {
            ProtocolVersion = Client.WSock.ReadByte();
            Name = Client.WSock.ReadString();
            MOTD = Client.WSock.ReadString();
            Usertype = Client.WSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(ProtocolVersion);
                Client.WSock.WriteString(Name);
                Client.WSock.WriteString(MOTD);
                Client.WSock.WriteByte(Usertype);
                Client.WSock.Purge();
            
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            Client.CS.LoginName = Name;
            Client.CS.MpPass = MOTD;

            if (ProtocolVersion != 7) {
                Core.Logger.Log("Handshake", "Disconnecting client '" + Name + "'. Unsupported protocol verison (" + ProtocolVersion.ToString() + ")", LogType.Info);
                Client.KickPlayer("Unsupported protocol version.");
            }

            if (!Core.HB.VerifyClientName(Client)) {
                Core.Logger.Log("Handshake", "Disconnecting client '" + Name + "'. Failed to verify name.", LogType.Info);
                Client.KickPlayer("Name verification incorrect.");
            }

            if (Libraries.Text.StringMatches(Name)) {
                Core.Logger.Log("Handshake", "Disconnecting Client '" + Name + "'. Invalid characters in name.", LogType.Info);
                Client.KickPlayer("Invalid characters in name.");
            }

            if (Name == "") {
                Core.Logger.Log("Handshake", "Disconnecting Client '" + Name + "'. Invalid name.", LogType.Info);
                Client.KickPlayer("Invalid name.");
            }

            if (Core.OnlinePlayers > Core.nh.MaxPlayers) 
                Client.KickPlayer("The server is full.");

            Client.CS.MpPass = MOTD;

            if (Usertype == 66) {
                // -- CPE Client
                Core.Logger.Log("Handshake", "CPE Client Detected.", LogType.Info);
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
                Client.WSock.WriteByte(Id);
                Client.WSock.Purge();
            
        }
        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct LevelInit : IPacket {
        public byte Id { get { return 2; } }

        public void Read(NetworkClient Client) {

        }
        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.Purge();
            
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
            Length = Client.WSock.ReadShort();
            Data = Client.WSock.ReadByteArray();
            Percent = Client.WSock.ReadByte();
        }
        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteShort(Length);
                Client.WSock.WriteByteArray(Data);
                Client.WSock.WriteByte(Percent);
                Client.WSock.Purge();
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
            SizeX = Client.WSock.ReadShort();
            SizeZ = Client.WSock.ReadShort();
            SizeY = Client.WSock.ReadShort();
        }
        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteShort(SizeX);
                Client.WSock.WriteShort(SizeZ);
                Client.WSock.WriteShort(SizeY);
                Client.WSock.Purge();
            
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
            X = Client.WSock.ReadShort();
            Z = Client.WSock.ReadShort();
            Y = Client.WSock.ReadShort();
            Mode = Client.WSock.ReadByte();
            Block = Client.WSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteShort(X);
                Client.WSock.WriteShort(Z);
                Client.WSock.WriteShort(Y);
                Client.WSock.WriteByte(Mode);
                Client.WSock.WriteByte(Block);
                Client.WSock.Purge();
            
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            Client.HandleBlockChange(X, Y, Z, Mode, Block);
        }
    }

    public struct SetBlockServer : IPacket {
        public byte Id { get { return 6; } }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte Block { get; set; }

        public void Read(NetworkClient Client) {
            X = Client.WSock.ReadShort();
            Z = Client.WSock.ReadShort();
            Y = Client.WSock.ReadShort();
            Block = Client.WSock.ReadByte();
        }
        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteShort(X);
                Client.WSock.WriteShort(Z);
                Client.WSock.WriteShort(Y);
                Client.WSock.WriteByte(Block);
                Client.WSock.Purge();
            
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
            PlayerID = Client.WSock.ReadSByte();
            PlayerName = Client.WSock.ReadString();
            X = Client.WSock.ReadShort();
            Z = Client.WSock.ReadShort();
            Y = Client.WSock.ReadShort();
            Yaw = Client.WSock.ReadByte();
            Pitch = Client.WSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteSByte(PlayerID);
                Client.WSock.WriteString(PlayerName);
                Client.WSock.WriteShort(X);
                Client.WSock.WriteShort(Z);
                Client.WSock.WriteShort(Y);
                Client.WSock.WriteByte(Yaw);
                Client.WSock.WriteByte(Pitch);
                Client.WSock.Purge();
            
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
            PlayerID = Client.WSock.ReadSByte();
            X = Client.WSock.ReadShort();
            Z = Client.WSock.ReadShort();
            Y = Client.WSock.ReadShort();
            yaw = Client.WSock.ReadByte();
            pitch = Client.WSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteSByte(PlayerID);
                Client.WSock.WriteShort(X);
                Client.WSock.WriteShort(Z);
                Client.WSock.WriteShort(Y);
                Client.WSock.WriteByte(yaw);
                Client.WSock.WriteByte(pitch);
                Client.WSock.Purge();
            
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            if (yaw != Client.CS.MyEntity.Rot || pitch != Client.CS.MyEntity.Look) {
                Client.CS.MyEntity.Rot = yaw;
                Client.CS.MyEntity.Look = pitch;
            }

            if (X != Client.CS.MyEntity.X || Y != Client.CS.MyEntity.Y || Z != Client.CS.MyEntity.Z) {
                Client.CS.MyEntity.X = X;
                Client.CS.MyEntity.Y = Y;
                Client.CS.MyEntity.Z = Z;
            }

            if (Client.CS.CPEExtensions.ContainsKey("HeldBlock")) {
                if (Client.CS.HeldBlock == null)
                    Client.CS.HeldBlock = Core.Blockholder.GetBlock(0);

                if (Client.CS.HeldBlock.Id == PlayerID)
                    return;
                //TODO: Held Block changed event
                Client.CS.HeldBlock = Core.Blockholder.GetBlock(PlayerID);
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
            PlayerID = Client.WSock.ReadSByte();
            ChangeX = Client.WSock.ReadShort();
            ChangeZ = Client.WSock.ReadShort();
            ChangeY = Client.WSock.ReadShort();
            yaw = Client.WSock.ReadByte();
            pitch = Client.WSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteSByte(PlayerID);
                Client.WSock.WriteShort(ChangeX);
                Client.WSock.WriteShort(ChangeZ);
                Client.WSock.WriteShort(ChangeY);
                Client.WSock.WriteByte(yaw);
                Client.WSock.WriteByte(pitch);
                Client.WSock.Purge();
            
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
            PlayerID = Client.WSock.ReadSByte();
            ChangeX = Client.WSock.ReadShort();
            ChangeZ = Client.WSock.ReadShort();
            ChangeY = Client.WSock.ReadShort();
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
            PlayerID = Client.WSock.ReadSByte();
            Yaw = Client.WSock.ReadByte();
            Pitch = Client.WSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
            Client.WSock.WriteByte(Id);
            Client.WSock.WriteSByte(PlayerID);
            Client.WSock.WriteByte(Yaw);
            Client.WSock.WriteByte(Pitch);
            Client.WSock.Purge();
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct DespawnPlayer : IPacket {
        public byte Id { get { return 12; } }
        public sbyte PlayerID;

        public void Read(NetworkClient Client) {
            PlayerID = Client.WSock.ReadSByte();
        }

        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteSByte(PlayerID);
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct Message : IPacket {
        public byte Id { get { return 13; } }
        public sbyte PlayerID { get; set; }
        public string Text { get; set; }

        public void Read(NetworkClient Client) {
            PlayerID = Client.WSock.ReadSByte();
            Text = Client.WSock.ReadString();
        }

        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteSByte(PlayerID);
                Client.WSock.WriteString(Text);
                Client.WSock.Purge();
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            Chat.HandleIncomingChat(Client, Text);
        }
    }

    public struct Disconnect : IPacket {
        public byte Id { get { return 14; } }
        public string Reason { get; set; }

        public void Read(NetworkClient Client) {
            Reason = Client.WSock.ReadString();
        }

        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteString(Reason);
                Client.WSock.Purge();
            
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct UpdateRank : IPacket {
        public byte Id { get { return 15; } }
        public byte Rank { get; set; }

        public void Read(NetworkClient Client) {
            Rank = Client.WSock.ReadByte();
        }

        public void Write(NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(Rank);
                Client.WSock.Purge();
            
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct ExtInfo : IPacket {
        public byte Id { get { return 16; } }
        public string AppName { get; set; }
        public short ExtensionCount { get; set; }

        public void Read(Client.NetworkClient Client) {
            AppName = Client.WSock.ReadString();
            ExtensionCount = Client.WSock.ReadShort();
        }

        public void Write(Client.NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteString(AppName);
                Client.WSock.WriteShort(ExtensionCount);
                Client.WSock.Purge();
            
        }

        public void Handle(NetworkClient Client, Hypercube Core) {
            Core.Logger.Log("CPE", "Client " + Client.CS.Ip + " is running on " + AppName + ", which supports " + ExtensionCount.ToString() + " extensions.", LogType.Info);
            Client.CS.Appname = AppName;
            Client.CS.Extensions = ExtensionCount;

            if (Client.CS.Extensions == 0)
                CPE.CPEPackets(Client);
        }
    }

    public struct ExtEntry : IPacket {
        public byte Id { get { return 17; } }
        public string ExtName { get; set; }
        public int Version { get; set; }

        public void Read(Client.NetworkClient Client) {
            ExtName = Client.WSock.ReadString();
            Version = Client.WSock.ReadInt();
        }

        public void Write(Client.NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteString(ExtName);
                Client.WSock.WriteInt(Version);
                Client.WSock.Purge();
            
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
            Distance = Client.WSock.ReadShort();
        }

        public void Write(Client.NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteShort(Distance);
                Client.WSock.Purge();
            
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }

    public struct CustomBlockSupportLevel : IPacket {
        public byte Id { get { return 19; } }
        public byte SupportLevel { get; set; }

        public void Read(Client.NetworkClient Client) {
            SupportLevel = Client.WSock.ReadByte();
        }

        public void Write(Client.NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(SupportLevel);
                Client.WSock.Purge();
            
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
            BlockToHold = Client.WSock.ReadByte();
            PreventChange = Client.WSock.ReadByte();
        }

        public void Write(Client.NetworkClient Client) {
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(BlockToHold);
                Client.WSock.WriteByte(PreventChange);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteString(Label);
                Client.WSock.WriteString(Action);
                Client.WSock.WriteInt(KeyCode);
                Client.WSock.WriteByte(KeyMods);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteShort(NameID);
                Client.WSock.WriteString(PlayerName);
                Client.WSock.WriteString(ListName);
                Client.WSock.WriteString(GroupName);
                Client.WSock.WriteByte(GroupRank);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(EntityID);
                Client.WSock.WriteString(InGameName);
                Client.WSock.WriteString(SkinName);
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteShort(NameID);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(ColorType);
                Client.WSock.WriteShort(Red);
                Client.WSock.WriteShort(Green);
                Client.WSock.WriteShort(Blue);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(SelectionID);
                Client.WSock.WriteString(Label);
                Client.WSock.WriteShort(StartX);
                Client.WSock.WriteShort(StartZ);
                Client.WSock.WriteShort(StartY);
                Client.WSock.WriteShort(EndX);
                Client.WSock.WriteShort(EndZ);
                Client.WSock.WriteShort(EndY);
                Client.WSock.WriteShort(Red);
                Client.WSock.WriteShort(Green);
                Client.WSock.WriteShort(Blue);
                Client.WSock.WriteShort(Opacity);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(SelectionID);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(BlockType);
                Client.WSock.WriteByte(AllowPlacement);
                Client.WSock.WriteByte(AllowDeletion);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(EntityID);
                Client.WSock.WriteString(ModelName);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteString(TextureURL);
                Client.WSock.WriteByte(SideBlock);
                Client.WSock.WriteByte(EdgeBlock);
                Client.WSock.WriteShort(SideLevel);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(WeatherType);
                Client.WSock.Purge();
            
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
                Client.WSock.WriteByte(Id);
                Client.WSock.WriteByte(Flying);
                Client.WSock.WriteByte(NoClip);
                Client.WSock.WriteByte(Speeding);
                Client.WSock.WriteByte(SpawnControl);
                Client.WSock.WriteByte(ThirdPerson);
                Client.WSock.WriteShort(JumpHeight);
                Client.WSock.Purge();
            
        }

        public void Handle(NetworkClient Client, Hypercube Core) {

        }
    }
}