; ClassicWorld Map Format Library
; by Umby24
; Made for use with Hypercube
; ###############################

XIncludeFile "UmbyNBT.pbi"

Global NewList MetadataParsers.IMetadataStructure

Structure IMetadataStructure
    ReadFunction.i
    WriteFunction.i
EndStructure

Structure ForeignMeta Extends IMetadataStructure
    Array Tags.NBTTag(0)
EndStructure

Structure CPEMetadata Extends IMetadataStructure
    ClickDistanceVersion.l
    ClickDistance.w
    
    CustomBlocksVersion.l
    CustomBlocksLevel.w
    *CustomBlocksFallback
    
    EnvColorsVersion.l
    SkyColor
    CloudColor
    FogColor
    AmbientColor
    SunlightColor
    
    EnvMapAppearanceVersion.l
    TextureURL.s
    SideBlock.a
    EdgeBlock.a
    SideLevel.w
EndStructure

Procedure ReadForeign(*Tag.NBTTag)
    Define Meta.ForeignMeta
    
    Dim Meta\Tags.NBTTag(ListSize(*Tag\Children()) - 1)
    
    For i = 0 To ListSize(*Tag\Children()) - 1
        SelectElement(*Tag\Children(), i)
        Meta\Tags(i) = *Tag\Children()
    Next
    
    ClearList(*Tag\Children())
    
    Return @Meta
EndProcedure

Procedure WriteForeign(*Meta.ForeignMeta)
    MetaBase.NBTTag
    MetaBase\Type = #TAG_Compound
    MetaBase\Name = "Metadata"
    
    For i = 0 To ArraySize(*Meta\Tags()) - 1
        AddElement(MetaBase\Children())
        MetaBase\Children() = *Meta\Tags(i)
    Next
    
    Return @MetaBase
EndProcedure

Procedure ReadCPEMeta(*Tag.NBTTag)
    Define CPE.CPEMetadata
    
    
; IDE Options = PureBasic 5.00 (Windows - x64)
; CursorPosition = 71
; FirstLine = 14
; Folding = --
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory