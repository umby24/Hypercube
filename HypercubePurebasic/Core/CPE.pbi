; Hypercube Classic Protocol Extension
; By Umby24
; Function: Handle CPE Related functions for clients.
; #########################

#CPE_Extensions = 14

Procedure ClientCPEHandshake(*Handshake.PlayerIdentification)
    NetworkClients()\Entity\Username = *Handshake\Username
    NetworkCLients()\MPPass = *Handshake\Verification
    
    Send_ExtInfo("Hypercube Server", #CPE_Extensions)
    Send_ExtEntry("CustomBlocks", 1)
    Send_ExtEntry("EmoteFix", 1)
    Send_ExtEntry("HeldBlock", 1)
    Send_ExtEntry("ClickDistance", 1)
    Send_ExtEntry("ChangeModel", 1)
    Send_ExtEntry("ExtPlayerList", 1)
    Send_ExtEntry("EnvWeatherType", 1)
    Send_ExtEntry("EnvMapAppearance", 1)
    Send_ExtEntry("MessageTypes", 1)
    Send_ExtEntry("BlockPermissions", 1)
    Send_ExtEntry("TextHotKey", 1)
    Send_ExtEntry("HackControl", 1)
    Send_ExtEntry("SelectionCuboid", 1)
    Send_ExtEntry("EnvColors", 1)
EndProcedure

Procedure ClientCPEPackets(Client_ID)
    If NetworkClients()\CustomBlocks = #True
        Send_CustomBlockSupportLevel(1)
    Else
        Client_Login(#Null)
    EndIf
EndProcedure

Procedure.s EmoteReplace(Message.s)
  Message = ReplaceString(Message, "{:)}", Chr(1))
  Message = ReplaceString(Message, "{smile2}", Chr(2))
  Message = ReplaceString(Message, "{smile}", Chr(1))
  Message = ReplaceString(Message, "{<3}", Chr(3))
  Message = ReplaceString(Message, "{heart}", Chr(3))
  Message = ReplaceString(Message, "{diamond}",Chr(4))
  Message = ReplaceString(Message, "{club}", Chr(5))
  Message = ReplaceString(Message, "{spade}", Chr(6))
  Message = ReplaceString(Message, "{male}", Chr(11))
  Message = ReplaceString(Message, "{female}", Chr(12))
  Message = ReplaceString(Message, "{note}", Chr(13))
  Message = ReplaceString(Message, "{notes}", Chr(14))
  Message = ReplaceString(Message, "{sun}", Chr(15))
  Message = ReplaceString(Message, "{!!}", Chr(19))
  Message = ReplaceString(Message, "{P}", Chr(20))
  Message = ReplaceString(Message, "{S}", Chr(21))
  Message = ReplaceString(Message, "{*}", Chr(7))
  Message = ReplaceString(Message, "{hole}", Chr(8))
  Message = ReplaceString(Message, "{circle}", Chr(9))
  Message = ReplaceString(Message, "{-}", Chr(22))
  Message = ReplaceString(Message, "{L}", Chr(28))
  Message = ReplaceString(Message, "{house}", Chr(127))
  Message = ReplaceString(Message, "{caret}", Chr(94))
  Message = ReplaceString(Message, "{tilde}", Chr(126))
  Message = ReplaceString(Message, "{'}", Chr(180))
  Message = ReplaceString(Message, "{^}", Chr(24))
  Message = ReplaceString(Message, "{v}", Chr(25))
  Message = ReplaceString(Message, "{>}", Chr(26))
  Message = ReplaceString(Message, "{<}", Chr(27))
  Message = ReplaceString(Message, "{^^}", Chr(30))
  Message = ReplaceString(Message, "{vv}", Chr(31))
  Message = ReplaceString(Message, "{>>}", Chr(16))
  Message = ReplaceString(Message, "{<<}", Chr(17))
  Message = ReplaceString(Message, "{<>}", Chr(29))
  Message = ReplaceString(Message, "{updown}", Chr(18))
  Message = ReplaceString(Message, "{updown2}", Chr(23))
  ProcedureReturn Message
EndProcedure

; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 31
; FirstLine = 20
; Folding = -
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory