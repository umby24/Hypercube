; Hypercube Networking
; By Umby24
; Function: Handle incoming packets, and send client outgoing packets.
; #####################

;-==== Init
InitNetwork()

;-==== Defines

Global MainNetwork.MainNetwork

Global NetworkInputThread = 0

;-==== Procedures

Procedure Network_Init()
    LoadNetworkSettings()
    
    MainNetwork\File_Date_Last = GetFileDate("Settings\Network.txt", #PB_Date_Modified)
    
    If MainNetwork\Server_ID
        Network_Shutdown()
    EndIf
    
    MainNetwork\Server_ID = CreateNetworkServer(#PB_Any, NetworkSettings\Port, #PB_Network_TCP)
    
    NetworkInputThread = CreateThread(@ClientInputHandle(), 0)
     
    If MainNetwork\Server_ID = 0
        _Log("Error", "Failed to start networking.", #PB_Compiler_Line, #PB_Compiler_Procedure)
    Else
        _Log("Info", "Server started on port " + Str(NetworkSettings\Port), #PB_Compiler_Line, #PB_Compiler_Procedure)
    EndIf
    
EndProcedure

Procedure Network_Shutdown()
    CloseNetworkServer(MainNetwork\Server_ID)
    MainNetwork\Server_ID = 0
    
    ;LockMutex(ClientMutex)
    ;ForEach NetworkClients()
    ;    Send_Disconnect("&cServer closing")
    ;Next
    ;UnlockMutex(ClientMutex)
    
    If IsThread(NetworkInputThread)
        KillThread(NetworkInputThread)
    EndIf
EndProcedure

Procedure Network_Event()
    Protected SEvent, Client_ID
        Repeat
            SEvent = NetworkServerEvent()
            Client_ID = EventClient()
            
            Select SEvent
                Case 0
                    Break
                Case #PB_NetworkEvent_Connect
                    If Client_ID
                        ClientAdd(Client_ID)
                    Else
                        _Log("Warn", "Client ID = 0", #PB_Compiler_Line, #PB_Compiler_Procedure)
                    EndIf
                    
                Case #PB_NetworkEvent_Data
                    ClientDataReceive(Client_ID)
                    
                Case #PB_NetworkEvent_Disconnect
                    ClientDelete(Client_ID, #True)
            EndSelect
        ForEver
EndProcedure

Procedure ClientInputHandle(*Dummy)
    Protected Command
    
    While Running
        LockMutex(ClientMutex)
        
        ForEach NetworkClients()
            If NetworkClients()\InputOffset > 0 ; Oh goody, We have data. Better parse that shit.
                Command = PeekA(NetworkClients()\InputBuffer) ; Looks into their buffer without advancing the position we read from
                
                Select Command
                    Case 0 ; Handshake
                        If Not NetworkClients()\InputOffset >= 131 ; Wait for more data
                            Continue
                        EndIf
                        
                        Define Handshake.PlayerIdentification
                        Handshake\PacketID = ClientReadByte()
                        Handshake\ProtocolVersion = ClientReadByte()
                        Handshake\Username = ClientReadString()
                        Handshake\Verification = ClientReadString()
                        Handshake\Client_ID = ClientReadByte()
                        
                        If NetworkClients()\LoggedIn = #True
                            ClearStructure(@Handshake, PlayerIdentification)
                            ProcedureReturn
                        EndIf
                        
                        If Handshake\ProtocolVersion <> 7
                            _Log("Info", "Disconnecting client '" + Handshake\Username + "'. Unsupported protocol version (" + Str(Handshake\ProtocolVersion) + ").", #PB_Compiler_Line, #PB_Compiler_Procedure)
                            Send_Disconnect("&cUnsupported protocol version.")
                            ClearStructure(@Handshake, PlayerIdentification)
                            Continue
                        EndIf
                        
                        If VerifyClientName(@Handshake) = #False
                            _Log("Info", "Disconnecting client '" + Handshake\Username + "'. Name verification failed.", #PB_Compiler_Line, #PB_Compiler_Procedure)
                            Send_Disconnect("&cName verification failed.")
                            ClearStructure(@Handshake, PlayerIdentification)
                            Continue
                        EndIf
                        
                        If StringMatches(Handshake\Username) = #True
                            _Log("Info", "Disconnecting client '" + Handshake\Username + "'. (" + NetworkClients()\IP + "). Invalid username.", #PB_Compiler_Line, #PB_Compiler_Procedure)
                            Send_Disconnect("&cInvalid name.")
                            ClearStructure(@Handshake, PlayerIdentification)
                            Continue
                        EndIf
                        
                        If Handshake\Username = ""
                            _Log("Info", "Disconnecting client '" + Handshake\Username + "'. (" + NetworkClients()\IP + "). Invalid username.", #PB_Compiler_Line, #PB_Compiler_Procedure)
                            Send_Disconnect("&cInvalid name.")
                            ClearStructure(@Handshake, PlayerIdentification)
                            Continue
                        EndIf
                        
                        If ListSize(NetworkClients()) > NetworkSettings\MaxPlayers
                            _Log("Info", "Disconnect client '" + Handshake\Username + "'. The server is full.", #PB_Compiler_Line, #PB_Compiler_Procedure)
                            Send_Disconnect("&eThe Server is full!")
                            ClearStructure(@Handshake, PlayerIdentification)
                            Continue
                        EndIf
                        
                        If NetworkClients()\LoggedIn = #False And Handshake\Client_ID = 66
                            _Log("Info", "CPE Client detected", 0, "")
                            ClientCPEHandshake(@Handshake)
                        ElseIf  NetworkClients()\LoggedIn = #False
                            Client_Login(@Handshake)
                        EndIf
                        
                        ClearStructure(@Handshake, PlayerIdentification)
                    Case 5 ; Place Block
                        If Not NetworkClients()\InputOffset >= 9 ; Wait for more data
                            Continue
                        EndIf
                        
                        Define BlockPlace.SetBlock
                        BlockPlace\PacketID = ClientReadByte()
                        BlockPlace\X = ClientReadShort()
                        BlockPlace\Z = ClientReadShort()
                        BlockPlace\Y = ClientReadShort()
                        BlockPlace\Mode = ClientReadByte()
                        BlockPlace\BlockType = ClientReadByte()
                        
                        MapBlockChangeClient(0, NetworkClients()\Entity\CurrentMap, BlockPlace\X, BlockPlace\Y, BlockPlace\Z, BlockPlace\BlockType, BlockPlace\Mode)
                        
                        ClearStructure(@BlockPlace, SetBlock)
                    Case 8 ; Teleport
                        If Not NetworkClients()\InputOffset >= 10
                            Continue
                        EndIf
                        
                        Define ClientMove.PositionUpdate
                        ClientMove\PacketID = ClientReadByte()
                        ClientMove\PlayerID = ClientReadByte()
                        ClientMove\X = ClientReadShort()
                        ClientMove\Z = ClientReadShort()
                        ClientMove\Y = ClientReadShort()
                        ClientMove\Rotation = ClientReadByte()
                        ClientMove\Look = ClientReadByte()
                        
                        If NetworkClients()\HeldBlock = #True
                            NetworkCLients()\Entity\HeldBlock = ClientMove\PlayerID
                        EndIf
                        
                        NetworkClients()\Entity\X = ClientMove\X
                        NetworkClients()\Entity\Y = ClientMove\Y
                        NetworkClients()\Entity\Z = ClientMove\Z
                        NetworkClients()\Entity\Rot = ClientMove\Rotation
                        NetworkClients()\Entity\Look = ClientMove\Look
                        
                        ClearStructure(@ClientMove, PositionUpdate)
                    Case 13 ; Chat Message
                        If Not NetworkClients()\InputOffset >= 66
                            Continue
                        EndIf
                        
                        Define ClientChat.Message
                        ClientChat\PacketID = ClientReadByte()
                        ClientChat\MessageType = ClientReadByte()
                        ClientChat\Message = ClientReadString()
                        
                        If Left(ClientChat\Message, 1) = "/"
                            ; Handle Command
                        ElseIf Left(ClientChat\Message, 1) = "#" Or NetworkClients()\Entity\GlobalChat = #True
                            ChatMessage_All(NetworkClients()\Entity\id, ClientChat\Message)
                        ElseIf Left(ClientChat\Message, 1) = "@"
                            Define PM.s = Mid(StringField(ClientChat\Message, 1, " "), 2)
                            ChatMessage_Entity(NetworkClients()\Entity\ID, PM, Mid(ClientChat\Message, 2+Len(PM)))
                        Else
                            ChatMessage_Map(NetworkClients()\Entity\ID, ClientChat\Message)
                        EndIf
                        
                        
                        ClearStructure(@ClientChat, Message)
                    Case 16 ; ExtInfo
                        If Not NetworkClients()\InputOffset >= 67
                            Continue
                        EndIf
                        
                        Define ClientExt.ExtInfo
                        ClientExt\PacketID = ClientReadByte()
                        ClientExt\AppName = ClientReadString()
                        ClientExt\ExtensionCount = ClientReadShort()
                        
                        _Log("Info", "Client " + NetworkClients()\IP + " is running on " + ClientExt\AppName + ", which supports " + Str(ClientExt\ExtensionCount) + " extensions.", #PB_Compiler_Line, #PB_Compiler_Procedure)
                        
                        NetworkClients()\CPE = #True
                        NetworkClients()\CustomExtensions = ClientExt\ExtensionCount
                        
                        ClearStructure(@ClientExt, ExtInfo)
                    Case 17 ; ExtEntry
                        If Not NetworkClients()\InputOffset >= 69
                            Continue
                        EndIf
                        
                        Define ClientEntry.ExtEntry
                        ClientEntry\PacketID = ClientReadByte()
                        ClientEntry\ExtName = ClientReadString()
                        ClientEntry\Version = ClientReadInt()
                        
                        AddElement(NetworkClients()\Extensions())
                        NetworkClients()\Extensions() = ClientEntry\ExtName
                        
                        AddElement(networkClients()\ExtensionVersions())
                        NetworkClients()\ExtensionVersions() = ClientEntry\Version
                        
                        Select ClientEntry\ExtName
                            Case "CustomBlocks"
                                NetworkClients()\CustomBlocks = #True  
                            Case "HeldBlock"
                                NetworkClients()\HeldBlock = #True  
                            Case "ClickDistance"
                                NetworkClients()\ClickDistance = #True
                            Case "SelectionCuboid"
                                NetworkClients()\SelectionCuboid = #True
                            Case "ExtPLayerList"
                                NetworkClients()\ExtPlayerList = #True
                            Case "ChangeModel"
                                NetworkClients()\ChangeModel = #True
                            Case "EnvWeatherType"
                                NetworkClients()\CPEWeather = #True
                            Case "EnvColors"
                                NetworkClients()\EnvColors = #True  
                            Case "MessageTypes"
                                NetworkClients()\MessageTypes = #True
                            Case "BlockPermissions"
                                NetworkClients()\BlockPermissions = #True
                            Case "EnvMapAppearance"
                                NetworkClients()\EnvMapAppearance = #True
                            Case "HackControl"
                                NetworkClients()\HackControl = #True
                            Case "TextHotKey"
                                NetworkClients()\TextHotkey = #True
                        EndSelect
                        
                        NetworkClients()\CustomExtensions - 1
                        
                        If NetworkClients()\CustomExtensions = 0
                            ClientCPEPackets(NetworkClients()\Client_ID)
                        EndIf
                        
                        ClearStructure(@ClientEntry, ExtEntry)
                    Case 19 ; CustomBlockSupportLevel
                        If Not NetworkClients()\InputOffset >= 2
                            Continue
                        EndIf
                        
                        Define ClientBlocks.CustomBlockSupportLevel
                        ClientBlocks\PacketID = ClientReadByte()
                        ClientBlocks\SupportLevel = ClientReadByte()
                        
                        NetworkClients()\CustomBlocks_Level = ClientBlocks\SupportLevel
                        Client_Login(#Null)
                        
                        ClearStructure(@ClientBlocks, CustomBlockSupportLevel)
                    Default
                        _Log("Warn", "Unknown packet received from " + NetworkClients()\IP + ". (" + Str(Command) + ")", #PB_Compiler_Line, #PB_Compiler_Procedure)
                        Send_Disconnect("Bad packed ID Received: " + Str(Command))
                EndSelect
                
            EndIf
        Next
        UnlockMutex(ClientMutex)
        Delay(5)
    Wend
EndProcedure

Procedure Network_Main()
    Protected FileDate
    FileDate = GetFileDate("Settings\Network.txt", #PB_Date_Modified)
    
    If MainNetwork\File_Date_Last <> FileDate
        LoadNetworkSettings()
        MainNetwork\File_Date_Last = FileDate
    EndIf
    
EndProcedure

RegisterCore("NetworkEvent", 5, #Null, #Null, @Network_Event())
RegisterCore("Network", 1000, @Network_Init(), @Network_Shutdown(), @Network_Main()) ; Register network with the server. THIS ONE.
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 183
; FirstLine = 162
; Folding = -
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory