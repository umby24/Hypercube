; Named Binary Tag Reader
; By Umby24
; Written for the Hypercube Minecraft Server
; ##########################################

XIncludeFile "ZLib.pbi"

#NBTBufferSize = 67108864
;{ Enumerations
Enumeration
    #NBTCompressionNone
    #NBTCompressionDetect
    #NBTCompressionGZip
    #NBTCompressionZlib
EndEnumeration

Enumeration
    #TAG_End
    #TAG_Byte
    #TAG_Short
    #TAG_Int
    #TAG_Long
    #TAG_Float
    #TAG_Double
    #TAG_Byte_Array
    #TAG_String
    #TAG_List
    #TAG_Compound
    #TAG_Int_Array
EndEnumeration
;}
;{ Structures
Structure NBTTag
    Name.s
    Type.a
    PayloadSize.l
    
    RawSize.l
    *RawData
    *Parent.NBTTag
    
    Byte.b
    Short.w
    Int.l
    Long.q
    Float.f
    Double.d
    String.s
    
    ListType.a
    ListLength.l
    
    List Children.NBTTag()
EndStructure

Structure NBTElement
    BaseTag.NBTTag
EndStructure
;}
;{ Helping Assembly Functions
Procedure.l Endian(val.l)
    !MOV Eax,dword[p.v_val]
    !BSWAP Eax
    ProcedureReturn
EndProcedure

Procedure.w EndianW(val.w) ; Change Endianness of a Short (Word). Yay inline ASM!
  !MOV ax, word[p.v_val]
  !XCHG al, ah                ; Swap Lo byte <-> Hi byte
  !MOV word[p.v_val], ax
  ProcedureReturn val
EndProcedure

Procedure.q EndianQ(val.q)
    Protected addr.l = @val
    
  EnableASM
  MOV edx, addr
  MOV eax, [edx + 4]
  MOV edx, [edx]
  BSWAP eax
  BSWAP edx
  DisableASM
  
  ProcedureReturn
EndProcedure
;}
Global NbtTagHolder.NBTTag
;{ Reading Functions
Procedure ParseUncompressedData(*Data, Datalength)
    Protected Reading = 1, Offset, LastType
    *PointedWorking.NBTTag
    
    *PointedWorking = @NbtTagHolder
    
    While Reading
        TagType.a = PeekA(*Data + Offset)
        Offset + 1
        
        If TagType <> #TAG_End
            If *PointedWorking\Parent And LastType <> #TAG_Compound
                *TempPointer.NBTTag = *PointedWorking\Parent
                AddElement(*TempPointer\Children())
                *TempPointer\Children()\Parent = *TempPointer
                *PointedWorking = *TempPointer\Children()
            EndIf
            
            Length = EndianW(PeekW(*Data + Offset))
            Offset + 2
            *PointedWorking\Name = PeekS(*Data + Offset, Length)
            *PointedWorking\Type = TagType
            Offset + Length
        Else
            *PointedWorking = *PointedWorking\Parent
        EndIf
        
        Select TagType
            Case #TAG_END
                If *PointedWorking\Parent = 0
                    ; Done reading the file :o?
                    Reading = 0
                ;Else
                ;    *PointedWorking = *PointedWorking\Parent
                EndIf
                
            Case #TAG_Byte                
                *PointedWorking\Byte = PeekB(*Data + Offset)
                Offset + 1
            Case #TAG_Short                
                *PointedWorking\Short = EndianW(PeekW(*Data + Offset))
                Offset + 2
            Case #TAG_Int                
                *PointedWorking\Int = Endian(PeekL(*Data + Offset))
                Offset + 4
            Case #TAG_Long                
                *PointedWorking\Long = EndianQ(PeekQ(*Data + Offset))
                Offset + 8
            Case #TAG_Float                
                *PointedWorking\Float = Endian(PeekF(*Data + Offset))
                Offset + 4
            Case #TAG_Double                
                *PointedWorking\Double = EndianQ(PeekD(*Data + Offset))
                Offset + 8
            Case #TAG_Byte_Array                
                *PointedWorking\RawSize = Endian(PeekL(*Data + Offset))
                Offset + 4
                *PointedWorking\RawData = AllocateMemory(*PointedWorking\RawSize)
                CopyMemory(*Data + Offset, *PointedWorking\RawData, *PointedWorking\RawSize)
                Offset + *PointedWorking\RawSize
                
            Case #TAG_String                
                *PointedWorking\RawSize = EndianW(PeekW(*Data + Offset))
                Offset + 2
                *PointedWorking\String = PeekS(*Data + Offset, *PointedWorking\RawSize)
                Offset + *PointedWorking\RawSize
            Case #TAG_List                
                *PointedWorking\ListType = PeekA(*Data + Offset)
                Offset + 1
                *PointedWorking\ListLength = Endian(PeekL(*Data + Offset))
                Offset + 4
                
                *PointedWorking\RawData = AllocateMemory(*PointedWorking\ListLength)
                CopyMemory(*Data + Offset, *PointedWorking\RawData, *PointedWorking\ListLength)
                Offset + *PointedWorking\ListLength
            Case #TAG_Compound                
                AddElement(*PointedWorking\Children())
                *PointedWorking\Children()\Parent = *PointedWorking
                *PointedWorking = *PointedWorking\Children()
                
            Case #TAG_Int_Array                
                *PointedWorking\ListType = #TAG_Int
                *PointedWorking\ListLength = Endian(PeekL(*Data + Offset))
                Offset + 4
                
                *PointedWorking\RawData = AllocateMemory(*PointedWorking\ListLength * 4)
                CopyMemory(*Data + Offset, *PointedWorking\RawData, *PointedWorking\ListLength * 4)
                Offset + *PointedWorking\ListLength * 4
        EndSelect
        
        LastType = TagType
    Wend
    
    ;ParentTag\BaseTag = Working
    
    ProcedureReturn *PointedWorking
EndProcedure

Procedure DecompressData(*Data, DataLength)
    Protected Stream.z_stream, BufferSize
    
    BufferSize = #NBTBufferSize
    *Temp = AllocateMemory(BufferSize)
    
    Stream\avail_in = DataLength
    Stream\avail_out = BufferSize
    Stream\next_in = *Data
    Stream\next_out = *Temp
    
    If Not inflateInit2_(@Stream, 47, zlibVersion(), SizeOf(z_stream)) = #Z_OK
        FreeMemory(*Temp)
        ProcedureReturn #Null
    EndIf
    
    Repeat
        TempResult = inflate(Stream, #Z_NO_FLUSH)
        
        If TempResult <> #Z_OK And TempResult <> #Z_STREAM_END
            FreeMemory(*Temp)
            inflateEnd(Stream)
            ProcedureReturn #Null
        EndIf
        
        If TempResult = #Z_OK
            BufferSize + #NBTBufferSize
            *Temp = ReAllocateMemory(*Temp, BufferSize)
            
            Stream\avail_out = #NBTBufferSize
            Stream\next_out = *Temp + BufferSize - #NBTBufferSize
        EndIf
    Until TempResult = #Z_STREAM_END
    
    inflateEnd(Stream)
    
    ProcedureReturn *Temp
EndProcedure

Procedure ParseNBT(*Data, DataLength, CompressionMode)
    ; This file will first decompress the data (if need be) then pass it off for real parsing, and then return it.
    *Result.NBTTag
    
    Select CompressionMode
        Case #NBTCompressionNone
            *Result = ParseUncompressedData(*Data, DataLength)
            FreeMemory(*Data)
        Case #NBTCompressionDetect
            If PeekA(*Data) = $1F And PeekA(*Data + 1) = $8B
                ; Compressed. Need to decompress.
                *Uncompressed = DecompressData(*Data, DataLength)
                ; We can now free our compressed data from memory, it is no longer needed.
                FreeMemory(*Data)
                
                If *Uncompressed <> #Null
                    *Result = ParseUncompressedData(*Uncompressed, MemorySize(*Uncompressed))
                    FreeMemory(*Uncompressed)
                EndIf
                
            Else
                *Result = ParseUncompressedData(*Data, DataLength)
                FreeMemory(*Data)
            EndIf
        Case #NBTCompressionGZip, #NBTCompressionZlib
            *Uncompressed = DecompressData(*Data, DataLength)
            ; We can now free our compressed data from memory, it is no longer needed.
            FreeMemory(*Data)
            
            If *Uncompressed <> #Null
                *Result = ParseUncompressedData(*Uncompressed, MemorySize(*Uncompressed))
            EndIf
            
            FreeMemory(*Uncompressed)
    EndSelect
    
    ProcedureReturn *Result
EndProcedure

Procedure ReadNBTFile(Filename.s, CompressionMode=#NBTCompressionDetect)
    Protected FileID, *FileBuffer
    
    FileID = OpenFile(#PB_Any, Filename)
    
    If Not FileID
        ProcedureReturn ; Couldn't open file.
    EndIf
    
    *FileBuffer = AllocateMemory(Lof(FileID))
    
    If Not *FileBuffer
        CloseFile(FileID)
        ProcedureReturn ; Failed to allocate memory
    EndIf
    
    If ReadData(FileID, *FileBuffer, Lof(FileID)) <> Lof(FileID)
        CloseFile(FileID)
        FreeMemory(*FileBuffer)
        ProcedureReturn
    EndIf
    
    CloseFile(FileID)
    
    *ParsedNBT.NBTTag = ParseNBT(*FileBuffer, MemorySize(*FileBuffer), CompressionMode)
    
    ProcedureReturn *ParsedNBT
EndProcedure
;}
;{ Saving Functions
Procedure GetNBTSize(*Tag.NBTTag)
    Protected Size
    
    If Not *Tag
        ProcedureReturn #False
    EndIf
    
    If Not (*Tag\Parent And *Tag\Parent\Type = #TAG_List)
        Size + 3 + StringByteLength(*Tag\Name, #PB_UTF8)
    EndIf
    
    ;Get payload size
    Select *Tag\Type
        Case #TAG_Byte : Size + 1
        Case #TAG_Short : Size + 2
        Case #TAG_Int : Size + 4
        Case #TAG_Long : Size + 8
        Case #TAG_Float : Size + 4
        Case #TAG_Double : Size + 8
        Case #TAG_Byte_Array : Size + 4 + *Tag\RawSize
        Case #TAG_String : Size + 2 + StringByteLength(*Tag\String, #PB_UTF8)
        Case #TAG_List
            Size + 5 ;Type and list size.
            
            ForEach *Tag\Children()
                Size + GetNBTSize(*Tag\Children())
            Next
        Case #TAG_Compound
            ForEach *Tag\Children()
                Size + GetNBTSize(*Tag\Children())
            Next
            Size + 1 ; Tag_End
        Case #TAG_Int_Array : Size + 4 + (*Tag\ListLength * 4)
    EndSelect
    
    ProcedureReturn Size
EndProcedure

Procedure SaveNBT(*Tag.NBTTag, *Memory)
    If Not *Tag
        ProcedureReturn #False
    EndIf
    
    If Not *Memory
        ProcedureReturn #False
    EndIf
    
    If Not (*Tag\Parent And *Tag\Parent\Type = #TAG_List)
        PokeB(*Memory, *Tag\Type) : *Memory + 1 ; Tag Type
        PokeW(*Memory, EndianW(StringByteLength(*Tag\Name, #PB_UTF8))) : *Memory + 2 ; Length of name
        PokeS(*Memory, *Tag\Name, -1, #PB_UTF8) : *Memory + StringByteLength(*Tag\Name, #PB_UTF8) ; The actual name
    EndIf
    
    ;Get payload size
    Select *Tag\Type
        Case #TAG_Byte
            PokeB(*Memory, *Tag\Byte)
            *Memory + 1
        Case #TAG_Short
            PokeW(*Memory, EndianW(*Tag\Short))
            *Memory + 2
        Case #TAG_Int
            PokeL(*Memory, Endian(*Tag\Int))
            *Memory + 4
        Case #TAG_Long
            PokeQ(*Memory, EndianQ(*Tag\Long))
            *Memory + 8
        Case #TAG_Float
            PokeF(*Memory, Endian(*Tag\Float))
            *Memory + 4
        Case #TAG_Double
            PokeD(*Memory, EndianQ(*Tag\Double))
            *Memory + 8
        Case #TAG_Byte_Array
            PokeL(*Memory, Endian(*Tag\RawSize))
            *Memory + 4
            
            If *Tag\RawData
                CopyMemory(*Tag\RawData, *Memory, *Tag\RawSize)
            EndIf
            
            *Memory + *Tag\RawSize
        Case #TAG_String
            PokeW(*Memory, EndianW(*Tag\RawSize))
            *Memory + 2
            
            PokeS(*Memory, *Tag\String, -1, #PB_UTF8)
            *Memory + *Tag\RawSize
        Case #TAG_List
            PokeB(*Memory, *Tag\ListType) : *Memory + 1
            PokeL(*Memory, Endian(ListSize(*Tag\Children())))
            *Memory + 4
            ForEach *Tag\Children()
                *Memory = SaveNBT(*Tag\Children(), *Memory)
            Next
            
        Case #TAG_Compound
            ForEach *Tag\Children()
                *Memory = SaveNBT(*Tag\Children(), *Memory)
            Next
            
            PokeB(*Memory, 0) : *Memory + 1
        Case #TAG_Int_Array
            PokeL(*Memory, Endian(*Tag\ListLength)) : *Memory + 4
            CopyMemory(*Tag\RawData, *Memory, *Tag\ListLength * 4)
            *Memory + (*Tag\ListLength * 4)
    EndSelect
    
    ProcedureReturn *Memory
EndProcedure

Procedure SaveNBTFile(*NBT.NBTTag, Filename.s, CompressionMode=#NBTCompressionGZip)
    Protected FileID, Stream.z_stream, WindowSize
    
    If Not *NBT
        ProcedureReturn #False
    EndIf
    
    FileID = CreateFile(#PB_Any, Filename)
    
    If Not FileID
        ProcedureReturn #False
    EndIf
    
    *Worker = AllocateMemory(GetNBTSize(*NBT))
    *Startingpoint = *Worker ; Placeholder since the memory location is going to be modified.
    
    *Worker = SaveNBT(*NBT, *Worker)
    
    *Worker = *Startingpoint
    
    If CompressionMode = #NBTCompressionGZip Or CompressionMode = #NBTCompressionZlib
        MemSize.l = MemorySize(*Worker)
        *TempBuffer = AllocateMemory(MemSize)
        
        Stream\avail_in = MemSize
        Stream\avail_out = MemSize
        Stream\next_in = *Worker
        Stream\next_out = *TempBuffer
        
        If CompressionMode = #NBTCompressionGZip
            WindowSize = 31
        Else
            WindowSize = 15
        EndIf
        
        If Not deflateInit2_(Stream, #Z_DEFAULT_COMPRESSION, #Z_DEFLATED, WindowSize, 8, #Z_DEFAULT_STRATEGY, zlibVersion(), SizeOf(z_stream)) = #Z_OK
            FreeMemory(*TempBuffer)
            FreeMemory(*Worker)
            CloseFile(FileID)
            ProcedureReturn #False
        EndIf
        
        If Not deflate(Stream, #Z_FINISH)
            FreeMemory(*TempBuffer)
            FreeMemory(*Worker)
            CloseFile(FileID)
            ProcedureReturn #False
        EndIf
        
        ReAllocateMemory(*TempBuffer, Stream\total_out)
        deflateEnd(Stream)
        
        FreeMemory(*Worker)
        
        WriteData(FileID, *TempBuffer, Stream\total_out)
        
        CloseFile(FileID)
        FreeMemory(*TempBuffer)
        
        ProcedureReturn #True
    Else
        WriteData(FileID, *Worker, MemorySize(*Worker))
        CloseFile(FileID)
        FreeMemory(*Worker)
        ProcedureReturn #True
    EndIf
    
    ProcedureReturn #False
EndProcedure
;}
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 5
; Folding = uL0
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory