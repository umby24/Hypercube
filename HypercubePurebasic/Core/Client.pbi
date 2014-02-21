#Client_BufferSize = 32

Procedure SelectClient(Client_ID)
    ForEach NetworkClients()
        If NetworkClients()\Client_ID = Client_ID
            ProcedureReturn #True
        EndIf
    Next
    
    ProcedureReturn #False
EndProcedure

Procedure SelectClientByEntity(Entity_ID)
    ForEach NetworkClients()
        If NetworkClients()\Entity\id = Entity_ID
            ProcedureReturn #True
        EndIf
    Next
    
    ProcedureReturn #False
EndProcedure

Procedure ClientAdd(Client_ID)
    If Client_ID = 0
        _Log("Warn", "ClientID = 0", #PB_Compiler_Line, #PB_Compiler_Procedure)
        CloseNetworkConnection(Client_ID)
        ProcedureReturn
    EndIf
    
    LockMutex(ClientMutex)
    
    If SelectClient(Client_ID) = #True
        _Log("Warn", "ClientID Already exists.", #PB_Compiler_Line, #PB_Compiler_Procedure)
        CloseNetworkConnection(Client_ID)
        UnlockMutex(ClientMutex)
        ProcedureReturn
    EndIf
    
    AddElement(NetworkClients())
    
    NetworkClients()\Client_ID = Client_ID
    NetworkClients()\InputBuffer = AllocateMemory(#Client_BufferSize)
    NetworkClients()\InputOffset = 0
    NetworkClients()\OutputBuffer = AllocateMemory(#Client_BufferSize)
    NetworkClients()\OutputOffset = 0
    NetworkClients()\IP = IPString(GetClientIP(Client_ID))
    NetworkClients()\LastActive = ElapsedMilliseconds()
    NetworkClients()\WriteLock = CreateMutex()
    
    NetworkClients()\EmoteFix = #False
    NetworkClients()\LoggedIn = #False
    
    If NetworkClients()\InputBuffer = 0 Or NetworkClients()\OutputBuffer = 0
        _Log("Warn", "Failed to allocate memory for Buffers.", #PB_Compiler_Line, #PB_Compiler_Procedure)
        ClearStructure(NetworkClients(), NetworkClient)
        DeleteElement(NetworkClients())
        CloseNetworkConnection(Client_ID)
        UnlockMutex(ClientMutex)
        ProcedureReturn
    EndIf
    
    ; Start the Send-Buffer
    NetworkClients()\WriteThread = CreateThread(@ClientDataSender(), @NetworkClients())
    _Log("Info", "Client Added. (IP = " + NetworkCLients()\IP + ")", #PB_Compiler_Line, #PB_Compiler_Procedure)
    
    UnlockMutex(ClientMutex)
EndProcedure

Procedure ClientDelete(Client_ID, WasDisconnect=#False)
    Protected Tempname.s, Tempname2.s, Loggedin
    
    If WasDisconnect = #True
        LockMutex(ClientMutex)
        MainHeartbeat\Clients - 1
        
        If SelectClient(Client_ID)
            DeleteEntity()
            
            KillThread(NetworkClients()\WriteThread)
            
            FreeMemory(NetworkClients()\InputBuffer)
            FreeMemory(NetworkClients()\OutputBuffer)
            
            Tempname = NetworkClients()\Entity\Username
            Tempname2 = NetworkClients()\Entity\FormattedName
            Loggedin = NetworkClients()\LoggedIn
            
            _Log("Info", "Client deleted. " + NetworkClients()\IP, 0, "")
            
            ClearStructure(@NetworkClients(), NetworkClient)
            DeleteElement(Networkclients())
            
            If Tempname2 <> "" And Loggedin
                _Log("Info", "Player logged out. (Name: " + Tempname + ")", #PB_Compiler_Line, #PB_Compiler_Procedure)
                ChatToAll("", "&ePlayer " + Tempname2 + "&e logged out.")
                MainHeartbeat\Clients - 1
            EndIf
        EndIf
        
        UnlockMutex(ClientMutex)
    Else
        DeleteEntity()
        KillThread(NetworkClients()\WriteThread)
            
        FreeMemory(NetworkClients()\InputBuffer)
        FreeMemory(NetworkClients()\OutputBuffer)
        
        Tempname = NetworkClients()\Entity\Username
        Tempname2 = NetworkClients()\Entity\FormattedName
        Loggedin = NetworkClients()\LoggedIn
        
        CloseNetworkConnection(NetworkClients()\Client_ID)
            
        _Log("Info", "Client deleted. " + NetworkClients()\IP, 0, "")
        
        ClearStructure(@NetworkClients(), NetworkClient)
        DeleteElement(Networkclients())
        
        If Tempname2 <> "" And Loggedin
            _Log("Info", "Player logged out. (Name: " + Tempname + ")", #PB_Compiler_Line, #PB_Compiler_Procedure)
            ChatToAll("", "&ePlayer " + Tempname2 + "&e logged out.")
            MainHeartbeat\Clients - 1
        EndIf
    EndIf
EndProcedure

Procedure ClientDataReceive(Client_ID)
    Protected Result.i, BytesReceived
    ;_Log("Debug", "Start Receive", 0, "")
    LockMutex(ClientMutex)
    
    If SelectClient(Client_ID)
        BytesReceived = ReceiveNetworkData(Client_ID, NetworkClients()\InputBuffer + NetworkClients()\InputOffset, #Client_BufferSize)
        
        ;While BytesReceived > 0
            NetworkClients()\InputOffset + BytesReceived
            
            If #Client_BufferSize - BytesReceived <> 0
                Result = ReAllocateMemory(NetworkClients()\InputBuffer, NetworkClients()\InputOffset + (#Client_BufferSize - (#Client_BufferSize - BytesReceived)))
            Else
                Result = ReAllocateMemory(NetworkClients()\InputBuffer, NetworkClients()\InputOffset + #Client_BufferSize)
            EndIf
            
            If Result <> 0
                NetworkCLients()\InputBuffer = Result
            Else
                Send_Disconnect("&cFailed to allocated memory.")
                UnlockMutex(ClientMutex)
                ProcedureReturn
            EndIf
            
            ;If BytesReceived <> #Client_BufferSize
            ;    BytesReceived = 0
            ;Else
                ;BytesReceived = ReceiveNetworkData(Client_ID, NetworkClients()\InputBuffer + NetworkClients()\InputOffset, #Client_BufferSize)
            ;EndIf
        ;Wend
    EndIf
    
    UnlockMutex(ClientMutex)
    ;_Log("Debug", "End Receive", 0, "")
EndProcedure

Procedure Client_Init()
EndProcedure

Procedure Client_Shutdown()
EndProcedure

Procedure ClientDataSender(*MyClient.NetworkClient) ; Threaded function that handles the sending of client's data.
    While Running
        If *MyClient\OutputOffset > 0
            ;If TryLockMutex(*MyClient\WriteLock) = 0
            ;    Continue
            ;EndIf
            
            LockMutex(*MyClient\WriteLock)
            
            SendNetworkData(*MyClient\Client_ID, *MyClient\OutputBuffer, *MyClient\OutputOffset)
            *MyClient\OutputOffset = 0
            *MyClient\OutputBuffer = ReAllocateMemory(*MyClient\OutputBuffer, #Client_BufferSize)
            
            UnlockMutex(*MyClient\WriteLock)
        EndIf
        Delay(5)
    Wend
EndProcedure

Procedure Client_Login(*Handshake.PlayerIdentification)
    
    If NetworkClients()\CPE = #False
        NetworkClients()\Entity\Username = *Handshake\Username
        NetworkClients()\MPPass = *Handshake\Verification
    EndIf
    
    If GetPlayer(NetworkClients()\Entity\Username) = #False
        AddPlayer(NetworkClients()\Entity\Username, NetworkClients()\IP)
    EndIf
    
    If IsPlayerBanned(NetworkClients()\Entity\Username)
        _Log("Info", "Login failed: Player is banned. (Name=" + NetworkClients()\Entity\Username + " IP=" + Networkclients()\IP + ")", 0, "")
        Send_Disconnect("Banned: " + GetPlayerBanMessage(NetworkClients()\Entity\Username))
        ProcedureReturn
    EndIf
    
    NetworkClients()\LoggedIn = #True
    NetworkClients()\Entity\FormattedName = GetRankformattedName(NetworkClients()\Entity\Username, GetPlayerRank(NetworkClients()\Entity\Username))
    
    SetPlayerIP(NetworkClients()\Entity\Username, NetworkClients()\IP)
    SetPlayerLogins(NetworkClients()\Entity\Username, GetPlayerLogins(NetworkClients()\Entity\Username) + 1)
    
    NetworkClients()\Entity\CurrentMap = SystemSettings\Main_World
    
    
    MapSend(SystemSettings\Main_World)
    
    MapAddClient(SystemSettings\Main_World)
    
    _Log("Info", "Player logged in. (Name=" + NetworkClients()\Entity\Username + ")", 0, "")
    ChatToAll("", "&ePlayer " + NetworkClients()\Entity\FormattedName + "&e logged in.")
    
    ChatToClient(NetworkClients()\Client_ID, SystemSettings\Welcome_Message)
    AddEntity()
    
    MainHeartbeat\Clients + 1
EndProcedure

;{ Network Writing Functions
Procedure.a ClientReadByte()
    Protected Result.a, *TempBuffer
    
    If NetworkClients()\InputOffset >= 1
        Result = PeekA(NetworkClients()\InputBuffer)
        ; Remove byte from input buffer..
        NetworkClients()\InputOffset = NetworkClients()\InputOffset - 1
        *TempBuffer = AllocateMemory(NetworkClients()\InputOffset + #Client_BufferSize) ; Allocate to the side of the input buffer plus our padding size.
        
        CopyMemory(NetworkClients()\InputBuffer + 1, *TempBuffer, NetworkClients()\InputOffset) ; Copy the old data in
        FreeMemory(NetworkClients()\InputBuffer) ; Free our old buffer
        
        NetworkClients()\InputBuffer = *TempBuffer ; And assign the memoryspace of TempBuffer as our new InputBuffer.
        ProcedureReturn Result
    EndIf
    
    ProcedureReturn 0
EndProcedure

Procedure.b ClientReadSByte()
    Protected Result.B, *TempBuffer
    If NetworkClients()\InputOffset >= 1
        Result = PeekB(NetworkClients()\InputBuffer)
        ; Remove byte from input buffer..
        NetworkClients()\InputOffset = NetworkClients()\InputOffset - 1
        *TempBuffer = AllocateMemory(NetworkClients()\InputOffset + #Client_BufferSize) ; Allocate to the side of the input buffer plus our padding size.
        
        CopyMemory(NetworkClients()\InputBuffer + 1, *TempBuffer, NetworkClients()\InputOffset) ; Copy the old data in
        FreeMemory(NetworkClients()\InputBuffer) ; Free our old buffer
        
        NetworkClients()\InputBuffer = *TempBuffer ; And assign the memoryspace of TempBuffer as our new InputBuffer.
        ProcedureReturn Result
    EndIf
    
    ProcedureReturn -1
EndProcedure

Procedure.w EndianW(val.w) ; Change Endianness of a Short (Word). Yay inline ASM!
  !MOV ax, word[p.v_val]
  !XCHG al, ah                ; Swap Lo byte <-> Hi byte
  !MOV word[p.v_val], ax
  ProcedureReturn val
EndProcedure

Procedure.w ClientReadShort()
    Protected Result.w, *TempBuffer
    If NetworkClients()\InputOffset >= 2
        Result.w = PeekW(NetworkClients()\InputBuffer)
        Result = EndianW(Result)
        ; Remove byte from input buffer..
        NetworkClients()\InputOffset = NetworkClients()\InputOffset - 2
        *TempBuffer = AllocateMemory(NetworkClients()\InputOffset + #Client_BufferSize) ; Allocate to the side of the input buffer plus our padding size.
        
        CopyMemory(NetworkClients()\InputBuffer + 2, *TempBuffer, NetworkClients()\InputOffset) ; Copy the old data in
        FreeMemory(NetworkClients()\InputBuffer) ; Free our old buffer
        
        NetworkClients()\InputBuffer = *TempBuffer ; And assign the memoryspace of TempBuffer as our new InputBuffer.
        ProcedureReturn Result
    EndIf
    
    ProcedureReturn -1
EndProcedure

Procedure.s ClientReadString()
    Protected Result.s{64}, *TempBuffer
    If NetworkClients()\InputOffset >= 64
        Result = PeekS(NetworkClients()\InputBuffer, 64)
        ; Remove byte from input buffer..
        NetworkClients()\InputOffset = NetworkClients()\InputOffset - 64
        *TempBuffer = AllocateMemory(NetworkClients()\InputOffset + #Client_BufferSize) ; Allocate to the side of the input buffer plus our padding size.
        
        CopyMemory(NetworkClients()\InputBuffer + 64, *TempBuffer, NetworkClients()\InputOffset) ; Copy the old data in
        FreeMemory(NetworkClients()\InputBuffer) ; Free our old buffer
        
        NetworkClients()\InputBuffer = *TempBuffer ; And assign the memoryspace of TempBuffer as our new InputBuffer.
        ProcedureReturn Trim(Result)
    EndIf
    
    ProcedureReturn ""
EndProcedure

Procedure ClientReadByteArray()
    Protected *Result, *TempBuffer
    If NetworkClients()\InputOffset >= 1024
        *Result = AllocateMemory(1024)
        CopyMemory(NetworkClients()\InputBuffer, *Result, 1024)
        ; Remove byte from input buffer..
        NetworkClients()\InputOffset = NetworkClients()\InputOffset - 1024
        *TempBuffer = AllocateMemory(NetworkClients()\InputOffset + #Client_BufferSize) ; Allocate to the side of the input buffer plus our padding size.
        
        CopyMemory(NetworkClients()\InputBuffer + 1024, *TempBuffer, NetworkClients()\InputOffset) ; Copy the old data in
        FreeMemory(NetworkClients()\InputBuffer) ; Free our old buffer
        
        NetworkClients()\InputBuffer = *TempBuffer ; And assign the memoryspace of TempBuffer as our new InputBuffer.
        ProcedureReturn *Result
    EndIf
    
    ProcedureReturn -1
EndProcedure

Procedure.l Endian(val.l)
    !MOV Eax,dword[p.v_val]
    !BSWAP Eax
    ProcedureReturn
EndProcedure

Procedure.l ClientReadInt()
    Protected Result.l, *TempBuffer
    If NetworkClients()\InputOffset >= 4
        Result = PeekL(NetworkClients()\InputBuffer)
        Result = Endian(Result)
        ; Remove byte from input buffer..
        NetworkClients()\InputOffset = NetworkClients()\InputOffset - 4
        *TempBuffer = AllocateMemory(NetworkClients()\InputOffset + #Client_BufferSize) ; Allocate to the side of the input buffer plus our padding size.
        
        CopyMemory(NetworkClients()\InputBuffer + 4, *TempBuffer, NetworkClients()\InputOffset) ; Copy the old data in
        FreeMemory(NetworkClients()\InputBuffer) ; Free our old buffer
        
        NetworkClients()\InputBuffer = *TempBuffer ; And assign the memoryspace of TempBuffer as our new InputBuffer.
        ProcedureReturn Result
    EndIf
    
    ProcedureReturn -1
EndProcedure

Procedure ClientWriteByte(Send.a)
    If MemorySize(NetworkClients()\OutputBuffer) < NetworkClients()\OutputOffset + 1
        NetworkClients()\OutputBuffer = ReAllocateMemory(NetworkClients()\OutputBuffer, NetworkClients()\OutputOffset + 1)
    EndIf
    
    PokeA(NetworkClients()\OutputBuffer + NetworkClients()\OutputOffset, Send)
    NetworkClients()\OutputOffset = NetworkClients()\OutputOffset + 1
EndProcedure

Procedure ClientWriteSByte(Send.b)
    If MemorySize(NetworkClients()\OutputBuffer) < NetworkClients()\OutputOffset + 1
        NetworkClients()\OutputBuffer = ReAllocateMemory(NetworkClients()\OutputBuffer, NetworkClients()\OutputOffset + 1)
    EndIf
    
    PokeB(NetworkClients()\OutputBuffer + NetworkClients()\OutputOffset, Send)
    NetworkClients()\OutputOffset = NetworkClients()\OutputOffset + 1
EndProcedure

Procedure ClientWriteShort(Send.w)
    If MemorySize(NetworkClients()\OutputBuffer) < NetworkClients()\OutputOffset + 2
        NetworkClients()\OutputBuffer = ReAllocateMemory(NetworkClients()\OutputBuffer, NetworkClients()\OutputOffset + 2)
    EndIf
    
    PokeW(NetworkClients()\OutputBuffer + NetworkClients()\OutputOffset, EndianW(Send))
    NetworkClients()\OutputOffset = NetworkClients()\OutputOffset + 2
EndProcedure

Procedure ClientWriteString(Send.s)
    If MemorySize(NetworkClients()\OutputBuffer) < NetworkClients()\OutputOffset + 64
        NetworkClients()\OutputBuffer = ReAllocateMemory(NetworkClients()\OutputBuffer, NetworkClients()\OutputOffset + 64)
    EndIf
    
    If Len(Send) < 64
        Send = LSet(Send, 64, " ")
    EndIf
    
    PokeS(NetworkClients()\OutputBuffer + NetworkClients()\OutputOffset, Send, 64)
    NetworkClients()\OutputOffset = NetworkClients()\OutputOffset + 64
EndProcedure

Procedure ClientWriteByteArray(*Send)
    If MemorySize(NetworkClients()\OutputBuffer) < NetworkClients()\OutputOffset + 1024
        NetworkClients()\OutputBuffer = ReAllocateMemory(NetworkClients()\OutputBuffer, NetworkClients()\OutputOffset + 1024)
    EndIf
    
    ;If MemorySize(*Send) < 1024
    ;    *Send = ReAllocateMemory(*Send, 1024)
    ;EndIf
    
    CopyMemory(*Send, NetworkClients()\OutputBuffer + NetworkClients()\OutputOffset, 1024)
    NetworkClients()\OutputOffset = NetworkClients()\OutputOffset + 1024
EndProcedure

Procedure ClientWriteInt(Send.l)
    If MemorySize(NetworkClients()\OutputBuffer) < NetworkClients()\OutputOffset + 4
        NetworkClients()\OutputBuffer = ReAllocateMemory(NetworkClients()\OutputBuffer, NetworkClients()\OutputOffset + 4)
    EndIf
    
    PokeL(NetworkClients()\OutputBuffer + NetworkClients()\OutputOffset, Endian(Send))
    NetworkClients()\OutputOffset = NetworkClients()\OutputOffset + 4
EndProcedure
;}
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 184
; FirstLine = 151
; Folding = -4---
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory