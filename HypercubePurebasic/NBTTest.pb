; NBT Serialization Test
; Using UmbyNBT
; ################

XIncludeFile "Includes/UmbyNBT.pbi"


Procedure.s SerializeNBT(*Tag.NBTTag, Level=0)
    Protected Output.s = Space(Level * 2)
    
    Select *Tag\Type
        Case #TAG_End
            Output + "[This Shouldn't Appear!]"
        Case #TAG_Byte
            Output + "TAG_Byte('" + *Tag\Name + "'): " + Str(*Tag\Byte)
        Case #TAG_Short
            Output + "TAG_Short('" + *Tag\Name + "'): " + Str(*Tag\Short)
        Case #TAG_Int
            Output + "TAG_Int('" + *Tag\Name + "'): " + Str(*Tag\Int)
        Case #Tag_Long
            Output + "TAG_Long('" + *Tag\Name + "'): " + Str(*Tag\Long)
        Case #TAG_Float
            Output + "TAG_Float('" + *Tag\Name + "'): " + StrF(*Tag\Float)
        Case #TAG_Double
            Output + "TAG_Double('" + *Tag\Name + "'): " + StrF(*Tag\Double)
        Case #TAG_Byte_Array
            Output + "TAG_Byte_Array('" + *Tag\Name + "'): [" + Str(*tag\RawSize) + " bytes]"
        Case #TAG_String
            output + "TAG_String('" + *Tag\Name + "'): '" + *Tag\String + "'"
        Case #TAG_List
            output + "TAG_List('" + *Tag\Name + "'): " + Str(*Tag\ListLength) + " entries"
        Case #TAG_Compound
            Output + "TAG_Compound('" + *Tag\Name + "'): " + Str(ListSize(*tag\Children())) + " entries" + #CRLF$
            Output + Space(Level * 2) + "{" + #CRLF$
            ForEach *Tag\Children()
                Output + SerializeNBT(*Tag\Children(), Level + 1) + #CRLF$
            Next
            Output + Space(Level * 2) + "}"
        Case #TAG_Int_Array
            Output + "TAG_Int_Array('" + *Tag\Name + "'): [" + Str(*Tag\ListLength) + " ints]"
        Default
            Output + "[Something went wrong!]"
    EndSelect
    
    ProcedureReturn Output
EndProcedure

OpenConsole()

*Finished = ReadNBTFile("Test.cw")

Result.s = SerializeNBT(*Finished)

PrintN(Result)

PrintN("Saving file...")

SaveResult = SaveNBTFile(*Finished, "TestOut.cw")

PrintN("Done " + Str(SaveResult))

Input()
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 4
; Folding = -
; EnableThread
; EnableXP
; EnableOnError
; Executable = NBTTest.exe
; CompileSourceDirectory