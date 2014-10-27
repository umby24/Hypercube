using Hypercube.Client;
using Hypercube.Core;
using Hypercube.Libraries;

namespace Hypercube.Network {
    // -- Contains structures for every supported packet.

    public struct Handshake : IPacket {

        public byte Id { get { return 0; } }
        public byte ProtocolVersion { get; set; }
        public string Name { get; set; }
        public string Motd { get; set; }
        public byte Usertype { get; set; }

        public void Read(NetworkClient client) {
            ProtocolVersion = client.WSock.ReadByte();
            Name = client.WSock.ReadString();
            Motd = client.WSock.ReadString();
            Usertype = client.WSock.ReadByte();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteByte(ProtocolVersion);
            client.WSock.WriteString(Name);
            client.WSock.WriteString(Motd);
            client.WSock.WriteByte(Usertype);
            client.WSock.Purge();
        }

        public void Handle(NetworkClient client) {
            client.CS.LoginName = Name;
            client.CS.MpPass = Motd;

            if (ProtocolVersion != 7) {
                ServerCore.Logger.Log("Handshake", "Disconnecting client '" + Name + "'. Unsupported protocol verison (" + ProtocolVersion + ")", LogType.Info);
                client.KickPlayer("Unsupported protocol version.");
                return;
            }

            if (!ServerCore.Hb.VerifyClientName(client)) {
                ServerCore.Logger.Log("Handshake", "Disconnecting client '" + Name + "'. Failed to verify name.", LogType.Info);
                client.KickPlayer("Name verification incorrect.");
                return;
            }

            if (Text.StringMatches(Name)) {
                ServerCore.Logger.Log("Handshake", "Disconnecting Client '" + Name + "'. Invalid characters in name.", LogType.Info);
                client.KickPlayer("Invalid characters in name.");
                return;
            }

            if (Name == "") {
                ServerCore.Logger.Log("Handshake", "Disconnecting Client '" + Name + "'. Invalid name.", LogType.Info);
                client.KickPlayer("Invalid name.");
                return;
            }

            if (ServerCore.OnlinePlayers > ServerCore.Nh.MaxPlayers) {
                client.KickNow("The server is full.");
                return;
            }

            client.CS.MpPass = Motd;

            if (Usertype == 66) {
                // -- CPE Client
                ServerCore.Logger.Log("Handshake", "CPE Client Detected.", LogType.Info);
                client.CS.CPE = true;
                CPE.CPEHandshake(client);
                return;
            }

            // -- Normal Client.
            client.CS.CPE = false;
            client.Login();
        }
    }

    public struct Ping : IPacket {
        public byte Id { get { return 1; } }

        public void Read(NetworkClient client) {

        }
        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.Purge();
            
        }
        public void Handle(NetworkClient client) {

        }
    }

    public struct LevelInit : IPacket {
        public byte Id { get { return 2; } }

        public void Read(NetworkClient client) {

        }
        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.Purge();
            
        }
        public void Handle(NetworkClient client) {

        }
    }

    public struct LevelChunk : IPacket {
        public byte Id { get { return 3; } }
        public short Length { get; set; }
        public byte[] Data { get; set; }
        public byte Percent { get; set; }

        public void Read(NetworkClient client) {
            Length = client.WSock.ReadShort();
            Data = client.WSock.ReadByteArray();
            Percent = client.WSock.ReadByte();
        }
        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteShort(Length);
                client.WSock.WriteByteArray(Data);
                client.WSock.WriteByte(Percent);
                client.WSock.Purge();
        }
        public void Handle(NetworkClient client) {

        }
    }

    public struct LevelFinalize : IPacket {
        public byte Id { get { return 4; } }

        public short SizeX { get; set; }
        public short SizeY { get; set; }
        public short SizeZ { get; set; }

        public void Read(NetworkClient client) {
            SizeX = client.WSock.ReadShort();
            SizeZ = client.WSock.ReadShort();
            SizeY = client.WSock.ReadShort();
        }
        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteShort(SizeX);
                client.WSock.WriteShort(SizeZ);
                client.WSock.WriteShort(SizeY);
                client.WSock.Purge();
            
        }
        public void Handle(NetworkClient client) {

        }
    }

    public struct SetBlock : IPacket {
        public byte Id { get { return 5; } }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte Mode { get; set; }
        public byte Block { get; set; }

        public void Read(NetworkClient client) {
            X = client.WSock.ReadShort();
            Z = client.WSock.ReadShort();
            Y = client.WSock.ReadShort();
            Mode = client.WSock.ReadByte();
            Block = client.WSock.ReadByte();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteShort(X);
            client.WSock.WriteShort(Z);
            client.WSock.WriteShort(Y);
            client.WSock.WriteByte(Mode);
            client.WSock.WriteByte(Block);
            client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {
            client.HandleBlockChange(X, Y, Z, Mode, Block);
        }
    }

    public struct SetBlockServer : IPacket {
        public byte Id { get { return 6; } }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte Block { get; set; }

        public void Read(NetworkClient client) {
            X = client.WSock.ReadShort();
            Z = client.WSock.ReadShort();
            Y = client.WSock.ReadShort();
            Block = client.WSock.ReadByte();
        }
        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteShort(X);
                client.WSock.WriteShort(Z);
                client.WSock.WriteShort(Y);
                client.WSock.WriteByte(Block);
                client.WSock.Purge();
            
        }
        public void Handle(NetworkClient client) {

        }
    }

    public struct SpawnPlayer : IPacket {
        public byte Id { get { return 7; } }
        public sbyte PlayerId { get; set; }
        public string PlayerName { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }

        public void Read(NetworkClient client) {
            PlayerId = client.WSock.ReadSByte();
            PlayerName = client.WSock.ReadString();
            X = client.WSock.ReadShort();
            Z = client.WSock.ReadShort();
            Y = client.WSock.ReadShort();
            Yaw = client.WSock.ReadByte();
            Pitch = client.WSock.ReadByte();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteSByte(PlayerId);
            client.WSock.WriteString(PlayerName);
            client.WSock.WriteShort(X);
            client.WSock.WriteShort(Z);
            client.WSock.WriteShort(Y);
            client.WSock.WriteByte(Yaw);
            client.WSock.WriteByte(Pitch);
            client.WSock.Purge();
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct PlayerTeleport : IPacket {
        public byte Id { get { return 8; } }
        public sbyte PlayerId { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }

        public void Read(NetworkClient client) {
            PlayerId = client.WSock.ReadSByte();
            X = client.WSock.ReadShort();
            Z = client.WSock.ReadShort();
            Y = client.WSock.ReadShort();
            Yaw = client.WSock.ReadByte();
            Pitch = client.WSock.ReadByte();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteSByte(PlayerId);
            client.WSock.WriteShort(X);
            client.WSock.WriteShort(Z);
            client.WSock.WriteShort(Y);
            client.WSock.WriteByte(Yaw);
            client.WSock.WriteByte(Pitch);
            client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {
            if (Yaw != client.CS.MyEntity.Rot || Pitch != client.CS.MyEntity.Look) {
                client.CS.MyEntity.Rot = Yaw;
                client.CS.MyEntity.Look = Pitch;
            }

            if (X != client.CS.MyEntity.X || Y != client.CS.MyEntity.Y || Z != client.CS.MyEntity.Z) {
                client.CS.MyEntity.X = X;
                client.CS.MyEntity.Y = Y;
                client.CS.MyEntity.Z = Z;
            }

            if (!client.CS.CPEExtensions.ContainsKey("HeldBlock")) 
                return;

            if (client.CS.HeldBlock == null)
                client.CS.HeldBlock = ServerCore.Blockholder.GetBlock(0);

            if (client.CS.HeldBlock.Id == PlayerId)
                return;

            //TODO: Held Block changed event
            client.CS.HeldBlock = ServerCore.Blockholder.GetBlock(PlayerId);
        }
    }

    public struct PosAndOrient : IPacket {
        public byte Id { get { return 9; } }
        public sbyte PlayerId { get; set; }
        public short ChangeX { get; set; }
        public short ChangeY { get; set; }
        public short ChangeZ { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }

        public void Read(NetworkClient client) {
            PlayerId = client.WSock.ReadSByte();
            ChangeX = client.WSock.ReadShort();
            ChangeZ = client.WSock.ReadShort();
            ChangeY = client.WSock.ReadShort();
            Yaw = client.WSock.ReadByte();
            Pitch = client.WSock.ReadByte();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteSByte(PlayerId);
            client.WSock.WriteShort(ChangeX);
            client.WSock.WriteShort(ChangeZ);
            client.WSock.WriteShort(ChangeY);
            client.WSock.WriteByte(Yaw);
            client.WSock.WriteByte(Pitch);
            client.WSock.Purge();        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct PositionUpdate : IPacket {
        public byte Id { get { return 10; } }
        public sbyte PlayerId { get; set; }
        public short ChangeX { get; set; }
        public short ChangeY { get; set; }
        public short ChangeZ { get; set; }

        public void Read(NetworkClient client) {
            PlayerId = client.WSock.ReadSByte();
            ChangeX = client.WSock.ReadShort();
            ChangeZ = client.WSock.ReadShort();
            ChangeY = client.WSock.ReadShort();
        }

        public void Write(NetworkClient client) {

        }
        public void Handle(NetworkClient client) {

        }
    }

    public struct OrientationUpdate : IPacket {
        public byte Id { get { return 11; } }
        public sbyte PlayerId { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }

        public void Read(NetworkClient client) {
            PlayerId = client.WSock.ReadSByte();
            Yaw = client.WSock.ReadByte();
            Pitch = client.WSock.ReadByte();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteSByte(PlayerId);
            client.WSock.WriteByte(Yaw);
            client.WSock.WriteByte(Pitch);
            client.WSock.Purge();
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct DespawnPlayer : IPacket {
        public byte Id { get { return 12; } }
        public sbyte PlayerId;

        public void Read(NetworkClient client) {
            PlayerId = client.WSock.ReadSByte();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteSByte(PlayerId);
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct Message : IPacket {
        public byte Id { get { return 13; } }
        public sbyte PlayerId { get; set; }
        public string Text { get; set; }

        public void Read(NetworkClient client) {
            PlayerId = client.WSock.ReadSByte();
            Text = client.WSock.ReadString();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteSByte(PlayerId);
            client.WSock.WriteString(Text);
            client.WSock.Purge();
        }

        public void Handle(NetworkClient client) {
            Chat.HandleIncomingChat(client, Text);
        }
    }

    public struct Disconnect : IPacket {
        public byte Id { get { return 14; } }
        public string Reason { get; set; }

        public void Read(NetworkClient client) {
            Reason = client.WSock.ReadString();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteString(Reason);
            client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct UpdateRank : IPacket {
        public byte Id { get { return 15; } }
        public byte Rank { get; set; }

        public void Read(NetworkClient client) {
            Rank = client.WSock.ReadByte();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteByte(Rank);
            client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct ExtInfo : IPacket {
        public byte Id { get { return 16; } }
        public string AppName { get; set; }
        public short ExtensionCount { get; set; }

        public void Read(NetworkClient client) {
            AppName = client.WSock.ReadString();
            ExtensionCount = client.WSock.ReadShort();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteString(AppName);
            client.WSock.WriteShort(ExtensionCount);
            client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {
            ServerCore.Logger.Log("CPE", "Client " + client.CS.Ip + " is running on " + AppName + ", which supports " + ExtensionCount + " extensions.", LogType.Info);
            client.CS.Appname = AppName;
            client.CS.Extensions = ExtensionCount;

            if (client.CS.Extensions == 0)
                CPE.CPEPackets(client);
        }
    }

    public struct ExtEntry : IPacket {
        public byte Id { get { return 17; } }
        public string ExtName { get; set; }
        public int Version { get; set; }

        public void Read(NetworkClient client) {
            ExtName = client.WSock.ReadString();
            Version = client.WSock.ReadInt();
        }

        public void Write(NetworkClient client) {
            client.WSock.WriteByte(Id);
            client.WSock.WriteString(ExtName);
            client.WSock.WriteInt(Version);
            client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {
            client.CS.CPEExtensions.Add(ExtName, Version);

            if (client.CS.CPEExtensions.Keys.Count == client.CS.Extensions)
                CPE.CPEPackets(client);
        }
    }

    public struct SetClickDistance : IPacket {
        public byte Id { get { return 18; } }
        public short Distance { get; set; }

        public void Read(NetworkClient client) {
            Distance = client.WSock.ReadShort();
        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteShort(Distance);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct CustomBlockSupportLevel : IPacket {
        public byte Id { get { return 19; } }
        public byte SupportLevel { get; set; }

        public void Read(NetworkClient client) {
            SupportLevel = client.WSock.ReadByte();
        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteByte(SupportLevel);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {
            client.CS.CustomBlocksLevel = SupportLevel;
            client.Login();
        }
    }

    public struct HoldThis : IPacket {
        public byte Id { get { return 20; } }
        public byte BlockToHold { get; set; }
        public byte PreventChange { get; set; }

        public void Read(NetworkClient client) {
            BlockToHold = client.WSock.ReadByte();
            PreventChange = client.WSock.ReadByte();
        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteByte(BlockToHold);
                client.WSock.WriteByte(PreventChange);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct SetTextHotKey : IPacket {
        public byte Id { get { return 21; } }
        public string Label { get; set; }
        public string Action { get; set; }
        public int KeyCode { get; set; }
        public byte KeyMods { get; set; }

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteString(Label);
                client.WSock.WriteString(Action);
                client.WSock.WriteInt(KeyCode);
                client.WSock.WriteByte(KeyMods);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct ExtAddPlayerName : IPacket {
        public byte Id { get { return 22; } }
        public short NameId { get; set; }
        public string PlayerName { get; set; }
        public string ListName { get; set; }
        public string GroupName { get; set; }
        public byte GroupRank { get; set; }

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteShort(NameId);
                client.WSock.WriteString(PlayerName);
                client.WSock.WriteString(ListName);
                client.WSock.WriteString(GroupName);
                client.WSock.WriteByte(GroupRank);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct ExtAddEntity : IPacket {
        public byte Id { get { return 23; } }
        public byte EntityId { get; set; }
        public string InGameName { get; set; }
        public string SkinName { get; set; }

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteByte(EntityId);
                client.WSock.WriteString(InGameName);
                client.WSock.WriteString(SkinName);
            
        }

        public void Handle(NetworkClient client) {

        }
    }
    public struct ExtRemovePlayerName : IPacket {
        public byte Id { get { return 24; } }
        public short NameId { get; set; }

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteShort(NameId);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct EnvSetColor : IPacket {
        public byte Id { get { return 25; } }
        public byte ColorType { get; set; }
        public short Red { get; set; }
        public short Green { get; set; }
        public short Blue { get; set; }

        public enum ColorTypes {
            SkyColor = 0,
            CloudColor,
            FogColor,
            AmbientColor,
            SunlightColor,
        }

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteByte(ColorType);
                client.WSock.WriteShort(Red);
                client.WSock.WriteShort(Green);
                client.WSock.WriteShort(Blue);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct MakeSelection : IPacket {
        public byte Id { get { return 26; } }
        public byte SelectionId { get; set; }
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

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteByte(SelectionId);
                client.WSock.WriteString(Label);
                client.WSock.WriteShort(StartX);
                client.WSock.WriteShort(StartZ);
                client.WSock.WriteShort(StartY);
                client.WSock.WriteShort(EndX);
                client.WSock.WriteShort(EndZ);
                client.WSock.WriteShort(EndY);
                client.WSock.WriteShort(Red);
                client.WSock.WriteShort(Green);
                client.WSock.WriteShort(Blue);
                client.WSock.WriteShort(Opacity);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct RemoveSelection : IPacket {
        public byte Id { get { return 27; } }
        public byte SelectionId { get; set; }

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteByte(SelectionId);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct SetBlockPermissions : IPacket {
        public byte Id { get { return 28; } }
        public byte BlockType { get; set; }
        public byte AllowPlacement { get; set; }
        public byte AllowDeletion { get; set; }

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteByte(BlockType);
                client.WSock.WriteByte(AllowPlacement);
                client.WSock.WriteByte(AllowDeletion);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }

    public struct ChangeModel : IPacket {
        public byte Id { get { return 29; } }
        public byte EntityId { get; set; }
        public string ModelName { get; set; }

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteByte(EntityId);
                client.WSock.WriteString(ModelName);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }
    public struct EnvSetMapAppearance : IPacket {
        public byte Id { get { return 30; } }
        public string TextureUrl { get; set; }
        public byte SideBlock { get; set; }
        public byte EdgeBlock { get; set; }
        public short SideLevel { get; set; }

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteString(TextureUrl);
                client.WSock.WriteByte(SideBlock);
                client.WSock.WriteByte(EdgeBlock);
                client.WSock.WriteShort(SideLevel);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }
    public struct EnvSetWeatherType : IPacket {
        public byte Id { get { return 31; } }
        public byte WeatherType { get; set; }

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteByte(WeatherType);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

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

        public void Read(NetworkClient client) {

        }

        public void Write(NetworkClient client) {
                client.WSock.WriteByte(Id);
                client.WSock.WriteByte(Flying);
                client.WSock.WriteByte(NoClip);
                client.WSock.WriteByte(Speeding);
                client.WSock.WriteByte(SpawnControl);
                client.WSock.WriteByte(ThirdPerson);
                client.WSock.WriteShort(JumpHeight);
                client.WSock.Purge();
            
        }

        public void Handle(NetworkClient client) {

        }
    }
}