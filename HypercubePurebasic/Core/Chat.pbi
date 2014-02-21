; Hypercube Chat.pbi
; By Umby24
; Function: Provide functions for sending chat messages to clients, Maps, and the whole server.
; ###############################

Procedure ChatToAll(Maps.s, Message.s, Log=#False)
    Protected Lines, i, Text.s
    PushListPosition(NetworkClients())
    
    Message = ReplaceString(Message, "<br>", Chr(10))
    Message = String_Multiline(Message)
    Message = CleanseString(Message)
    Message = EmoteReplace(Message)
    
    Lines = CountString(Message, Chr(10))
    
    For i = 0 To Lines
        Text.s = StringField(Message, i + 1, Chr(10))
        Text.s = LSet(Text, 64, " ")
        
        If Text.s = LSet("", 64, " ") Or Text.s = ""
            Continue
        EndIf
        
        
        ForEach NetworkClients()
            If StringMatches(Right(Text, 1)) And NetworkCLients()\EmoteFix = #False
                Text + "."
            EndIf
            
            If NetworkClients()\Entity\CurrentMap = Maps Or Maps = ""
                If Log
                    _Log("Chat", Text, #PB_Compiler_Line, #PB_Compiler_Procedure)
                EndIf
                
                Send_ChatMessage(0, Text)
            EndIf
        Next

    Next
    
    PopListPosition(NetworkClients())
EndProcedure

Procedure ChatToClient(Client_ID, Message.s, Log=#False)
    Protected Lines, i, Text.s
    PushListPosition(NetworkClients())
    
    Message = ReplaceString(Message, "<br>", Chr(10))
    Message = String_Multiline(Message)
    Message = CleanseString(Message)
    Message = EmoteReplace(Message)
    
    Lines = CountString(Message, Chr(10))
    
    If SelectClient(Client_ID) = #False
        _Log("Error", "Unable to find client", #PB_Compiler_Line, #PB_Compiler_Procedure)
        PopListPosition(NetworkClients())
        ProcedureReturn
    EndIf
    
    For i = 1 To Lines
        Text.s = StringField(Message, i, Chr(10))
        
        If Log
            _Log("Chat", Text, #PB_Compiler_Line, #PB_Compiler_Procedure)
        EndIf
        
        Text.s = LSet(Text, 64, " ")
        
        If Text.s = LSet("", 64, " ") Or Text.s = ""
            Continue
        EndIf

        If StringMatches(Right(Text, 1)) And NetworkCLients()\EmoteFix = #False
            Text + "."
        EndIf
        
        Send_ChatMessage(0, Text)
        ;ClientWriteByte(13)
        ;ClientWriteByte(0)
        ;ClientWriteString(Text)

    Next
    
    PopListPosition(NetworkCLients())
EndProcedure

;-
Procedure ChatMessage_All(Entity_ID, Message.s)
    Protected ListPosition, i
    ListPosition = ListIndex(NetworkClients())

    If NetworkClients()\Entity\Muted < Date()
        Message = ReplaceString(Message, "%%", "§")     
    
        For i = 0 To 9
            Message = ReplaceString(Message, "%"+Str(i), "&"+Str(i))
        Next
        
        For i = 97 To 102
          Message = ReplaceString(Message, "%"+Chr(i), "&"+Chr(i))
        Next
        
        Message = ReplaceString(Message, "§", "%")
        Message = ReplaceString(Message, "<br>", Chr(10))
        Message = ReplaceString(Message, Chr(10), Chr(10)+"&c# " + NetworkClients()\Entity\FormattedName + "&f: ")

        _Log("Chat", "# "+Networkclients()\Entity\Username+": "+Message, #PB_Compiler_Line, #PB_Compiler_Procedure)
      
        Message = "&c# " + NetworkClients()\Entity\FormattedName + "&f: " + Message
        
        ChatToAll("", Message)
    Else
        ChatToClient(NetworkClients()\Client_ID, "&4Error:&f You are muted!")
    EndIf
    
    SelectElement(NetworkClients(), ListPosition)
EndProcedure

Procedure ChatMessage_Map(Entity_ID, Message.s)
    Protected ListPosition, i, Playermap.s
    ListPosition = ListIndex(NetworkClients())
    
    If NetworkClients()\Entity\Muted < Date()
        Message = ReplaceString(Message, "%%", "§")      
    
        For i = 0 To 9
            Message = ReplaceString(Message, "%"+Str(i), "&"+Str(i))
        Next
        
        For i = 97 To 102
          Message = ReplaceString(Message, "%"+Chr(i), "&"+Chr(i))
        Next
        
        Message = ReplaceString(Message, "§", "%")
        Message = ReplaceString(Message, "<br>", Chr(10))
        Message = ReplaceString(Message, Chr(10), Chr(10)+NetworkClients()\Entity\FormattedName + "&f: ")
        Playermap.s = NetworkClients()\Entity\CurrentMap
        
        _Log("Chat", "[" + Playermap + "] " + Networkclients()\Entity\Username+": "+Message, #PB_Compiler_Line, #PB_Compiler_Procedure)
      
        Message = NetworkClients()\Entity\FormattedName + "&f: " + Message
        
        ChatToAll(Playermap, Message)
    Else
        ChatToClient(NetworkClients()\Client_ID, "&4Error:&f You are muted!")
    EndIf

    SelectElement(NetworkClients(), ListPosition)
EndProcedure

Procedure ChatMessage_Entity(Entity_ID, PlayerName.s, Message.s)
    Protected ListPosition, i, Playermap.s, Found, Temp
    ListPosition = ListIndex(NetworkClients())

    If NetworkClients()\Entity\Muted < Date()
        Message = ReplaceString(Message, "%%", "§")    
    
        For i = 0 To 9
            Message = ReplaceString(Message, "%"+Str(i), "&"+Str(i))
        Next
        
        For i = 97 To 102
          Message = ReplaceString(Message, "%"+Chr(i), "&"+Chr(i))
        Next
        
        Message = ReplaceString(Message, "§", "%")
        Message = ReplaceString(Message, "<br>", Chr(10))
        Message = ReplaceString(Message, Chr(10), Chr(10)+"&cPM " + NetworkClients()\Entity\FormattedName + "&7: ")
      
        Message = "&cPM " +NetworkClients()\Entity\FormattedName + "&7: " + Message
        
        Temp = ListIndex(NetworkClients())
        Found = 0
        
        ForEach NetworkClients()
            If LCase(NetworkClients()\Entity\Username) = LCase(PlayerName)
                Found = 1
                ChatToClient(NetworkClients()\Client_ID, Message)
                Break
            EndIf
        Next
        
        SelectElement(NetworkClients(), Temp)
        
        If Found = 0
            ChatToClient(NetworkClients()\Client_ID, "&4Error:&f User '" + PlayerName + "' not found!")
        EndIf
        
    Else
        ChatToClient(NetworkClients()\Client_ID, "&4Error:&f You are muted!")
    EndIf
    
    SelectElement(NetworkClients(), ListPosition)
EndProcedure

; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 153
; FirstLine = 141
; Folding = -
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory