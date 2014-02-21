; Hypercube Player Database
; By Umby24
; Function: Interact with sqlite player database to retreive and store user information.
; ################################################
;-==== Defines
Global PlayerDBName.s = "Database.s3db"

;-==== Init
UseSQLiteDatabase()

;-==== Procedures
Procedure DBInit()
    If FileSize("Settings/" + PlayerDBName) = -1
        CreateFile(1337, "Settings/" + PlayerDBName)
        
        If IsFile(1337)
            CloseFile(1337)
            OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
            DatabaseUpdate(0, "CREATE TABLE PlayerDB (Number INTEGER PRIMARY KEY, Name TEXT UNIQUE, Rank INTEGER, RankStep INTEGER, BoundBlock INTEGER, RankChangedBy TEXT, LoginCounter INTEGER, KickCounter INTEGER, Ontime INTEGER, LastOnline INTEGER, IP TEXT, Stopped INTEGER, StoppedBy TEXT, Banned INTEGER, Vanished INTEGER, BannedBy STRING, BannedUntil INTEGER, Global INTEGER, Time_Muted INTEGER, BanMessage TEXT, KickMessage TEXT, MuteMessage TEXT, RankMessage TEXT, StopMessage TEXT)")
            DatabaseUpdate(0, "CREATE TABLE RankDB (Number INTEGER PRIMARY KEY, Name TEXT UNIQUE, Prefix TEXT, Suffix TEXT, Next TEXT, RGroup TEXT, Points INTEGER, Op INTEGER)")
            CloseDatabase(0)
        EndIf
        
    EndIf
EndProcedure

Procedure AddPlayer(Name.s, IP.s, DefaultRank.s)
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    DatabaseUpdate(0, "INSERT INTO PlayerDB (Name, IP, Rank) VALUES ('" + Name + "', '" + IP + "', '" + DefaultRank + "')")
    CloseDatabase(0)
EndProcedure

Procedure GetPlayer(Name.s)
    Protected Count
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    Count = 0
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='"+Name+"'")
        While NextDatabaseRow(0)
            Count + 1
        Wend
    Else
        FinishDatabaseQuery(0)
        CloseDatabase(0)
        ProcedureReturn #False
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    If Count > 0
        ProcedureReturn #True
    Else
        ProcedureReturn #False
    EndIf
    
EndProcedure

Procedure CreateRank(Rankname.s, RankGroup.s, RankPrefix.s = "", RankSuffix.s = "", IsOp = 0, PointsInRank = 10, NextRank = "")
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    DatabaseUpdate(0, "INSERT INTO RankDB (Name, Prefix, Suffix, Next, RGroup, Points, Op) VALUES ('" + Rankname + "', '" + RankPrefix + "', '" + RankSuffix + "', '" + NextRank + "', '" + RankGroup + "', '" + Str(PointsInRank) + "', '" + Str(IsOp) + "')")
    CloseDatabase(0)
EndProcedure

Procedure ContainsRank(Name.s)
    Protected Count
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    Count = 0
    
    If DatabaseQuery(0, "SELECT * FROM RankDB WHERE Name='"+Name+"'")
        While NextDatabaseRow(0)
            Count + 1
        Wend
    Else
        FinishDatabaseQuery(0)
        CloseDatabase(0)
        ProcedureReturn #False
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    If Count > 0
        ProcedureReturn #True
    Else
        ProcedureReturn #False
    EndIf
EndProcedure

Procedure.l GetDatabaseInt(Name.s, Table.s, Field.s)
    Procedure Result.l = -1
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
        
    If DatabaseQuery(0, "SELECT * FROM " + Table + " WHERE " + Field + "='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            Result = GetDatabaseLong(0, 0)
        Wend
    EndIf
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn Result
EndProcedure

Procedure.s DatbaseStringGet(Name.s, Table.s, Field.s)
    Protected Result.s = ""
    
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
        
    If DatabaseQuery(0, "SELECT * FROM " + Table + " WHERE " + Field + "='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            Result = GetDatabaseLong(0, 0)
        Wend
    EndIf
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn Result
EndProcedure

;{ Get methods
Procedure.l GetPlayerNumber(Name.s)
    Protected PlayerNumber.l
    
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    PlayerNumber = -1
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            PlayerNumber = GetDatabaseLong(0, 0)
        Wend
    EndIf
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn PlayerNumber
EndProcedure

Procedure.l GetPlayerRank(Name.s)
    Protected PlayerRank.l
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    PlayerRank = -1
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            PlayerRank = GetDatabaseLong(0, 2)
        Wend
    EndIf
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn PlayerRank
EndProcedure

Procedure.l GetPlayerLogins(Name.s)
    Protected Logins.l
    
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    Logins = -1
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            Logins = GetDatabaseLong(0, 3)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn Logins
EndProcedure

Procedure.l GetPlayerKicks(Name.s)
    Protected Kicks.l
    
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    Kicks = -1
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            Kicks = GetDatabaseLong(0, 4)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn Kicks
EndProcedure

Procedure.f GetPlayerOntime(Name.s)
    Protected Ontime.f
    
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    Ontime = -1
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            Ontime = GetDatabaseFloat(0, 5)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn Ontime
EndProcedure

Procedure.s GetPlayerIP(Name.s)
    Protected IP.s
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    IP = ""
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            IP = GetDatabaseString(0, 6)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn IP
EndProcedure

Procedure IsPlayerStopped(Name.s)
    Protected Stopped
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    Stopped = #False
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            
            Stopped = GetDatabaseLong(0, 7)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn Stopped
EndProcedure

Procedure IsPlayerBanned(Name.s)
    Protected Banned
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    Banned = #False
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            PrintN(Str(DatabaseColumnType(0, 8)))
            Banned = GetDatabaseLong(0, 8)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn Banned
EndProcedure

Procedure.l GetPlayerMuted(Name.s)
    Protected Muted.l
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    Muted = 0
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            Muted = GetDatabaseLong(0, 9)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn Muted
EndProcedure

Procedure.s GetPlayerBanMessage(Name.s)
    Protected BanMessage.s
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    BanMessage = ""
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            BanMessage = GetDatabaseString(0, 10)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn BanMessage
EndProcedure

Procedure.s GetPlayerKickMessage(Name.s)
    Protected KickMessage.s
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    KickMessage = ""
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            KickMessage = GetDatabaseString(0, 11)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn KickMessage
EndProcedure

Procedure.s GetPlayerMuteMessage(Name.s)
    Protected MuteMessage.s
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    MuteMessage = ""
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            MuteMessage = GetDatabaseString(0, 12)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn MuteMessage
EndProcedure

Procedure.s GetPlayerRankMessage(Name.s)
    Protected Rankmessage.s
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    Rankmessage = ""
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            Rankmessage = GetDatabaseString(0, 13)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn Rankmessage
EndProcedure
Procedure.s GetPlayerStopMessage(Name.s)
    Protected StopMessage.s
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    StopMessage = ""
    
    If DatabaseQuery(0, "SELECT * FROM PlayerDB WHERE Name='" + Name + "' LIMIT 1")
        While NextDatabaseRow(0)
            StopMessage = GetDatabaseString(0, 14)
        Wend
    EndIf
    
    FinishDatabaseQuery(0)
    CloseDatabase(0)
    
    ProcedureReturn StopMessage
EndProcedure
;}

;{ Set Methods
Procedure SetPlayerRank(Name.s, Rank, RankMessage.s="")
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    DatabaseUpdate(0, "UPDATE PlayerDB SET Rank=" + Str(Rank) + " WHERE Name='" + Name + "'")
    DatabaseUpdate(0, "UPDATE PlayerDB SET RankMessage='" + RankMessage + "' WHERE Name='" + Name + "'")
    CloseDatabase(0)
EndProcedure

Procedure SetPlayerLogins(Name.s, Logins)
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    DatabaseUpdate(0, "UPDATE PlayerDB SET LoginCounter=" + Str(Logins) + " WHERE Name='" + Name + "'")
    CloseDatabase(0)
EndProcedure

Procedure SetPlayerKicks(Name.s, Kicks)
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    DatabaseUpdate(0, "UPDATE PlayerDB SET KickCounter=" + Str(Kicks) + " WHERE Name='" + Name + "'")
    CloseDatabase(0)
EndProcedure

Procedure SetPlayerOntime(Name.s, Ontime)
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    DatabaseUpdate(0, "UPDATE PlayerDB SET Ontime=" + Str(Ontime) + " WHERE Name='" + Name + "'")
    CloseDatabase(0)
EndProcedure

Procedure SetPlayerIP(Name.s, IP.s)
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    DatabaseUpdate(0, "UPDATE PlayerDB SET IP='" + IP + "' WHERE Name='" + Name + "'")
    CloseDatabase(0)
EndProcedure

Procedure SetPlayerStopped(Name.s, Stopped, Reason.s="")
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    If Stopped = #True
        DatabaseUpdate(0, "UPDATE PlayerDB SET Stopped=true WHERE Name='" + Name + "'")
        DatabaseUpdate(0, "UPDATE PlayerDB SET StopMessage='" + Reason + "' WHERE Name='" + Name + "'")
    Else
        DatabaseUpdate(0, "UPDATE PlayerDB SET Stopped=false WHERE Name='" + Name + "'")
    EndIf
    CloseDatabase(0)
EndProcedure

Procedure SetPlayerBanned(Name.s, Banned, Reason.s="")
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    If Banned = #True
        DatabaseUpdate(0, "UPDATE PlayerDB SET Banned=true WHERE Name='" + Name + "'")
        DatabaseUpdate(0, "UPDATE PlayerDB SET BanMessage='" + Reason + "' WHERE Name='" + Name + "'")
    Else
        DatabaseUpdate(0, "UPDATE PlayerDB SET Banned=false WHERE Name='" + Name + "'")
    EndIf
    CloseDatabase(0)
EndProcedure

Procedure SetPlayerMuted(Name.s, Duration=0, Reason.s="")
    OpenDatabase(0, "Settings/" + PlayerDBName, "", "", #PB_Database_SQLite)
    DatabaseUpdate(0, "UPDATE PlayerDB SET Time_Muted=" + Str(Duration) + " WHERE Name='" + Name + "'")
    DatabaseUpdate(0, "UPDATE PlayerDB SET MuteMessage='" + Reason + "' WHERE Name='" + Name + "'")
    CloseDatabase(0)
EndProcedure

;}

RegisterCore("Database", 1000, @DBInit(), #Null, #Null)
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 106
; FirstLine = 82
; Folding = ---f--
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory