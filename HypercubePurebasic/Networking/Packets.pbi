; Hypercube Packets
; By Umby24
; Function: Contains all structures, Sending, and Reading functions for individual Minecraft Packets.
; #######################

;-==== Structures
;-==== Server-to-Client Sending

Procedure Send_ServerIdentification(UserType)

    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(0)
    ClientWriteByte(7)
    ClientWriteString(LSet(NetworkSettings\ServerName, 64, " "))
    ClientWriteString(LSet(NetworkSettings\MOTD, 64, " "))
    ClientWriteByte(UserType)
    
    UnlockMutex(NetworkClients()\WriteLock)

EndProcedure

Procedure Send_Ping()
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(1)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_LevelInit()
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(2)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_LevelChunk(ChunkLength.w, *ChunkData, Percentage)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(3)
    ClientWriteShort(ChunkLength)
    ClientWriteByteArray(*ChunkData)
    ClientWriteByte(Percentage)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_LevelFinalize(Map_X.w, Map_Y.w, Map_Z.w)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(4)
    ClientWriteShort(Map_X)
    ClientWriteShort(Map_Z)
    ClientWriteShort(Map_Y)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_SetBlock(X.w, Y.w, Z.w, Type)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(6)
    ClientWriteShort(X)
    ClientWriteShort(Z)
    ClientWriteShort(Y)
    ClientWriteByte(Type)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_PlayerSpawn(PlayerID.b, PlayerName.s, X.w, Y.w, Z.w, Rot, Look)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(7)
    ClientWriteSByte(PlayerID)
    ClientWriteString(PlayerName)
    ClientWriteShort(X)
    ClientWriteShort(Z)
    ClientWriteShort(Y)
    ClientWriteByte(Rot)
    ClientWriteByte(Look)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_PlayerTeleport(PlayerID.b, X.w, Y.w, Z.w, Rot, Look)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(8)
    ClientWriteSByte(PlayerID)
    ClientWriteShort(X)
    ClientWriteShort(Z)
    ClientWriteShort(Y)
    ClientWriteByte(Rot)
    ClientWriteByte(Look)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_PlayerDespawn(PlayerID.b)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(12)
    ClientWriteSByte(PlayerID)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_ChatMessage(MessageType.b, Message.s)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(13)
    ClientWriteSByte(MessageType)
    ClientWriteString(Message)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_Disconnect(Reason.s)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(14)
    ClientWriteString(Reason)
    
    UnlockMutex(NetworkClients()\WriteLock)
    
    Delay(10)
    ClientDelete(NetworkClients()\Client_ID)
EndProcedure

Procedure Send_UpdateType(Type)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(15)
    ClientWriteByte(Type)
    
    UnlockMutex(NetworkClients()\WriteLock)    
EndProcedure

Procedure Send_ExtInfo(AppName.s, ExtensionCount.w)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(16)
    ClientWriteString(AppName)
    ClientWriteShort(ExtensionCount)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_ExtEntry(ExtName.s, Version.l)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(17)
    ClientWriteString(ExtName)
    ClientWriteInt(Version)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_SetClickDistance(Distance.w)
    LockMutex(NetworkClients()\WriteLock)
    ClientWriteByte(18)
    ClientWriteShort(Distance)
    UnlockMutex(NetworkClients()\WriteLock)    
EndProcedure

Procedure Send_CustomBlockSupportLevel(SupportLevel.a)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(19)
    ClientWriteByte(SupportLevel)
    
    UnlockMutex(NetworkClients()\WriteLock)    
EndProcedure

Procedure Send_HoldThis(BlockToHold.a, PreventChange.a)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(20)
    ClientWriteByte(BlockToHold)
    ClientWriteByte(PreventChange)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_TextHotKey(Label.s, Action.s, KeyCode.l, KeyMods.a)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(21)
    ClientWriteString(Label)
    ClientWriteString(Action)
    ClientWriteInt(KeyCode)
    ClientWriteByte(KeyMods)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_ExtAddPlayerName(NameID.w, PlayerName.s, ListName.s, GroupName.s, GroupRank.a)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(22)
    ClientWriteShort(NameID)
    ClientWriteString(PlayerName)
    ClientWriteString(ListName)
    ClientWriteString(GroupName)
    ClientWriteByte(GroupRank)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_ExtAddEntity(EntityID.a, IngameName.s, SkinName.s)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(23)
    ClientWriteByte(EntityID)
    ClientWriteString(IngameName)
    ClientWriteString(SkinName)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_RemovePlayerName(NameID.w)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(24)
    ClientWriteShort(NameID)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_EnvSetColor(Type.a, Red.w, Green.w, Blue.w)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(25)
    ClientWriteByte(Type)
    ClientWriteShort(Red)
    ClientWriteShort(Green)
    ClientWriteShort(Blue)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_MakeSelection(SelectionID.a, Label.s, StartX.w, StartY.w, StartZ.w, EndX.w, EndY.w, EndZ.w, Red.w, Green.w, Blue.w, Opacity.w)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(26)
    ClientWriteByte(SelectionID)
    ClientWriteString(Label)
    ClientWriteShort(StartX)
    ClientWriteShort(StartZ)
    ClientWriteShort(StartY)
    ClientWriteShort(EndX)
    ClientWriteShort(EndZ)
    ClientWriteShort(EndY)
    ClientWriteShort(Red)
    ClientWriteShort(Green)
    ClientWriteShort(Blue)
    ClientWriteShort(Opacity)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_RemoveSelection(SelectionID.a)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(27)
    ClientWriteByte(SelectionID)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_BlockPermissions(Blocktype.a, AllowPlacement.a, AllowDeletion.a)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(28)
    ClientWriteByte(Blocktype)
    ClientWriteByte(AllowPlacement)
    ClientWriteByte(AllowDeletion)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_ChangeModel(EntityID.a, Modelname.s)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(29)
    ClientWriteByte(EntityID)
    ClientWriteString(Modelname)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_EnvMapAppearance(TextureURL.s, Sideblock.a, Edgeblock.a, Sidelevel.w)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(30)
    ClientWriteString(TextureURL)
    ClientWriteByte(Sideblock)
    ClientWriteByte(Edgeblock)
    ClientWriteByte(Sidelevel)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_EnvSetWeatherType(Weather.a)
    LockMutex(NetworkClients()\WriteLock)

    ClientWriteByte(31)
    ClientWriteByte(Weather)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

Procedure Send_HackControl(Flying.a, Noclip.a, Speeding.a, SpawnControl.a, ThirdPerson.a, Weathercontrol.a, Jumpheight.w)
    LockMutex(NetworkClients()\WriteLock)
    
    ClientWriteByte(32)
    ClientWriteByte(Flying)
    ClientWriteByte(Noclip)
    ClientWriteByte(Speeding)
    ClientWriteByte(SpawnControl)
    ClientWriteByte(ThirdPerson)
    ClientWriteByte(Weathercontrol)
    ClientWriteShort(Jumpheight)
    
    UnlockMutex(NetworkClients()\WriteLock)
EndProcedure

; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 128
; FirstLine = 120
; Folding = -----
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory