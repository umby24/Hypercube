Declare UnregisterCore(Name.s)

Declare RegisterCore(Name.s, Timer.i, *InitFunction, *ShutdownFunction, *MainFunction)

Declare CoreLoop()

Declare CoreInit()

Declare CoreShutdown()

Declare.s CleanseString(In.s)

Declare StringMatches(In.s)

Declare.s String_Multiline(Input.s) ; Teilt einen String in mehrere Zeilen auf / Split a string into multiple lines

Declare LoadNetworkSettings()

Declare LoadSystemSettings()

Declare SaveNetworkSettings()

Declare SaveSystemSettings()

Declare Settings_Init()

Declare Settings_Shutdown()

Declare Log_Init()

Declare Log_Shutdown()

Declare _Log(Type.s, Message.s, Line, Proc.s)

Declare LoadRanks(Filename.s)

Declare SaveRanks(Filename.s)

Declare.s GetRankPrefix(Rank)

Declare.s GetRankSuffix(Rank)

Declare.s GetRankformattedName(Name.s, Rank)

Declare Rank_Init()

Declare Rank_Main()

Declare Rank_Shutdown()

Declare LoadBlocks()

Declare SaveBlocks()

Declare BlockInit()

Declare BlockShutdown()

Declare BlockMain()

Declare PlayerDBInit()

Declare AddPlayer(Name.s, IP.s)

Declare GetPlayer(Name.s)

Declare.l GetPlayerNumber(Name.s)

Declare.l GetPlayerRank(Name.s)

Declare.l GetPlayerLogins(Name.s)

Declare.l GetPlayerKicks(Name.s)

Declare.f GetPlayerOntime(Name.s)

Declare.s GetPlayerIP(Name.s)

Declare IsPlayerStopped(Name.s)

Declare IsPlayerBanned(Name.s)

Declare.l GetPlayerMuted(Name.s)

Declare.s GetPlayerBanMessage(Name.s)

Declare.s GetPlayerKickMessage(Name.s)

Declare.s GetPlayerMuteMessage(Name.s)

Declare.s GetPlayerRankMessage(Name.s)

Declare.s GetPlayerStopMessage(Name.s)

Declare SetPlayerRank(Name.s, Rank, RankMessage.s="")

Declare SetPlayerLogins(Name.s, Logins)

Declare SetPlayerKicks(Name.s, Kicks)

Declare SetPlayerOntime(Name.s, Ontime)

Declare SetPlayerIP(Name.s, IP.s)

Declare SetPlayerStopped(Name.s, Stopped, Reason.s="")

Declare SetPlayerBanned(Name.s, Banned, Reason.s="")

Declare SetPlayerMuted(Name.s, Duration=0, Reason.s="")

Declare Send_ServerIdentification(UserType)

Declare Send_Ping()

Declare Send_LevelInit()

Declare Send_LevelChunk(ChunkLength.w, *ChunkData, Percentage)

Declare Send_LevelFinalize(Map_X.w, Map_Y.w, Map_Z.w)

Declare Send_SetBlock(X.w, Y.w, Z.w, Type)

Declare Send_PlayerSpawn(PlayerID.b, PlayerName.s, X.w, Y.w, Z.w, Rot, Look)

Declare Send_PlayerTeleport(PlayerID.b, X.w, Y.w, Z.w, Rot, Look)

Declare Send_PlayerDespawn(PlayerID.b)

Declare Send_ChatMessage(MessageType.b, Message.s)

Declare Send_Disconnect(Reason.s)

Declare Send_UpdateType(Type)

Declare Send_ExtInfo(AppName.s, ExtensionCount.w)

Declare Send_ExtEntry(ExtName.s, Version.l)

Declare Send_SetClickDistance(Distance.w)

Declare Send_CustomBlockSupportLevel(SupportLevel.a)

Declare Send_HoldThis(BlockToHold.a, PreventChange.a)

Declare Send_TextHotKey(Label.s, Action.s, KeyCode.l, KeyMods.a)

Declare Send_ExtAddPlayerName(NameID.w, PlayerName.s, ListName.s, GroupName.s, GroupRank.a)

Declare Send_ExtAddEntity(EntityID.a, IngameName.s, SkinName.s)

Declare Send_RemovePlayerName(NameID.w)

Declare Send_EnvSetColor(Type.a, Red.w, Green.w, Blue.w)

Declare Send_MakeSelection(SelectionID.a, Label.s, StartX.w, StartY.w, StartZ.w, EndX.w, EndY.w, EndZ.w, Red.w, Green.w, Blue.w, Opacity.w)

Declare Send_RemoveSelection(SelectionID.a)

Declare Send_BlockPermissions(Blocktype.a, AllowPlacement.a, AllowDeletion.a)

Declare Send_ChangeModel(EntityID.a, Modelname.s)

Declare Send_EnvMapAppearance(TextureURL.s, Sideblock.a, Edgeblock.a, Sidelevel.w)

Declare Send_EnvSetWeatherType(Weather.a)

Declare Send_HackControl(Flying.a, Noclip.a, Speeding.a, SpawnControl.a, ThirdPerson.a, Weathercontrol.a, Jumpheight.w)

Declare Network_Init()

Declare Network_Shutdown()

Declare Network_Event()

Declare ClientInputHandle(*Dummy)

Declare Network_Main()

Declare ClientCPEHandshake(*Handshake.PlayerIdentification)

Declare ClientCPEPackets(Client_ID)

Declare.s EmoteReplace(Message.s)

Declare GZip_Compress(*Output, Output_Len, *Input, Input_Len)

Declare MapLoadSettings(Filename.s) ; Loads settings from a D3 Map file, and converts it to ClassicWorld format.

Declare Select_Map(Filename.s)

Declare MapAddClient(Name.s)

Declare MapRemoveClient(Name.s)

Declare MapGetBlockpointer(X, Y, Z) ;!! Map must already be selected!!

Declare.b Map_Get_BlockID(Name.s, X, Y, Z)

Declare MapBlockChange(PlayerNumber.l, X.w, Y.w, Z.w, Block, Send) ; MainMutex should already be locked, and the map already selected! If not the server may crash!

Declare MapBlockChangeClient(PlayerNumber.l, Name.s, X.w, Y.w, Z.w, Block, Mode) ; ClientMutex should already be locked!! If not the server may crash!

Declare Handle_Tag(*Tag.NBT_Tag, Parent.s, *CWMap.CWMap)

Declare SerializedMapLoad(*Tag.NBT_Tag, *Map_To_Load.CWMap, Parent.s="")

Declare NewMapLoad(Filename.s) ; Loads a previously never-loaded map.

Declare NewUnloadedMap(Filename.s)

Declare MapLoad(*Map.HMap)

Declare MapSave(*Map.HMap, Unloading=#False)

Declare MapUnload(*Map.HMap) ; Unloads a map but keeps its settings in memory.

Declare MapSend(Name.s)

Declare MapCreate(Name.s, X.w, Y.w, Z.w)

Declare MapInit()

Declare MapMain()

Declare MapShutdown()

Declare MapAutoSave()

Declare PlayerDBInit()

Declare AddPlayer(Name.s, IP.s)

Declare GetPlayer(Name.s)

Declare.l GetPlayerNumber(Name.s)

Declare.l GetPlayerRank(Name.s)

Declare.l GetPlayerLogins(Name.s)

Declare.l GetPlayerKicks(Name.s)

Declare.f GetPlayerOntime(Name.s)

Declare.s GetPlayerIP(Name.s)

Declare IsPlayerStopped(Name.s)

Declare IsPlayerBanned(Name.s)

Declare.l GetPlayerMuted(Name.s)

Declare.s GetPlayerBanMessage(Name.s)

Declare.s GetPlayerKickMessage(Name.s)

Declare.s GetPlayerMuteMessage(Name.s)

Declare.s GetPlayerRankMessage(Name.s)

Declare.s GetPlayerStopMessage(Name.s)

Declare SetPlayerRank(Name.s, Rank, RankMessage.s="")

Declare SetPlayerLogins(Name.s, Logins)

Declare SetPlayerKicks(Name.s, Kicks)

Declare SetPlayerOntime(Name.s, Ontime)

Declare SetPlayerIP(Name.s, IP.s)

Declare SetPlayerStopped(Name.s, Stopped, Reason.s="")

Declare SetPlayerBanned(Name.s, Banned, Reason.s="")

Declare SetPlayerMuted(Name.s, Duration=0, Reason.s="")

Declare SelectClient(Client_ID)

Declare SelectClientByEntity(Entity_ID)

Declare ClientAdd(Client_ID)

Declare ClientDelete(Client_ID, WasDisconnect=#False)

Declare ClientDataReceive(Client_ID)

Declare Client_Init()

Declare Client_Shutdown()

Declare ClientDataSender(*MyClient.NetworkClient) ; Threaded function that handles the sending of client's data.

Declare Client_Login(*Handshake.PlayerIdentification)

Declare.a ClientReadByte()

Declare.b ClientReadSByte()

Declare.w EndianW(val.w) ; Change Endianness of a Short (Word). Yay inline ASM!

Declare.w ClientReadShort()

Declare.s ClientReadString()

Declare ClientReadByteArray()

Declare.l Endian(val.l)

Declare.l ClientReadInt()

Declare ClientWriteByte(Send.a)

Declare ClientWriteSByte(Send.b)

Declare ClientWriteShort(Send.w)

Declare ClientWriteString(Send.s)

Declare ClientWriteByteArray(*Send)

Declare ClientWriteInt(Send.l)

Declare GetFreeEntityID()

Declare GetFreeMapID(Name.s)

Declare SetEntityPosition(X, Y, Z, R, L, Mapname.s)    

Declare SendEntityToMap(ID, X.w, Y.w, Z.w, H, P)

Declare SendEntityPositions()

Declare AddEntity()

Declare EntityChange()

Declare DeleteEntity()

Declare Entity_Main()

Declare.s CreateSalt()

Declare DoHeartbeatClassicube(*Dummy)

Declare VerifyClientName(*Handshake.PlayerIdentification)

Declare Heartbeat_Init()

Declare Heartbeat_Shutdown()

Declare ChatToAll(Maps.s, Message.s, Log=#False)

Declare ChatToClient(Client_ID, Message.s, Log=#False)

Declare ChatMessage_All(Entity_ID, Message.s)

Declare ChatMessage_Map(Entity_ID, Message.s)

Declare ChatMessage_Entity(Entity_ID, PlayerName.s, Message.s)

Declare HandleError()

