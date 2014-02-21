; Map include for Hypercube
; By Umby24
; Function: Provide map related functions including loading, saving, and sending. Conversion will be done elsewhere.
; Info: This uses the Classic World Format documented at http://wiki.vg/ClassicWorld_file_format. 
; Info: A Default Map converter will convert D3 Formatted maps.
; #########################

;-==== Defines
Global NewList Maps.HMap()
#MaximumMaploadedTime = 15000 ; In milliseconds. (Default: 15 seconds)

;-==== Procedures
Procedure GZip_Compress(*Output, Output_Len, *Input, Input_Len)
    Protected Stream.z_stream, Version.s, Err
    
    Stream\next_in = *Input
    Stream\avail_in = Input_Len
    Stream\next_out = *Output
    Stream\avail_out = Output_Len
    
    If Stream\avail_out <> Output_Len : ProcedureReturn -1 : EndIf
    
    Stream\zalloc = 0
    Stream\zfree = 0
    Stream\opaque = 0
    
    Version = #ZLIB_VERSION
    
    Err = deflateInit2_(@Stream, #Z_BEST_COMPRESSION, #Z_DEFLATED, 15+16, 9, #Z_FILTERED, @Version, SizeOf(z_stream))
    
    If Err <> #Z_OK : ProcedureReturn -1 : EndIf
    
    Err = deflate(@Stream, #Z_FINISH)
    If Err <> #Z_STREAM_END
        deflateEnd(@Stream)
        ProcedureReturn - 1
    EndIf
    
    Output_Len = Stream\total_out
    
    Err = deflateEnd(@Stream)
    If Err <> #Z_OK : ProcedureReturn -1 : EndIf
    
    ProcedureReturn Output_Len
EndProcedure

Procedure MapLoadSettings(Filename.s) ; Loads settings from a D3 Map file, and converts it to ClassicWorld format.
    
EndProcedure

Procedure Select_Map(Filename.s)
    ForEach Maps()
        If Maps()\Filename = Filename + ".cw" Or Maps()\Filename = Filename + ".cwu"
            ProcedureReturn #True
        EndIf
    Next
    
    ProcedureReturn #False
EndProcedure

Procedure MapAddClient(Name.s)
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    
    If Select_Map(Name)
        Maps()\Clients + 1
    EndIf
    
    UnlockMutex(MainMutex)
EndProcedure

Procedure MapRemoveClient(Name.s)
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    
    If Select_Map(Name)
        Maps()\Clients - 1
    EndIf
    
    UnlockMutex(MainMutex)
EndProcedure

Procedure MapGetBlockpointer(X, Y, Z) ;!! Map must already be selected!!
    ProcedureReturn Maps()\BaseMap\Data + (Z * Maps()\BaseMap\Size_Y + Y) * Maps()\BaseMap\Size_X + X
EndProcedure


Procedure.b Map_Get_BlockID(Name.s, X, Y, Z)
    Protected Result
    
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    Result = 0
    
    If Select_Map(Name)
        Result = PeekB(Maps()\BaseMap\Data + (Z * Maps()\BaseMap\Size_Y + Y) * Maps()\BaseMap\Size_X + X)
    Else
        _Log("Error", "Cannot find map " + Name + ".", #PB_Compiler_Line, #PB_Compiler_Procedure)
    EndIf
    
    UnlockMutex(MainMutex)
    ProcedureReturn Result
EndProcedure

Procedure MapBlockChange(PlayerNumber.l, X.w, Y.w, Z.w, Block, Send) ; MainMutex should already be locked, and the map already selected! If not the server may crash!
    Define *BlockPointer = MapGetBlockpointer(X, Y, Z)
    PrintN("MapBlockChange " + Str(Block))
    
    If PeekB(*BlockPointer) = Block
        ProcedureReturn
    Else
        PokeB(*BlockPointer, Block)
        Maps()\BaseMap\ModifiedTime = Date()
    EndIf
    
    ;TODO: Add physics
    ;TODO: Possibly add blockchange queue
    ;TODO: Add plugin events
    ;TODO: Add Undo.
    
    ; Client should be locked as well, so lets go ahead and send this to everyone..
    If Send ;Send = false can be used for map generation.
        PushListPosition(NetworkClients())
        ForEach NetworkClients()
            If NetworkClients()\Entity\CurrentMap + ".cw" = Maps()\Filename
                Send_SetBlock(X, Y, Z, Block)
                PrintN("Sent")
            EndIf
        Next
        PopListPosition(NetworkClients())
    EndIf
    
EndProcedure

Procedure MapBlockChangeClient(PlayerNumber.l, Name.s, X.w, Y.w, Z.w, Block, Mode) ; ClientMutex should already be locked!! If not the server may crash!
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    PrintN("MapBlockChangeClient")
    
    If Select_Map(Name)
        If X =< Maps()\BaseMap\Size_X And Y =< Maps()\BaseMap\Size_Y And Z =< Maps()\BaseMap\Size_Z And X >= 0 And Y >= 0 And Z >= 0 ; Make sure the block is within the bounds of the map.
            Define *MapBlock = MapGetBlockpointer(X, Y, Z)
            PrintN("Inside bounds")
            NetworkClients()\Entity\LastMaterial = Block
            ;PlayerRank.w = GetPlayerRank(NetworkClients()\Entity\Username)
            
            ;If IsPlayerStopped(NetworkClients()\Entity\Username)
                ;ChatToClient(NetworkClients()\Client_ID, "&eYou cannot build, you are stopped!") ; -- NOTE THAT THIS DOESN'T WORK..
            If Maps()\BaseMap\Rank_Build > 0
                ChatToClient(NetworkClients()\Client_ID, "&eYou are not high enough rank to build on this map.")
                PrintN("Not high enough rank")
            ElseIf Block(PeekB(*MapBlock))\PlaceRank > 0 And Mode = 1
                ChatToClient(NetworkClients()\Client_ID, "&eYou are not allowed to build this block type.")
                PrintN("Not allowed")
            ElseIf Block(PeekB(*MapBlock))\DeleteRank > 0 And Mode = 0
                ChatToClient(NetworkClients()\Client_ID, "&eYou are not allowed to break this block type.")
                PrintN("Can't break.")
            Else
                ; Actually Change the block.
                If Mode = 1
                    MapBlockChange(0, X, Y, Z, Block, 1)
                Else
                    MapBlockChange(0, X, Y, Z, 0, 1)
                EndIf
            EndIf
        Else
            PrintN("Out of bounds")
        EndIf
    EndIf
    
    UnlockMutex(MainMutex)
EndProcedure

;{ NBT ClassicWorld Handling
Procedure Handle_Tag(*Tag.NBT_Tag, Parent.s, *CWMap.CWMap)
    If Parent = "ClassicWorld"
        Select *Tag\Name
            Case "FormatVersion"
                *CWMap\FormatVersion = *Tag\Byte
            Case "Name"
                *CWMap\Name = *Tag\String
            Case "UUID"
                *CWMap\UUID = AllocateMemory(MemorySize(*Tag\Raw))
                CopyMemory(*Tag\Raw, *CWMap\UUID, MemorySize(*Tag\Raw))
            Case "X"
                *CWMap\Size_X = *Tag\Word
            Case "Y"
                *CWMap\Size_Z = *Tag\Word ; Flip Z and Y because of notch.
            Case "Z"
                *CWMap\Size_Y = *Tag\Word
            Case "TimeCreated"
                *CWMap\CreationTime = *Tag\Quad
            Case "LastAccessed"
                *CWMap\AccessTime = *Tag\Quad
            Case "LastModified"
                *CWMap\ModifiedTime = *Tag\Quad
            Case "BlockArray"
                *CWMap\Data = AllocateMemory(*Tag\Raw_Size)
                NBT_Tag_Get_Array(*Tag, *CWMap\Data, *Tag\Raw_Size)
                
                ;CopyMemory(*Tag\Raw, *CWMap\Data, MemorySize(*Tag\Raw))
        EndSelect
        
    ElseIf Parent = "CreatedBy"
        Select *Tag\Name
            Case "Service"
                *CWMap\ServiceName = *Tag\String
            Case "Username"
                *CWMap\ServiceUsername = *Tag\String
        EndSelect
    
    ElseIf Parent = "MapGenerator"
        Select *Tag\Name
            Case "Software"
                *CWMap\GeneratingSoftware = *Tag\String
            Case "MapGeneratorName"
                *CWMap\GeneratorName = *Tag\String
        EndSelect
        
    ElseIf Parent = "Spawn"
        Select *Tag\Name
            Case "X"
                *CWMap\Spawn_X = *Tag\Word
            Case "Y"
                *CWMap\Spawn_Z = *Tag\Word
            Case "Z"
                *CWMap\Spawn_Y = *Tag\Word
            Case "H"
                *CWMap\Spawn_Rot = *Tag\Byte
            Case "P"
                *CWMap\Spawn_Look = *Tag\Byte
        EndSelect
        
    ElseIf Parent = "Metadata"
        Select *Tag\Type
            Case #NBT_Tag_Compound
                ForEach *Tag\Child()
                    Handle_Tag(*Tag\Child(), *Tag\Name, *CWMap)
                Next
            Default
                *CWMap\ErrorString = "Incorrectly formatted Metadata"
                *CWMap\NoErrors = #False
        EndSelect
        
    ElseIf Parent = "CPE"
        If *Tag\Type = #NBT_Tag_Compound
            ForEach *Tag\Child()
                Handle_Tag(*Tag\Child(), *Tag\Name, *CWMap)
            Next
            
        Else
            *CWMap\ErrorString = "Incorrectly formatted CPE Metadata"
            *CWMap\NoErrors = #False
        EndIf
    ElseIf Parent = "ClickDistance"
        Select *Tag\Name
            Case "ExtensionVersion"
                *CWMap\ClickDistanceVersion = *Tag\Long
            Case "Distance"
                *CWMap\ClickDistance = *Tag\Word
        EndSelect
    ElseIf Parent = "CustomBlocks"
        Select *Tag\Name
            Case "ExtensionVersion"
                *CWMap\CustomBlocksVersion = *Tag\Long
            Case "SupportLevel"
                *CWMap\SupportLevel = *Tag\Word
            Case "Fallback"
                *CWMap\FallbackArray = AllocateMemory(256) ; Currently fixed at 256 bytes.
                CopyMemory(*Tag\Raw, *CWMap\FallbackArray, 256)
        EndSelect
        
    ElseIf Parent = "EnvColors"
        Select *Tag\Type
            Case #NBT_Tag_Compound
                ForEach *Tag\Child()
                    Handle_Tag(*Tag\Child(), *Tag\Name, *CWMap)
                Next
            Case #NBT_Tag_Long
                *CWMap\EnvColorsVersion = *Tag\Long
        EndSelect
        
    ElseIf Parent = "EnvMapAppearance"
        Select *Tag\Name
            Case "ExtensionVersion"
                *CWMap\EnvMapAppearanceVersion = *Tag\Long
            Case "TextureURL"
                *CWMap\TextureURL = *Tag\String
            Case "SideBlock"
                *CWMap\SideBlock = *Tag\Byte
            Case "EdgeBlock"
                *CWMap\EdgeBlock = *Tag\Byte
            Case "SideLevel"
                *CWMap\SideLevel = *Tag\Word
        EndSelect
    ElseIf Parent = "Sky"
    ElseIf Parent = "Cloud"
    ElseIf Parent = "Fog"
    ElseIf Parent = "Ambient"
    ElseIf parent = "Sunlight"
    ElseIf parent = "Hypercube"
        Select *Tag\Name
            Case "BuildRank"
                *CWMap\Rank_Build = *Tag\Word
            Case "ShowRank"
                *CWMap\Rank_Show = *Tag\Word
            Case "JoinRank"
                *CWMap\Rank_Join = *Tag\Word
            Case "Physics"
                *CWMap\Physics = *Tag\Byte
            Case "Building"
                *CWMap\Building = *Tag\Byte
            Case "MOTD"
                *CWMap\MOTD = *Tag\String
        EndSelect
        
    Else
        PrintN("Unknown tag " + *Tag\Name + " Parent: " + Parent)
        
;         If *CWMap\Metadata ; Attempt to store unknown metadata.
;             *TempBuff = AllocateMemory(MemorySize(*CWMap\Metadata) + MemorySize(*Tag\Raw))
;             CopyMemory(*CWMap\Metadata, *TempBuff, MemorySize(*CWMap\Metadata))
;             CopyMemory(*Tag\Raw, *TempBuff + MemorySize(*CWMap\Metadata), MemorySize(*Tag\Raw))
;             ReAllocateMemory(*CWMap\Metadata, MemorySize(*TempBuff))
;             CopyMemory(*TempBuff, *CWMap\Metadata, MemorySize(*TempBuff))
;             
;             FreeMemory(*TempBuff)
;         Else
;             *CWMap\Metadata = AllocateMemory(MemorySize(*Tag\Raw))
;             CopyMemory(*tag\Raw, *CWMap\Metadata, MemorySize(*Tag\Raw))
;         EndIf
        
    EndIf
    
    ProcedureReturn *CWMap
EndProcedure

Procedure SerializedMapLoad(*Tag.NBT_Tag, *Map_To_Load.CWMap, Parent.s="")
    If Parent = "" ;Initial map load, make sure no errors are set.
        *Map_To_Load\NoErrors = #True
    EndIf
    
    If Not *Tag
        *Map_To_Load\NoErrors = #False
        *Map_To_Load\ErrorString = "Invalid Element provided"
        ProcedureReturn *Map_To_Load
    EndIf
    
    Select *Tag\Type
        Case #NBT_Tag_End
            *Map_To_Load\NoErrors = #False
            *Map_To_Load\ErrorString = "NBT Library exception (Reached Tag_End)"
        Case #NBT_Tag_Byte
            *Map_To_Load = Handle_Tag(*Tag, Parent, *Map_To_Load)
        Case #NBT_Tag_Word
            *Map_To_Load = Handle_Tag(*Tag, Parent, *Map_To_Load)
        Case #NBT_Tag_Long
            *Map_To_Load = Handle_Tag(*Tag, Parent, *Map_To_Load)
        Case #NBT_Tag_Quad
            *Map_To_Load = Handle_Tag(*Tag, Parent, *Map_To_Load)
        Case #NBT_Tag_Float
            *Map_To_Load = Handle_Tag(*Tag, Parent, *Map_To_Load)
        Case #NBT_Tag_Double
            *Map_To_Load = Handle_Tag(*Tag, Parent, *Map_To_Load)
        Case #NBT_Tag_Byte_Array
            *Map_To_Load = Handle_Tag(*Tag, Parent, *Map_To_Load)
        Case #NBT_Tag_String
            *Map_To_Load = Handle_Tag(*Tag, Parent, *Map_To_Load)
        Case #NBT_Tag_List
            *Map_To_Load = Handle_Tag(*Tag, Parent, *Map_To_Load)
        Case #NBT_Tag_Compound
            ForEach *Tag\Child()
                SerializedMapLoad(*Tag\Child(), *Map_To_Load, *Tag\Name)
            Next
        Default
            *Map_To_Load\ErrorString = "Map file uses incorrect NBT formatting."
            *Map_To_Load\NoErrors = #False
    EndSelect
    
    ProcedureReturn *Map_To_Load
EndProcedure
;}

Procedure NewMapLoad(Filename.s) ; Loads a previously never-loaded map.
    Protected NBT_Errors.s, Test.CWMap, *Temp_Map.CWMap
    
    If FileSize("Maps/" + Filename) = -1
        _Log("Warn", "Map file not found: " + Filename, #PB_Compiler_Line, #PB_Compiler_Procedure)
        ProcedureReturn
    EndIf
    
    Define *NBTMap.NBT_Element = NBT_Read_File("Maps/" + Filename)
    
    NBT_Errors = NBT_Error_Get()
    
    If NBT_Errors <> ""
        _Log("Warn", "Unable to load map " + Filename + ": " + NBT_Errors, #PB_Compiler_Line, #PB_Compiler_Procedure)
        ProcedureReturn
    EndIf

    *Temp_Map = @Test
    
    SerializedMapLoad(*NBTMap\NBT_Tag, *Temp_Map)
    
    If *Temp_Map\NoErrors = #False
        _Log("Warn", "Unable to load map " + Filename + ": " + *Temp_Map\ErrorString, #PB_Compiler_Line, #PB_Compiler_Procedure)
        ProcedureReturn
    EndIf
    
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    
    AddElement(Maps())
    
    Maps()\BaseMap = Test
    Maps()\Loaded = #True
    Maps()\Filename = Filename
    Maps()\BaseMap\AccessTime = Date()
    
    UnlockMutex(MainMutex)
    
    _Log("Info", "Map " + Filename + " loaded. (X=" + Str(*Temp_Map\Size_X) + " Y=" + Str(*Temp_Map\Size_Y) + " Z=" + Str(*Temp_Map\Size_Z) + ")", 0, "")
    
    ClearStructure(*Temp_Map, CWMap) ; Memory cleanup.
    NBT_Element_Delete(*NBTMap)
    NBT_Error_Get()
EndProcedure

Procedure NewUnloadedMap(Filename.s)
    Protected NBT_Errors.s, Test.CWMap, *Temp_Map.CWMap
    
    If FileSize("Maps/" + Filename) = -1
        _Log("Warn", "Map file not found: " + Filename, #PB_Compiler_Line, #PB_Compiler_Procedure)
        ProcedureReturn
    EndIf
    
    Define *NBTMap.NBT_Element = NBT_Read_File("Maps/" + Filename)
    
    NBT_Errors = NBT_Error_Get()
    
    If NBT_Errors <> ""
        _Log("Warn", "Unable to load map " + Filename + ": " + NBT_Errors, #PB_Compiler_Line, #PB_Compiler_Procedure)
        ProcedureReturn
    EndIf
    
    *Temp_Map = @Test
    
    SerializedMapLoad(*NBTMap\NBT_Tag, *Temp_Map)
    
    If *Temp_Map\NoErrors = #False
        _Log("Warn", "Unable to load map " + Filename + ": " + *Temp_Map\ErrorString, #PB_Compiler_Line, #PB_Compiler_Procedure)
        ProcedureReturn
    EndIf
    
    FreeMemory(Test\Data) ; Unload the map data for conservation of ram.
    
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    
    AddElement(Maps())
    Maps()\BaseMap = Test
    Maps()\Loaded = #False
    Maps()\Filename = Filename
    Maps()\BaseMap\AccessTime = Date()
    UnlockMutex(MainMutex)
    
    _Log("Info", "Map " + Filename + " loaded. (X=" + Str(*Temp_Map\Size_X) + " Y=" + Str(*Temp_Map\Size_Y) + " Z=" + Str(*Temp_Map\Size_Z) + ")", 0, "")
    
    ClearStructure(*Temp_Map, CWMap) ; Memory cleanup.
    NBT_Element_Delete(*NBTMap)
    NBT_Error_Get()
EndProcedure

Procedure MapLoad(*Map.HMap)
    Protected NBT_Errors.s, Test.CWMap, *Temp_Map.CWMap
    
    If FileSize("Maps/" + *Map\Filename) = -1
        _Log("Warn", "Map file not found: " + *Map\Filename, #PB_Compiler_Line, #PB_Compiler_Procedure)
        ProcedureReturn
    EndIf
    
    Define *NBTMap.NBT_Element = NBT_Read_File("Maps/" + *Map\Filename)
    
    NBT_Errors.s = NBT_Error_Get()
    
    If NBT_Errors <> ""
        _Log("Warn", "Unable to load map " + *Map\Filename + ": " + NBT_Errors, #PB_Compiler_Line, #PB_Compiler_Procedure)
        ProcedureReturn
    EndIf
    
    *Temp_Map = @Test
    SerializedMapLoad(*NBTMap\NBT_Tag, *Temp_Map)
    
    If *Temp_Map\NoErrors = #False
        _Log("Warn", "Unable to load map " + *Map\Filename + ": " + *Temp_Map\ErrorString, #PB_Compiler_Line, #PB_Compiler_Procedure)
        ProcedureReturn
    EndIf
    
    *Map\BaseMap\Data = AllocateMemory(*Map\BaseMap\Size_X * *Map\BaseMap\Size_Y * *Map\BaseMap\Size_Z)
    CopyMemory(*Temp_Map\Data, *Map\BaseMap\Data, MemorySize(*Map\BaseMap\Data))
    
    If *Temp_Map\Metadata
        *Map\BaseMap\Metadata = AllocateMemory(MemorySize(*Temp_Map\Metadata))
        CopyMemory(*Temp_Map\Metadata, *Map\BaseMap\Metadata, MemorySize(*Map\BaseMap\Metadata))
    EndIf
    
    *Map\BaseMap\AccessTime = Date()
    
    *Map\Loaded = #True
    _Log("Info", "Map " + *Map\Filename + " reloaded.", 0, "")
    
    FreeMemory(*Temp_Map\Data)
    
    If *Temp_Map\Metadata
        FreeMemory(*Temp_Map\Metadata)
    EndIf
    
    If Right(*Map\Filename, 1) = "u"
        *Map\Filename = Left(*Map\Filename, Len(*Map\Filename) - 1)
    EndIf
    
    ClearStructure(*Temp_Map, CWMap)
    NBT_Element_Delete(*NBTMap)
EndProcedure

Procedure MapSave(*Map.HMap, Unloading=#False)
    If *Map\Loaded = #False
        ProcedureReturn 0 ; If the map is unloaded, then it shouldn't need to be saved, it will already be written.
    EndIf
    
    Protected *NBT_Map.NBT_Element = NBT_Element_Add()
    NBT_Tag_Set_Name(*NBT_Map\NBT_Tag, "ClassicWorld")
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "FormatVersion", #NBT_Tag_Byte)
    *NBT_Map\NBT_Tag\Child()\Byte = 1
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "Name", #NBT_Tag_String)
    *NBT_Map\NBT_Tag\Child()\String = *Map\BaseMap\Name
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "UUID", #NBT_Tag_Byte_Array)
    NBT_Tag_Set_Array(*NBT_Map\NBT_Tag\Child(), *map\BaseMap\UUID, MemorySize(*map\BaseMap\UUID))
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "X", #NBT_Tag_Word)                              ; Map size
    *NBT_Map\NBT_Tag\Child()\Word = *Map\BaseMap\Size_X
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "Y", #NBT_Tag_Word)
    *NBT_Map\NBT_Tag\Child()\Word = *Map\BaseMap\Size_Z ; Reverse for compatibility.
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "Z", #NBT_Tag_Word)
    *NBT_Map\NBT_Tag\Child()\Word = *Map\BaseMap\Size_Y
    
    If *Map\BaseMap\ServiceName <> ""                                               ; If Applicable, copy in the creation service and name.
        NBT_Tag_Add(*NBT_Map\NBT_Tag, "CreatedBy", #NBT_Tag_Compound)
        NBT_Tag_Add(*NBT_Map\NBT_Tag\Child(), "Service", #NBT_Tag_String)
        NBT_Tag_Set_String(*NBT_Map\NBT_Tag\Child()\Child(), *Map\BaseMap\ServiceName)
        
        NBT_Tag_Add(*NBT_Map\NBT_Tag\Child(), "Username", #NBT_Tag_String)
        NBT_Tag_Set_String(*NBT_Map\NBT_Tag\Child()\Child(), *Map\BaseMap\ServiceUsername)
    EndIf
    
    If *Map\BaseMap\GeneratingSoftware <> ""                                         ; If applicable, copy in map generator information
        NBT_Tag_Add(*NBT_Map\NBT_Tag, "MapGenerator", #NBT_Tag_Compound)
        NBT_Tag_Add(*NBT_Map\NBT_Tag\Child(), "Software", #NBT_Tag_String)
        NBT_Tag_Set_String(*NBT_Map\NBT_Tag\Child()\Child(), *Map\BaseMap\GeneratingSoftware)
        
        NBT_Tag_Add(*NBT_Map\NBT_Tag\Child(), "MapGeneratorName", #NBT_Tag_String)
        NBT_Tag_Set_String(*NBT_Map\NBT_Tag\Child()\Child(), *Map\BaseMap\GeneratorName)
    EndIf
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "TimeCreated", #NBT_Tag_Quad)                    ; Copy in the access times.
    *NBT_Map\NBT_Tag\Child()\Double = *Map\BaseMap\CreationTime
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "LastAccessed", #NBT_Tag_Quad)
    *NBT_Map\NBT_Tag\Child()\Double = *Map\BaseMap\AccessTime
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "LastModified", #NBT_Tag_Quad)
    *NBT_Map\NBT_Tag\Child()\Double = *Map\BaseMap\ModifiedTime
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "Spawn", #NBT_Tag_Compound)                         ; Copy the spawn position in
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child(), "X", #NBT_Tag_Word)
    *NBT_Map\NBT_Tag\Child()\Child()\Word = *Map\BaseMap\Spawn_X
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child(), "Y", #NBT_Tag_Word)
    *NBT_Map\NBT_Tag\Child()\Child()\Word = *Map\BaseMap\Spawn_Z
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child(), "Z", #NBT_Tag_Word)
    *NBT_Map\NBT_Tag\Child()\Child()\Word = *Map\BaseMap\Spawn_Y
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child(), "H", #NBT_Tag_Byte)
    *NBT_Map\NBT_Tag\Child()\Child()\Word = *Map\BaseMap\Spawn_Rot
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child(), "P", #NBT_Tag_Byte)
    *NBT_Map\NBT_Tag\Child()\Child()\Word = *Map\BaseMap\Spawn_Look
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "BlockArray", #NBT_Tag_Byte_Array)                ; Copy map blocks.
    NBT_Tag_Set_Array(*NBT_Map\NBT_Tag\Child(), *Map\BaseMap\Data, MemorySize(*Map\BaseMap\Data))
    
    
    ; Metadata copying.. D:
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag, "Metadata", #NBT_Tag_Compound)
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child(), "Hypercube", #NBT_Tag_Compound)
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child()\Child(), "BuildRank", #NBT_Tag_Word)
    *NBT_Map\NBT_Tag\Child()\Child()\Word = *Map\BaseMap\Rank_Build
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child()\Child(), "ShowRank", #NBT_Tag_Word)
    *NBT_Map\NBT_Tag\Child()\Child()\Word = *Map\BaseMap\Rank_Show
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child()\Child(), "JoinRank", #NBT_Tag_Word)
    *NBT_Map\NBT_Tag\Child()\Child()\Word = *Map\BaseMap\Rank_Join
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child()\Child(), "Physics", #NBT_Tag_Byte)
    *NBT_Map\NBT_Tag\Child()\Child()\Byte = *Map\BaseMap\Physics
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child()\Child(), "Building", #NBT_Tag_Byte)
    *NBT_Map\NBT_Tag\Child()\Child()\Byte = *Map\BaseMap\Building
    
    NBT_Tag_Add(*NBT_Map\NBT_Tag\Child()\Child(), "MOTD", #NBT_Tag_String)
    *NBT_Map\NBT_Tag\Child()\Child()\String = *Map\BaseMap\MOTD
    
    If Unloading = #False
        NBT_Write_File(*NBT_Map, "Maps/" + ReplaceString(*Map\Filename, ".cwu", ".cw"), #True) ; If the map is loaded, make sure to save it as a loaded map.
    Else
        If Right(*Map\Filename, 2) = "cw" ; If the map is unloaded, save it with the "cwu" (ClassicWorld Unloaded) Extension.
            NBT_Write_File(*NBT_Map, "Maps/" + *Map\Filename + "u", #True)
            DeleteFile("Maps/" + *Map\Filename)
        Else
            NBT_Write_File(*NBT_Map, "Maps/" + *Map\Filename, #True)
        EndIf
    EndIf
    
    _Log("Info", "Map saved. " + *Map\Filename, 0, "")
    
    NBT_Element_Delete(*NBT_Map) ; Memory cleanup
    NBT_Error_Get()
EndProcedure

Procedure MapUnload(*Map.HMap) ; Unloads a map but keeps its settings in memory.
    MapSave(*Map, #True)
    FreeMemory(*Map\BaseMap\Data)
    
    If *Map\BaseMap\Metadata
        FreeMemory(*Map\BaseMap\Metadata)
    EndIf
    
    *Map\Loaded = #False
    _Log("Info", "Map " + *Map\Filename + " conserved.", 0, "")
    
    If Right(*Map\Filename, 2) = "cw"
        *Map\Filename = *map\Filename + "u"
    EndIf
    
EndProcedure

Procedure MapSend(Name.s)
    Protected MapSize.l, *TempBuffer, TempBufferOffset.l, TempBlock.Block, *TempBuffer2, CompressedSize.l, Result, Bytes_2_Send, Bytes_In_Block, Bytes_Sent, i
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    
    If Select_Map(Name) = #False
        _Log("Error", "Couldn't find map " + Name, #PB_Compiler_Line, #PB_Compiler_Procedure)
        UnlockMutex(MainMutex)
        ProcedureReturn
    EndIf
    
    If Maps()\Loaded = #False
        MapLoad(@Maps())
    EndIf
    
    Send_ServerIdentification(0)
    
    MapSize = Maps()\BaseMap\Size_X * Maps()\BaseMap\Size_Y * Maps()\BaseMap\Size_Z
    *TempBuffer = AllocateMemory(MapSize + 4)
    TempBufferOffset = 0
    
    PokeL(*TempBuffer, Endian(MapSize))
    
    TempBufferOffset + 4
    
    For i = 0 To MapSize - 1
        TempBlock = Block(PeekB(Maps()\BaseMap\Data + i))
        
        If TempBlock\CPELevel > NetworkClients()\CustomBlocks_Level
            PokeB(*TempBuffer + TempBufferOffset, TempBlock\CPEReplace) : TempBufferOffset + 1
        Else
            PokeB(*TempBuffer + TempBufferOffset, TempBlock\OnClient) : TempBufferOffset + 1
        EndIf
    Next
    
    UnlockMutex(MainMutex)
    
    *TempBuffer2 = AllocateMemory(compressBound(TempBufferOffset) + 1024 + 512)
    
    CompressedSize.l = MemorySize(*TempBuffer2)
    
    Result = GZip_Compress(*TempBuffer2, CompressedSize, *TempBuffer, TempBufferOffset)
    
    If Result <> -1
        CompressedSize = Result
        FreeMemory(*TempBuffer)
        CompressedSize + (1024 - (CompressedSize % 1024))
        
        Send_LevelInit()
        
        Bytes_2_Send = CompressedSize
        Bytes_Sent = 0
        
        While Bytes_2_Send > 0
            Bytes_In_Block = Bytes_2_Send
            If Bytes_In_Block > 1024 : Bytes_In_Block = 1024 : EndIf
            Send_LevelChunk(Bytes_In_Block, *TempBuffer2 + Bytes_Sent, Bytes_Sent * 100 / CompressedSize)
            Bytes_Sent + Bytes_In_Block
            Bytes_2_Send - Bytes_In_Block
        Wend
        
        Send_LevelFinalize(Maps()\BaseMap\Size_X, Maps()\BaseMap\Size_Z, Maps()\BaseMap\Size_Y)
    Else
        _Log("Warn", "Failed to compress map.", #PB_Compiler_Line, #PB_Compiler_Procedure)
        FreeMemory(*TempBuffer)
    EndIf
    
    FreeMemory(*TempBuffer2)
EndProcedure

Procedure MapCreate(Name.s, X.w, Y.w, Z.w)
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    
    AddElement(Maps())
    Maps()\Filename = Name + ".cw"
    Maps()\Loaded = #True
    Maps()\BaseMap\AccessTime = Date()
    Maps()\BaseMap\CreationTime = Date()
    Maps()\BaseMap\ModifiedTime = Date()
    Maps()\BaseMap\Data = AllocateMemory(X * Y * Z)
    Maps()\BaseMap\Size_X = X
    Maps()\BaseMap\Size_Y = Y
    Maps()\BaseMap\Size_Z = Z
    Maps()\BaseMap\FormatVersion = 1
    Maps()\BaseMap\GeneratingSoftware = "Hypercube"
    Maps()\BaseMap\GeneratorName = ""
    Maps()\BaseMap\UUID = AllocateMemory(16)
    PokeQ(Maps()\BaseMap\UUID, Random(927301297381))
    PokeQ(Maps()\BaseMap\UUID + 8, Random(927301297381))
    Maps()\BaseMap\Spawn_X = X / 2
    Maps()\BaseMap\Spawn_Y = Y / 2
    Maps()\BaseMap\Spawn_Z = Z / 2
    Maps()\BaseMap\ServiceName = "Classicube"
    Maps()\BaseMap\ServiceUsername = "Hypercube"
    Maps()\BaseMap\Name = Name
    
    UnlockMutex(MainMutex)
    MapSave(Maps())
EndProcedure

Procedure MapInit()
    Protected Directory_ID, EntryName.s
    
    If FileSize("Maps") = -1
        CreateDirectory("Maps")
    EndIf
    
    If FileSize("Maps/" + SystemSettings\Main_World + ".cw") = -1 And FileSize("Maps/" + SystemSettings\Main_World + ".cwu") = -1
        _Log("Info", "Main map file not found, Creating new 128x128x128 map.", #PB_Compiler_Line, #PB_Compiler_Procedure)
        MapCreate(SystemSettings\Main_World, 128, 128, 128)
    EndIf
    
    Directory_ID = ExamineDirectory(#PB_Any, "Maps", "")
    
    If Directory_ID
        While NextDirectoryEntry(Directory_ID)
            If DirectoryEntryType(Directory_ID) = #PB_DirectoryEntry_File
                EntryName = DirectoryEntryName(Directory_ID)
                
                If LCase(Right(EntryName, 3)) = ".cw"
                    NewMapLoad(EntryName)
                EndIf
                
                If LCase(Right(EntryName, 4)) = ".cwu"
                    NewUnloadedMap(EntryName)
                EndIf
            EndIf
        Wend
    EndIf
    
    FinishDirectory(Directory_ID)
EndProcedure

Procedure MapMain()
    If Running = 0
        ProcedureReturn
    EndIf
    
    Protected Directory_ID, EntryName.s, Found
    Directory_ID = ExamineDirectory(#PB_Any, "Maps", "") ; Loads any new maps.
    
    If Directory_ID
        While NextDirectoryEntry(Directory_ID)
            If DirectoryEntryType(Directory_ID) = #PB_DirectoryEntry_File
                EntryName = DirectoryEntryName(Directory_ID)
                
                If LCase(Right(EntryName, 3)) = ".cw"
                    Found = 0
                    MutexLock(MainMutex, #PB_Compiler_Procedure)
                    ForEach Maps()
                        If Maps()\Filename = EntryName
                            Found = 1
                            Break
                        EndIf
                    Next
                    UnlockMutex(MainMutex)
                    If Found = 0
                        NewMapLoad(EntryName)
                    EndIf
                EndIf
                
                If LCase(Right(EntryName, 4)) = ".cwu"
                    Found = 0
                    MutexLock(MainMutex, #PB_Compiler_Procedure)
                    ForEach Maps()
                        If Maps()\Filename = EntryName
                            Found = 1
                            Break
                        EndIf
                    Next
                    UnlockMutex(MainMutex)
                    If Found = 0
                        NewUnloadedMap(EntryName)
                    EndIf
                    
                EndIf
            EndIf
        Wend
    EndIf
    
    FinishDirectory(Directory_ID)
    
    MutexLock(MainMutex, #PB_Compiler_Procedure) ; Memory concervation ;D 
    ForEach Maps()
        If Maps()\Loaded
            If Maps()\Clients = 0
                Maps()\LastClient + 1000
            EndIf
            If Maps()\LastClient >= #MaximumMaploadedTime
                MapUnload(Maps())
            EndIf
        EndIf
    Next
    UnlockMutex(MainMutex)
    
EndProcedure

Procedure MapShutdown()
    
    ;MutexLock(MainMutex, #PB_Compiler_Procedure)
    ForEach Maps()
       
        If Maps()\Loaded = #True
            MapSave(Maps())
            FreeMemory(Maps()\BaseMap\Data)
        EndIf
        
        If Maps()\BaseMap\Metadata
            FreeMemory(Maps()\BaseMap\Metadata)
        EndIf
        
        ClearStructure(Maps()\BaseMap, CWMap)
        ClearStructure(Maps(), HMap)
        DeleteElement(Maps())
    Next
    ;UnlockMutex(MainMutex)
    
    _Log("Info", "All maps saved and unloaded.", #PB_Compiler_Line, #PB_Compiler_Procedure)
EndProcedure

Procedure MapAutoSave()
    MutexLock(MainMutex, #PB_Compiler_Procedure)
    
    ForEach Maps()
        If Maps()\Loaded
            MapSave(@Maps())
        EndIf
    Next
    
    UnlockMutex(MainMutex)
EndProcedure

RegisterCore("Maps", 1000, @MapInit(), @MapShutdown(), @MapMain())
RegisterCore("Mapsave", 300000, #Null, #Null, @MapAutoSave())
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 152
; FirstLine = 97
; Folding = 9Pj-
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory