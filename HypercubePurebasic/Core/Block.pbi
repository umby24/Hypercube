; Hypercube Block Include
; by Umby24
; Function: Handles the loading, saving, creation and so on of blocks.
; ################################

Global MainBlock.Block_Main
Global Dim Block.Block(255)

Procedure LoadBlocks()
    Protected i
    
    If OpenPreferences(MainBlock\Filename)
        For i = 0 To 255
            PreferenceGroup(Str(i))
            Block(i)\ID = i
            Block(i)\OnClient = ReadPreferenceLong("OnClient", 49)
            Block(i)\Color = ReadPreferenceLong("Color", 0)
            Block(i)\CPELevel = ReadPreferenceLong("CPELevel", 0)
            Block(i)\CPEReplace = ReadPreferenceLong("CPEReplace", 0)
            Block(i)\DeleteRank = ReadPreferenceLong("DeleteRank", 0)
            Block(i)\PlaceRank = ReadPreferenceLong("PlaceRank", 0)
            Block(i)\Killer = ReadPreferenceLong("Kills", 0)
            Block(i)\Name = ReadPreferenceString("Name", "[Unknown]")
            Block(i)\Physics = ReadPreferenceLong("Physics", 0)
            Block(i)\PhysicsPlugin = ReadPreferenceString("PhysicsPlugin", "")
            Block(i)\ReplaceOnLoad = ReadPreferenceLong("ReplaceOnLoad", -1)
            Block(i)\Special = ReadPreferenceLong("Special", 0)
        Next
        ClosePreferences()
        
        MainBlock\Last_Modified = GetFileDate(MainBlock\Filename, #PB_Date_Modified)
        _Log("Info", "Blocks loaded.", #PB_Compiler_Line, #PB_Compiler_Procedure)
    Else
        PrintN("EHHHHH fail.")
    EndIf
EndProcedure

Procedure SaveBlocks()
    Protected i
    If CreatePreferences(MainBlock\Filename)
        For i = 0 To 255
            PreferenceGroup(Str(i))
            WritePreferenceString("Name", Block(i)\Name)
            WritePreferenceLong("OnClient", Block(i)\OnClient)
            WritePreferenceLong("PlaceRank", Block(i)\PlaceRank)
            WritePreferenceLong("DeleteRank", Block(i)\DeleteRank)
            WritePreferenceLong("Physics", Block(i)\Physics)
            WritePreferenceString("PhysicsPlugin", Block(i)\PhysicsPlugin)
            WritePreferenceLong("Kills", Block(i)\Killer)
            WritePreferenceLong("Color", Block(i)\Color)
            WritePreferenceLong("CPELevel", Block(i)\CPELevel)
            WritePreferenceLong("CPEReplace", Block(i)\CPEReplace)
            WritePreferenceLong("Special", Block(i)\Special)
            
            If Block(i)\ReplaceOnLoad <> -1
                WritePreferenceLong("ReplaceOnLoad", Block(i)\ReplaceOnLoad)
            EndIf
            
        Next
        ClosePreferences()
        MainBlock\Last_Modified = GetFileDate(MainBlock\Filename, #PB_Date_Modified)
        _Log("Info", "Blocks saved.", #PB_Compiler_Line, #PB_Compiler_Procedure)
    EndIf
EndProcedure

Procedure BlockInit()
    MainBlock\Filename = "Settings/Blocks.txt"
    LoadBlocks()
EndProcedure

Procedure BlockShutdown()
    SaveBlocks()
EndProcedure

Procedure BlockMain()
    If GetFileDate(MainBlock\Filename, #PB_Date_Modified) <> MainBlock\Last_Modified
        LoadBlocks()
    EndIf
EndProcedure

RegisterCore("Blocks", 1000, @BlockInit(), @BlockShutdown(), #Null)
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 80
; FirstLine = 25
; Folding = -
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory