Global NewList CoreEvents.EventStruct()
Prototype.i Core_Function()

Procedure UnregisterCore(Name.s)
    ForEach CoreEvents()
        If LCase(CoreEvents()\ID) = LCase(Name)
            DeleteElement(CoreEvents())
            ProcedureReturn
        EndIf
    Next
EndProcedure

Procedure RegisterCore(Name.s, Timer.i, *InitFunction, *ShutdownFunction, *MainFunction)
    UnregisterCore(Name)
    
    If Not AddElement(CoreEvents())
        _Log("Error", "Failed to create a new Core Event (ID: " + Name + ")", #PB_Compiler_Line, #PB_Compiler_Procedure)
        ProcedureReturn
    EndIf
    
    CoreEvents()\ID = Name
    CoreEvents()\InitFunction = *InitFunction
    CoreEvents()\ShutdownFunciton = *ShutdownFunction
    CoreEvents()\MainFunction = *MainFunction
    CoreEvents()\Time = 0
    CoreEvents()\Timer = Timer
    
    _Log("Debug", "Created new Core object. " + Name, #PB_Compiler_Line, #PB_Compiler_Procedure)
EndProcedure

Procedure CoreLoop()
    Protected myfun.Core_Function
    
    ForEach CoreEvents()
        If (ElapsedMilliseconds() - CoreEvents()\Time) >= CoreEvents()\Timer
            If CoreEvents()\MainFunction
                myfun = CoreEvents()\MainFunction
                myfun()
                Delay(40)
            EndIf
            CoreEvents()\Time = ElapsedMilliseconds()
        EndIf
    Next
EndProcedure

Procedure CoreInit()
    Protected myfun.Core_Function
    
    ForEach CoreEvents()
        If CoreEvents()\InitFunction
            myfun = CoreEvents()\InitFunction
            myfun()
        EndIf
    Next
    
EndProcedure

Procedure CoreShutdown()
    Protected myfun.Core_Function
    
    ForEach CoreEvents()
        If CoreEvents()\ShutdownFunciton
            myfun = CoreEvents()\ShutdownFunciton
            myfun()
        EndIf
    Next
    
EndProcedure



; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 27
; Folding = -
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory