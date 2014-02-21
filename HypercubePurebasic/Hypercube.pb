EnableExplicit
OpenConsole()

; Hypercube Classic Minecraft Server
; Written for efficiency by Umby24.
; ###################################
; ##############  Notes  ############
; # TODO: Handle all packets
; # TODO: Implement build modes
; # TODO: Implement an undo / redo queue.
; # TODO: Implement block player tracking
; # TODO: Implement commands
; # TODO: Implement Plugins
; # TODO: Fix PlayerDB Ban
; # TODO: Implement console commands
; # TODO: Move console thread procedure elsewhere
; # TODO: 
; #
;###################################
Global LockedName.s

XIncludeFile "Includes/NBT.pbi"
XIncludeFile "Includes/Structures.pbi"

;-==== Declares

IncludeFile "Headers.pbi"

Global MainHeartbeat.Heartbeat
Global MainEntity.EntityMain
Global MainRank.RankMain
Global NewList NetworkClients.NetworkClient()

Global Running = 1
Global ClientMutex = CreateMutex()
Global MainMutex = CreateMutex()
Global FreeID = 0
Global NextID = 0

;-==== Procedures
Procedure Console_Input(*Dummy)
    Protected Mystring.s
    
    Repeat
        Mystring = Input()
        
        If TryLockMutex(ClientMutex) = 0
            PrintN("Client locked")
            PrintN(LockedName)
        EndIf
        If TryLockMutex(MainMutex) = 0
            PrintN("Main locked.")
            PrintN(LockedName)
        EndIf
        
        If Mystring = "END"
            Running = 0
        EndIf
        
    ForEver
EndProcedure

Procedure MutexLock(Mutex, Name.s)
    LockedName = Name
    LockMutex(Mutex)
EndProcedure


;-==== Includes
XIncludeFile "Core/Events.pbi"

XIncludeFile "Includes/String.pbi"

XIncludeFile "Core/Settings.pbi"
XIncludeFile "Core/Logging.pbi"
XIncludeFile "Core/Rank.pbi"
XIncludeFile "Core/Block.pbi"

XIncludeFile "Includes/PlayerDB.pbi"

XIncludeFile "Networking/Packets.pbi"
XIncludeFile "Networking/Network_Main.pbi"

XIncludeFile "Core/CPE.pbi"
XIncludeFile "Includes/Map.pbi"
XIncludeFile "Includes/PlayerDB.pbi"

XIncludeFile "Core/Client.pbi"
XIncludeFile "Core/Entity.pbi"
XIncludeFile "Core/Heartbeat.pbi"
XIncludeFile "Core/Chat.pbi"

XIncludeFile "Includes/Error.pbi"

;-==== Code
_Log("Info", "Server Starting", 0, "")

;-==== Load section
CoreInit() ; Modules will register their needed init functions to run anything they need to instantiate.

_Log("Info", "Server Loaded.", 0, "")
CreateThread(@Console_Input(), 0) ; Seperate thread so you can input commands into the console.

While Running
    CoreLoop() ; Same with above, modules will register their core functions if they need anything run in a loop.
    Delay(10)
Wend

Running = 0
_Log("Info", "Server shutting down.", 0, "")
Delay(20) ; Give threads a few milliseconds to exit themselves..

CoreShutdown()
CloseConsole()
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 56
; FirstLine = 30
; Folding = -
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory