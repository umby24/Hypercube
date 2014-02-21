; Hypercube Entity.pbi
; By umby24
; Function: Provides Entity management for the server.
; ################################

Procedure GetFreeEntityID()
    
EndProcedure

Procedure GetFreeMapID(Name.s)
    Protected AssignedID
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    
    If Select_Map(Name)
        AssignedID = FreeID
        
        If (Maps()\FreeID <> Maps()\NextID)
            Maps()\FreeID = Maps()\NextID  
        Else
            Maps()\FreeID = Maps()\FreeID + 1
            Maps()\NextID = Maps()\FreeID
        EndIf
        
        If Maps()\FreeID > 127 ; Map will be full with next user.
            Maps()\FreeID = -1
        EndIf
        
        UnlockMutex(MainMutex)
        ProcedureReturn AssignedID
    EndIf
    
    UnlockMutex(MainMutex)
EndProcedure

Procedure SetEntityPosition(X, Y, Z, R, L, Mapname.s)    
;     If (LCase(Mapname) <> LCase(networkclients()\Entity\CurrentMap))
;         MutexLock(MainMutex, #PB_Compiler_Procedure)
;         
;         If Select_Map(Mapname)
;             If Maps()\BaseMap\Rank_Join =< GetPlayerRank(NetworkClients()\Entity\Username)
;                 ; Allow to join map.
;             Else
;                 ChatToClient(NetworkClients()\Client_ID, "&eYou are not allowed to join this map.")
;             EndIf
;         EndIf
;         
;         UnlockMutex(MainMutex)
;     Else
;         NetworkClients()\Entity\X = X
;         NetworkClients()\Entity\Y = Y
;         NetworkClients()\Entity\Z = Z
;         NetworkClients()\Entity\Rot = R
;         NetworkClients()\Entity\Look = L
;     EndIf
EndProcedure

Procedure SendEntityToMap(ID, X.w, Y.w, Z.w, H, P)
    
    ForEach networkClients()
        If NetworkClients()\LoggedIn And NetworkClients()\Entity\CurrentMap + ".cw" = Maps()\Filename
            If NetworkClients()\Entity\ClientID = ID And NetworkClients()\Entity\SendOwn = #True
                Send_PlayerTeleport(255, X, Y, Z, H, P)
                NetworkClients()\Entity\SendOwn = #False
            ElseIf NetworkClients()\Entity\ClientID <> ID
                Send_PlayerTeleport(ID, X, Y, Z, H, P)
            EndIf
        EndIf
    Next
    
EndProcedure

Procedure SendEntityPositions()
    If Running = 0
        ProcedureReturn
    EndIf
    LockMutex(ClientMutex)
    
    ForEach NetworkClients()
        If NetworkCLients()\LoggedIn = #False
            Continue
        EndIf
        
        MutexLock(MainMutex, #PB_Compiler_Procedure)
        
        If Select_Map(NetworkClients()\Entity\CurrentMap)
            Maps()\LastClient = 0
            PushListPosition(NetworkClients())
            SendEntityToMap(NetworkClients()\Entity\ClientID, NetworkClients()\Entity\X, NetworkClients()\Entity\Y, NetworkClients()\Entity\Z, NetworkClients()\Entity\Rot, NetworkClients()\Entity\Look)
            PopListPosition(NetworkCLients())
        EndIf
        
        UnlockMutex(MainMutex)
    Next
    Delay(5)
    UnlockMutex(ClientMutex)
EndProcedure

Procedure AddEntity()
    Protected *TempClient.NetworkClient, *TempClient2.NetworkClient, myIndex.l
    
    NetworkClients()\Entity\ID = MainEntity\FreeID
    NetworkClients()\Entity\NameID = FreeID
    NetworkClients()\Entity\ClientID = GetFreeMapID(NetworkClients()\Entity\CurrentMap)
        
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    If Select_Map(NetworkClients()\Entity\CurrentMap)
        NetworkClients()\Entity\X = (Maps()\BaseMap\Spawn_X * 32)
        NetworkClients()\Entity\Y = (Maps()\BaseMap\Spawn_Y * 32)
        NetworkClients()\Entity\Z = (Maps()\BaseMap\Spawn_Z * 32) + 51
        NetworkClients()\Entity\Rot = Maps()\BaseMap\Spawn_Rot
        NetworkClients()\Entity\Look = Maps()\BaseMap\Spawn_Look
        NetworkCLients()\Entity\SendOwn = #True
        *TempClient = @NetworkClients()
        
        PushListPosition(NetworkClients())
        ForEach NetworkClients() ; Sends the new entity to all current players.
            
            If *TempClient\Client_ID = NetworkClients()\Client_ID
                ;Send_PlayerSpawn(NetworkClients()\Client_ID, 255, NetworkClients()\Entity\FormattedName, NetworkClients()\Entity\X, NetworkClients()\Entity\Y, NetworkClients()\Entity\Z, NetworkClients()\Entity\Rot, NetworkClients()\Entity\Look)
            Else
                Send_PlayerSpawn(*TempClient\Entity\ClientID, *TempClient\Entity\FormattedName, *TempClient\Entity\X * 32, *TempClient\Entity\Y * 32, *TempClient\Entity\Z * 32 + 51, *TempClient\Entity\Rot, *TempClient\Entity\Look)
                *TempClient2 = @NetworkClients()
                
                myIndex = ListIndex(NetworkClients())
                SelectClient(*TempClient\Client_ID)
                Send_PlayerSpawn(*TempClient2\Entity\ClientID, *TempClient2\Entity\FormattedName, *TempClient2\Entity\X * 32, *TempClient2\Entity\Y * 32, *TempClient2\Entity\Z * 32 + 51, *TempClient2\Entity\Rot, *TempClient2\Entity\Look)
                SelectElement(NetworkClients(), myIndex)
            EndIf
            
        Next
        PopListPosition(NetworkClients())
        
        ;SendExistingEntities(@TempClient) ; Sends all current players to the new entity.
        ;ClearStructure(*TempClient, NetworkClient)
    EndIf
    
    UnlockMutex(MainMutex)
    
    NetworkCLients()\Entity\BuildMaterial = -1
    NetworkClients()\Entity\HeldBlock = 1
    NetworkClients()\Entity\LastMaterial = 1
    NetworkClients()\Entity\Muted = GetPlayerMuted(NetworkClients()\Entity\Username)
    
    If (MainEntity\FreeID <> MainEntity\NextID) ; Sets up next available Entity ID
        MainEntity\FreeID = MainEntity\NextID  
    Else
        MainEntity\FreeID = MainEntity\FreeID + 1
        MainEntity\NextID = MainEntity\FreeID
    EndIf
    
    If (FreeID <> NextID) ; Sets up next available Name ID.
        FreeID = NextID
    Else
        FreeID + 1
        NextID = FreeID
    EndIf
    
EndProcedure

Procedure EntityChange()
EndProcedure

Procedure DeleteEntity()
    MainEntity\FreeID = NetworkClients()\Entity\ID
    FreeID = Networkclients()\Entity\NameID
    
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    
    If Select_Map(NetworkClients()\Entity\CurrentMap)
        Maps()\FreeID = NetworkClients()\Entity\ClientID
    EndIf
    Define TempID = NetworkClients()\Entity\ClientID
    
    PushListPosition(NetworkClients())
    
    ForEach NetworkClients()
        If networkClients()\Entity\CurrentMap + ".cw" = Maps()\Filename
            Send_PlayerDespawn(TempID)
        EndIf
        
    Next
    PopListPosition(NetworkClients())
    
    UnlockMutex(MainMutex)
    
    MapRemoveClient(NetworkClients()\Entity\CurrentMap)
EndProcedure

Procedure Entity_Main()
    If Running = 0
        ProcedureReturn
    EndIf
    LockMutex(ClientMutex)
    
    ForEach NetworkClients()
        If NetworkCLients()\LoggedIn = #True And NetworkClients()\Entity\CurrentMap = ""
            NetworkClients()\Entity\CurrentMap = SystemSettings\Main_World
            MapAddClient(NetworkClients()\Entity\CurrentMap)
            ;AddEntity()
            
            ;PushListPosition(NetworkCLients())
            ;MutexUnlock(ClientMutex)
            
            If NetworkClients()\Entity\ClientID <> -1
                MapSend(NetworkClients()\Entity\CurrentMap)
            Else
                Send_Disconnect("&cMap is full.")
                ;PopListPosition(NetworkClients())
                Continue
            EndIf
            
            ;MutexLock(ClientMutex, "Ent Main")
            ;PopListPosition(NetworkClients())
        EndIf
    Next
     
    UnlockMutex(ClientMutex)
EndProcedure

RegisterCore("Entity", 1000, #Null, #Null, @Entity_Main())
RegisterCore("Entity_Send", 1, #Null, #Null, @SendEntityPositions())
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 220
; FirstLine = 168
; Folding = --
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory