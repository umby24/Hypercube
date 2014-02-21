; Hypercube Structures
; By Umby24
; Function: Contain all major structures for the server to reduce clutter across other files.
; ################################

Structure PlayerIdentification
    PacketID.b
    ProtocolVersion.b
    Username.s{64}
    Verification.s{64}
    Client_ID.b
EndStructure

Structure SetBlock
    PacketID.b
    X.w
    Y.w
    Z.w
    Mode.b
    BlockType.b
EndStructure

Structure PositionUpdate
    PacketID.b
    PlayerID.b
    X.w
    Y.w
    Z.w
    Rotation.b
    Look.b
EndStructure

Structure Message
    PacketID.b
    MessageType.b
    Message.s{64}
EndStructure

;-==== Bi-Directional Packets

Structure ExtInfo
    PacketID.b
    AppName.s{64}
    ExtensionCount.w
EndStructure

Structure ExtEntry
    PacketID.b
    ExtName.s{64}
    Version.l
EndStructure

Structure CustomBlockSupportLevel
    PacketID.b
    SupportLevel.b
EndStructure

Structure Log_Main
    File_ID.l
    Last_Modified.l
    Filename.s
EndStructure
Structure Block_Main
    Filename.s
    Last_Modified.l
EndStructure

Structure NetworkSettings
    ServerName.s{64}
    MOTD.s{64}
    Port.w
    MaxPlayers.l
    Public.b
    WhiteList.b
    File_Date_Last.l
EndStructure

Structure SystemSettings
    LogFile.s
    Logging.b
    Main_World.s
    Welcome_Message.s
    File_Date_Last.l
EndStructure

Structure EventStruct
    InitFunction.i
    ShutdownFunciton.i
    MainFunction.i
    ID.s
    Timer.i
    Time.i
EndStructure

Structure Block
    ID.a
    Name.s
    OnClient.a
    CPELevel.a
    CPEReplace.a
    Physics.a
    PhysicsPlugin.s
    DeleteRank.w
    PlaceRank.w
    ReplaceOnLoad.a
    Special.b
    Killer.b
    Color.l
    
EndStructure

Structure RankMain
    File_Date_Last.l
EndStructure

Structure Rank
    Rank.l
    Name.s
    Prefix.s
    Suffix.s
    Op.b
EndStructure

Structure MainNetwork
    File_Date_Last.l
    Server_ID.l
EndStructure

Structure EntityMain
    NextID.b
    FreeID.b
EndStructure

Structure Entity
    ID.l
    ClientID.a
    NameID.w
    
    CurrentMap.s
    Username.s
    FormattedName.s
    
    X.w
    Y.w
    Z.w
    Rot.b
    Look.b
    SendOwn.b
    
    HeldBlock.a
    Model.s
    
    LastMaterial.a
    BoundBlock.a
    BuildMaterial.w
    BuildMode.s
    
    *Client
    
    Muted.l
    GlobalChat.b
EndStructure


Structure NetworkClient
    Client_ID.l
    
    *OutputBuffer
    *InputBuffer
    
    InputOffset.l
    OutputOffset.l
    
    WriteThread.l
    WriteLock.l
    
    LastActive.l
    IP.s
    LoggedIn.b
    
    Entity.Entity
    
    MPPass.s
    
    Appname.s
    CPE.b                       ; If the client supports CPE.
    CustomExtensions.w          ; How many extensions the client supports
    CustomBlocks.b              ; If the client supports the CustomBlocks plugin
    CustomBlocks_Level.b        ; The level of block support the client has.
    HeldBlock.b                 ; Defines if the client supports CPE HeldBlock
    EmoteFix.b                  ; Defines if the client supports EmoteFix.
    ClickDistance.b             ; Defines if the client supports ClickDistance.
    SelectionCuboid.b           ; Defines if the client supports SelectionCuboid
    ExtPlayerList.b             ; Defines if the client supports extPlayerList.
    ChangeModel.b               ; Defines if the client supports ChangeModel.
    CPEWeather.b                ; Defines if the client support ExtWeatherType.
    EnvColors.b                 ; Defines if the client supports EnvColors.
    MessageTypes.b              ; Defines if the client supports MessageTypes.
    BlockPermissions.b          ; Defines if the client supports BlockPermissions.
    EnvMapAppearance.b          ; Defines if the client supports EnvMapAppearance.
    HackControl.b               ; Defines if the client supports HackControl
    TextHotkey.b                ; Defines if the client supports TextHotkey.
  
    List Extensions.s()        ; Holds a list of all supported plugins on the client.
    List ExtensionVersions.i()  ;Holds a list of the extension versions that work.
    List Selections.b()        ; Holds a list of all current selections for this player.
EndStructure

Structure Heartbeat
    Salt.s
    Clients.l
EndStructure

Structure CWMap
    FormatVersion.b
    Name.s
    *UUID
    Size_X.w
    Size_Y.w
    Size_Z.w
    ServiceName.s
    ServiceUsername.s
    GeneratingSoftware.s
    GeneratorName.s
    CreationTime.d
    AccessTime.d
    ModifiedTime.d
    Spawn_X.w
    Spawn_Y.w
    Spawn_Z.w
    Spawn_Rot.b
    Spawn_Look.b
    
    *Data
    *Metadata    ; As NBT_Tag, will only contain unsupported metadata.
    
    ;CPE Metadata Structures
    ClickDistanceVersion.l
    ClickDistance.w
    
    CustomBlocksVersion.l
    SupportLevel.w
    *FallbackArray
    
    EnvColorsVersion.l
    SkyColor.l
    CloudColor.l
    FogColor.l
    AmbientColor.l
    SunlightColor.l
    
    EnvMapAppearanceVersion.l
    TextureURL.s
    SideBlock.b
    EdgeBlock.b
    SideLevel.w
    
    ;Hypercube Specific
    NoErrors.b
    ErrorString.s
    
    Rank_Show.w    ; Metadata
    Rank_Build.w
    Rank_Join.w
    MOTD.s{64}
    Physics.b
    Building.b
EndStructure

Structure HMap
    BaseMap.CWMap
    Filename.s
    Loaded.b      ; If the file is loaded or unloaded (For memory conservation)
    FreeID.b
    NextID.b
    Clients.b
    LastClient.l ; The last time at which there was a client on this map.
EndStructure

Structure Map_Data
    ID.l                      ; Internal ID of the map (-1 is none)
    Unique_ID.s               ; Map Unique ID
    Name.s                    ; Name der Karte
    Save_Interval.l           ; Map save interval in minutes
    Save_Time.l               ; Time the map was last saved
    Directory.s               ; Directory of the map
    Overview_Type.l           ; Overview Type: 0=None, 1=2D, 2=Iso
    Size_X.u                  ; Map size
    Size_Y.u                  
    Size_Z.u                  
    Block_Counter.l [256]     ; Map Block Counter
    Spawn_X.f                 ; Spawnpoint
    Spawn_Y.f                 ; Spawnpoint
    Spawn_Z.f                 ; Spawnpoint
    Spawn_Rot.f               ; Spawnpoint
    Spawn_Look.f              ; Spawnpoint
    
    *Data                     ; Map Data
    *Physic_Data              ; Bitmask, if block is in physic queue (1 Byte --> 8 Blocks)
    *Blockchange_Data         ; Whether a block is already in the Block Change Queue Bitmask (1 Byte --> 8 Blocks)
    
    ;List Map_Block_Do.Map_Block_Do() 
    ;List Map_Block_Changed.Map_Block_Changed()
    ;List Undo_Step.Undo_Step(); Undo-Steps
    ;List Rank.Map_Rank_Element(); List of Rank-Boxes
    ;List Teleporter.Map_Teleporter_Element(); List of teleporters
    
    Physic_Stopped.a          ; When 1, Block physics are deactivated.
    Blockchange_Stopped.a     ; Wenn 1, Block changes are stopped
    Rank_Build.w              ; Map restrictions
    Rank_Join.w               
    Rank_Show.w               
    MOTD_Override.s           ; When "", Uses the main MOTD.
  
    SkyColor.l                ; CPE EnvSetColor Parameters.
    CloudColor.l
    FogColor.l
    alight.l
    dlight.l
    ColorsSet.b
  
    CustomAppearance.b      ; CPE EnvMapAppearance Parameters.
    CustomURL.s
    Side_Block.b
    Edge_Block.b
    Side_level.w
EndStructure


; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 327
; FirstLine = 45
; Folding = PGBg
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory