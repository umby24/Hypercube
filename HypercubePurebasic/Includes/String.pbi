; Hypercube String
; by Umby24
; Function: Handles all string related cleansing and breaking.
; ######################

;-==== Defines
Global RegexId = CreateRegularExpression(#PB_Any, ",", #PB_RegularExpression_DotAll|#PB_RegularExpression_MultiLine)

;-==== Procedures
Procedure.s CleanseString(In.s)
    If IsRegularExpression(RegexId)
        ProcedureReturn ReplaceRegularExpression(RegexId, In, "*")
    EndIf
EndProcedure

Procedure StringMatches(In.s)
    If IsRegularExpression(RegexId)
        ProcedureReturn MatchRegularExpression(RegexId, In)
    EndIf
EndProcedure

;TODO: Remake this.
Procedure.s String_Multiline(Input.s) ; Teilt einen String in mehrere Zeilen auf / Split a string into multiple lines
    Protected Output_Message.s, Max_Length, i, k, j
    
  Output_Message = ""
  Max_Length = 65
  While Len(Input) > 0
    For i = 1 To Max_Length
      If Mid(Input, i, 1) = Chr(10)
        Output_Message + Left(Input, i)
        Input = Mid(Input, i+1)
        Break
      ElseIf Mid(Input, i, 1) = Chr(0)
        Output_Message + Left(Input, i-1)
        Input = Mid(Input, i)
        Break
      ElseIf i = Max_Length
        For k = i-5 To 2 Step -1
          If Mid(Input, k, 1) = Chr(32)
            Output_Message + Left(Input, k-1) + "&3>>"+Chr(10)+"&3>>"
            Max_Length = 65-4
            For j = k-1 To 1 Step -1
              If Mid(Input, j, 1) = "&"
                Output_Message + Mid(Input, j, 2)
                Max_Length - 2
                Break
              EndIf
            Next
            Input = Mid(Input, k+1)
            Break 2
          EndIf
        Next
        Output_Message + Left(Input, i-5) + "&3>>"+Chr(10)+"&3>>"
        Max_Length = 65-4
        For j = i-5 To 1 Step -1
          If Mid(Input, j, 1) = "&"
            Output_Message + Mid(Input, j, 2)
            Max_Length -2
            Break
          EndIf
        Next
        Input = Mid(Input, i-4)
        Break
      EndIf
    Next
  Wend
  ProcedureReturn Output_Message
EndProcedure
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 21
; FirstLine = 12
; Folding = -
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory