; Hypercube Heartbeat
; By umby24
; Function: Preform server heartbeat updates to Minecraft.net or Classicube.net. Also is used to verify user names.
; ##########################

;-==== Defines
Global HeartbeatThread

;-==== Procedures
Procedure.s CreateSalt()
    Protected Salt.s, i
    Salt = ""
    
    For i = 1 To 32
        Salt.s + Chr(65 + Random(25))
    Next
    
    ProcedureReturn Salt
EndProcedure

Procedure DoHeartbeatClassicube(*Dummy)
    While Running
        Protected Publicstring.s = ""
        
        If NetworkSettings\Public = #True
            Publicstring = "true"
        Else
            Publicstring = "false"
        EndIf
        
        If ReceiveHTTPFile("http://www.classicube.net/heartbeat.jsp?port="+Str(NetworkSettings\Port)+"&users="+Str(MainHeartbeat\Clients)+"&max="+Str(NetworkSettings\MaxPlayers)+"&name="+NetworkSettings\ServerName+"&public="+Publicstring+"&version="+Str(7)+"&salt="+MainHeartbeat\Salt, "ServerURL.txt")
            _Log("Info", "Heartbeat sent.", #PB_Compiler_Line, #PB_Compiler_Procedure)
        Else
            _Log("Info", "Heartbeat failed.", #PB_Compiler_Line, #PB_Compiler_Procedure)
        EndIf
        
        Delay(45000)
    Wend
EndProcedure

Procedure VerifyClientName(*Handshake.PlayerIdentification)
    If Networkclients()\IP = "127.0.0.1" Or NetworkSettings\WhiteList
        ProcedureReturn #True
    EndIf
    
    Protected FPointer.s = MainHeartbeat\Salt + *Handshake\Username
    Protected Fingerprint.s = MD5Fingerprint(@FPointer, Len(MainHeartbeat\Salt + *Handshake\Username))
    
    If Trim(LCase(Fingerprint)) = Trim(LCase(*Handshake\Verification))
        ProcedureReturn #True
    Else
        ProcedureReturn #False
    EndIf
EndProcedure

Procedure Heartbeat_Init()
    MainHeartbeat\Salt = CreateSalt()
    HeartbeatThread = CreateThread(@DoHeartbeatClassicube(), 0)
EndProcedure

Procedure Heartbeat_Shutdown()
    If IsThread(HeartbeatThread)
        KillThread(HeartbeatThread)
    EndIf
EndProcedure


RegisterCore("Heartbeat", 45000, @Heartbeat_Init(), @Heartbeat_Shutdown(), 0)
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 18
; Folding = -
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory