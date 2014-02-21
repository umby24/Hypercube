; Function: Load, Save, and Manage all server settings.
;-==== Definitions

Global NetworkSettings.NetworkSettings
Global SystemSettings.SystemSettings

Declare SaveNetworkSettings()
Declare SaveSystemSettings()

;-==== Loading
Procedure LoadNetworkSettings()
    If FileSize("Settings\Network.txt") = -1
        NetworkSettings\ServerName = "Hypercube Server"
        NetworkSettings\MOTD = "&cWelcome to Hypercube!"
        NetworkSettings\MaxPlayers = 128
        NetworkSettings\Port = 9999
        NetworkSettings\WhiteList = #False
        NetworkSettings\Public = #True
        SaveNetworkSettings()
    Else
        OpenPreferences("Settings\Network.txt")
        
        NetworkSettings\ServerName = ReadPreferenceString("Name", "Hypercube Server")
        NetworkSettings\MOTD = ReadPreferenceString("MOTD", "Welcome to Hypercube!")
        NetworkSettings\MaxPlayers = ReadPreferenceLong("Max_Players", 128)
        NetworkSettings\Port = ReadPreferenceLong("Port", 9999)
        NetworkSettings\WhiteList = ReadPreferenceLong("Whitelist", 0)
        NetworkSettings\Public = ReadPreferenceLong("Public", 1)
        ClosePreferences()
    EndIf
    
    NetworkSettings\File_Date_Last = GetFileDate("Settings\Network.txt", #PB_Date_Modified)
    _Log("Info", "Network settings loaded.", 0, "")
EndProcedure

Procedure LoadSystemSettings()
    If FileSize("Settings\System.txt") = -1
        SystemSettings\LogFile = "Log"
        SystemSettings\Logging = #True
        SystemSettings\Main_World = "world"
        SystemSettings\Welcome_Message = "Welcome to Hypercube!"
        
        SaveSystemSettings()
    Else
        OpenPreferences("Settings\System.txt")
        
        SystemSettings\LogFile = ReadPreferenceString("Log_File","Log")
        SystemSettings\Logging = ReadPreferenceLong("Logging",#True)
        SystemSettings\Main_World = ReadPreferenceString("Main_World","world")
        SystemSettings\Welcome_Message = ReadPreferenceString("Welcome_Message","Welcome To Hypercube!")
        
        ClosePreferences()
    EndIf
    
    SystemSettings\File_Date_Last = GetFileDate("Settings\System.txt", #PB_Date_Modified)
    _Log("Info", "System settings loaded.", 0, "")
EndProcedure

;-==== Saving
Procedure SaveNetworkSettings()
    If FileSize("Settings") = -1
        CreateDirectory("Settings")
    EndIf
    
    OpenPreferences("Settings\Network.txt")
    
    WritePreferenceString("Name", NetworkSettings\ServerName)
    WritePreferenceString("MOTD", NetworkSettings\MOTD)
    WritePreferenceInteger("Max_Players",NetworkSettings\MaxPlayers)
    WritePreferenceInteger("Port",NetworkSettings\Port)
    WritePreferenceInteger("Whitelist",NetworkSettings\WhiteList)
    WritePreferenceInteger("Public", NetworkSettings\Public)
    ClosePreferences()
    
    NetworkSettings\File_Date_Last = GetFileDate("Settings\Network.txt", #PB_Date_Modified)
    _Log("Info","Network settings saved.", 0, "")
EndProcedure

Procedure SaveSystemSettings()
    If FileSize("Settings") = -1
        CreateDirectory("Settings")
    EndIf
    
    OpenPreferences("Settings\System.txt")
    
    WritePreferenceString("Log_File", SystemSettings\LogFile)
    WritePreferenceLong("Logging", SystemSettings\Logging)
    WritePreferenceString("Main_World", SystemSettings\Main_World)
    WritePreferenceString("Welcome_Message",SystemSettings\Welcome_Message)
    
    ClosePreferences()
    
    SystemSettings\File_Date_Last = GetFileDate("Settings\System.txt", #PB_Date_Modified)
    _Log("Info", "System settings saved.", 0, "")
EndProcedure


Procedure Settings_Init()
    LoadSystemSettings()
    Log_Init()
    ;LoadNetworkSettings()
EndProcedure

Procedure Settings_Shutdown()
    SaveSystemSettings()
    SaveNetworkSettings()
EndProcedure

registercore("Settings",10000, @Settings_Init(), @Settings_Shutdown(), #Null)

; IDE Options = PureBasic 5.00 (Windows - x86)
; CursorPosition = 98
; FirstLine = 54
; Folding = --
; EnableThread
; EnableXP
; EnableOnError
; CompileSourceDirectory