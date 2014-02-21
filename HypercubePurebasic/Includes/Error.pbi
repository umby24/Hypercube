; Hypercube Error Include
; By Umby24
; Function: Catches any errors that occur across the server.
; ##########################


Procedure HandleError()
    PrintN(ErrorMessage())
    PrintN(Str(ErrorAddress()))
    PrintN(Str(ErrorTargetAddress()))
    PrintN(Str(ErrorCode()))
    PrintN(Str(ErrorLine()))
    PrintN(ErrorFile())
EndProcedure

OnErrorCall(@HandleError())
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 12
; Folding = -
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory