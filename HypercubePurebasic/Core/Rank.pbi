; Hypercube Rank.pbi
; by Umby24
; Function: Provide loading of ranks, saving of ranks, adding ranks, and gets rank information such as colors, prefixes, ect.
; #################################

Global NewList Ranks.rank()



Procedure LoadRanks(Filename.s)
    If OpenPreferences(Filename)
    
        ClearList(Ranks())
    
        If ExaminePreferenceGroups()
            While NextPreferenceGroup()
                AddElement(Ranks())
                Ranks()\Rank = Val(PreferenceGroupName())
                Ranks()\Name = ReadPreferenceString("Name", "-")
                Ranks()\Op = ReadPreferenceLong("Op", 0)
                Ranks()\Prefix = ReadPreferenceString("Prefix", "")
                Ranks()\Suffix = ReadPreferenceString("Suffix", "")
            Wend
        EndIf
    
        _Log("Info", "Ranks loaded.", #PB_Compiler_Line, #PB_Compiler_Procedure)
        
        ClosePreferences()
    Else
        _Log("Error", "Failed to open ranks file.", #PB_Compiler_Line, #PB_Compiler_Procedure)
    EndIf
EndProcedure

Procedure SaveRanks(Filename.s)
    If CreatePreferences(Filename)
        ForEach Ranks()
            PreferenceGroup(Str(Ranks()\Rank))
            WritePreferenceString("Name", Ranks()\Name)
            WritePreferenceString("Prefix", Ranks()\Prefix)
            WritePreferenceString("Suffix", Ranks()\Suffix)
            WritePreferenceInteger("Op", Ranks()\Op)
        Next
        ClosePreferences()
        _Log("Info", "Ranks saved.", #PB_Compiler_Line, #PB_Compiler_Procedure)
    Else
        _Log("Error", "Failed to save ranks.", #PB_Compiler_Line, #PB_Compiler_Procedure)
    EndIf
EndProcedure

Procedure.s GetRankPrefix(Rank)
    Protected Last.l, LastIndex
    Last = -32780
    LastIndex = 0
    
    ForEach Ranks()
        If Rank >= Ranks()\Rank And Last < Ranks()\Rank
            Last = Ranks()\Rank
            LastIndex = ListIndex(Ranks())
        EndIf
    Next
    
    SelectElement(Ranks(), LastIndex)
    
    ProcedureReturn Ranks()\Prefix
EndProcedure

Procedure.s GetRankSuffix(Rank)
    Protected Last.l, LastIndex
    Last = -32780
    LastIndex = 0
    
    ForEach Ranks()
        If Rank >= Ranks()\Rank And Last < Ranks()\Rank
            Last = Ranks()\Rank
            LastIndex = ListIndex(Ranks())
        EndIf
    Next
    
    SelectElement(Ranks(), LastIndex)
    
    ProcedureReturn Ranks()\Suffix
EndProcedure

Procedure.s GetRankformattedName(Name.s, Rank)
    ProcedureReturn GetRankPrefix(Rank) + Name + GetRankSuffix(Rank)
EndProcedure

Procedure Rank_Init()
    LoadRanks("Settings\Ranks.txt")
EndProcedure

Procedure Rank_Main()
    Protected FileDate
    
    FileDate = GetFileDate("Settings\Ranks.txt", #PB_Date_Modified)
    
    If MainRank\File_Date_Last <> FileDate
        LoadRanks("Settings\Ranks.txt")
        MainRank\File_Date_Last = FileDate
    EndIf
EndProcedure

Procedure Rank_Shutdown()
    SaveRanks("Settings\Ranks.txt")
EndProcedure

RegisterCore("Ranks", 1000, @Rank_Init(), @Rank_Shutdown(), @Rank_Main())
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 93
; FirstLine = 49
; Folding = --
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory