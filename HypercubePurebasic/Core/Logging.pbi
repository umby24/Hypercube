; Logging Include for Hypercube
; by Umby24
; Function: Provide Error and Server Logging to the console and to file.
; ###############################
;-==== Declarations

Global Log_Main.Log_Main

;-==== Procedures
Procedure Log_Init()
    Protected Incriment.l
    
    If FileSize("Log") = -1 ; Creates the Log directory if it doesn't exist.
        CreateDirectory("Log")
    EndIf
    
    Incriment = 0
    While 1
        If FileSize("Log\" + SystemSettings\LogFile + "_" + Str(Incriment) + ".txt") = -1
            Log_Main\Filename = SystemSettings\LogFile + "_" + Str(Incriment) + ".txt"
            Break
        EndIf
        
        Incriment = Incriment + 1
    Wend
    
    If SystemSettings\Logging = #True
        Log_Main\File_ID = OpenFile(#PB_Any, "Log\" + Log_Main\Filename)
        If Log_Main\File_ID = 0
            _Log("Warn", "Couldn't open log file, continuing without logging.", #PB_Compiler_Line, #PB_Compiler_Procedure)
            SystemSettings\Logging = #False
        EndIf
    EndIf
    
EndProcedure
        
Procedure Log_Shutdown()
    If IsFile(Log_Main\File_ID)
        CloseFile(Log_Main\File_ID)
    EndIf
EndProcedure

Procedure _Log(Type.s, Message.s, Line, Proc.s)
    Protected Result
    Select Type
        Case "Info"
            PrintN("[" + UCase(Type) + "] "+Message)
        Case "Warn"
            PrintN("[" + UCase(Type) + "] ["+Proc+"] "+LSet(Str(Line), 4, " ")+"| "+Message)
        Case "Error"
            PrintN("[" + UCase(Type) + "] ["+Proc+"] "+LSet(Str(Line), 4, " ")+"| "+Message)
        Case "Chat"
            PrintN("[" + UCase(Type) + "] "+Message)
        Case "Debug"
            PrintN("[" + UCase(Type) + "] ["+Proc+"] "+LSet(Str(Line), 4, " ")+"| "+Message)
    EndSelect
    
    If SystemSettings\Logging = #True
        If IsFile(Log_Main\File_ID)
            Result = WriteStringN(Log_Main\File_ID, "[" + UCase(Type) + "] [" + Proc + "] " + Str(Line) + "| " + Message)
            If Result = #False
                SystemSettings\Logging = #False
                CloseFile(Log_Main\File_ID)
                _Log("Warn", "Error writing to log file, turning off logging.", #PB_Compiler_Line, #PB_Compiler_Procedure)
            EndIf
        EndIf
    EndIf
    
EndProcedure

RegisterCore("Log", 1000, @Log_Init(), @Log_Shutdown(), #Null)
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 43
; FirstLine = 10
; Folding = 0
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory