[Setup]
AppName=Visor 3.0
AppVersion=3.0.0.0
DefaultDirName={pf}\Visor
DefaultGroupName=Visor
UninstallDisplayIcon={app}\Visor.EXE

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"


[Icons]
Name: "{group}\Visor 3.0"; Filename: "{app}\Visor.EXE"; WorkingDir: "{app}"
Name: "{group}\Desinstalar"; Filename: "{uninstallexe}"

Name: "{userdesktop}\Visor 3.0"; Filename: "{app}\Visor.EXE"; Tasks: desktopicon


[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked


[Files]
Source: "dependencies\NDP451-KB2858728-x86-x64-AllOS-ENU.exe"; DestDir: {tmp}; Flags: deleteafterinstall; Check: FrameworkIsNotInstalled

Source: "dependencies\JWP2\*.*"; DestDir: "C:\JWPreludio"
Source: "dependencies\JWC2\*.*"; DestDir: "C:\JWCanticos"
Source: "dependencies\JWVISOR2\*.*"; DestDir: "C:\JWMidias"

Source: "dependencies\Fonts\guiv2t.ttf"; DestDir: "{fonts}"; FontInstall: "Guifx v2 Transports"; Flags: onlyifdoesntexist uninsneveruninstall
Source: "dependencies\Fonts\arlrdb.ttf"; DestDir: "{fonts}"; FontInstall: "Arial Rounded MT Bold"; Flags: onlyifdoesntexist uninsneveruninstall
Source: "dependencies\Fonts\tcm.ttf";    DestDir: "{fonts}"; FontInstall: "Tw Cen MT"; Flags: onlyifdoesntexist uninsneveruninstall
Source: "dependencies\Fonts\TCMI____.ttf";    DestDir: "{fonts}"; FontInstall: "Tw Cen MT Italic"; Flags: onlyifdoesntexist uninsneveruninstall
Source: "dependencies\Fonts\TCCB____.ttf";    DestDir: "{fonts}"; FontInstall: "Tw Cen MT Condensed Negrito"; Flags: onlyifdoesntexist uninsneveruninstall

Source: "bin\debug\*.dll"; DestDir: "{app}"

;C:\Users\{NOMEDOUSUARIO}\AppData\Roaming\Visor
Source: "bin\debug\config.xml"; DestDir: "{userappdata}\Visor"

Source: "bin\debug\Visor.exe"; DestDir: "{app}"


[Run]
Filename: {tmp}\NDP451-KB2858728-x86-x64-AllOS-ENU.exe; Description: Install Microsoft .NET Framework 4.5.1; Parameters: /q /norestart; Check: FrameworkIsNotInstalled


[code]
function FrameworkIsNotInstalled: Boolean;
begin
  Result := not RegKeyExists(HKEY_LOCAL_MACHINE, 'Software\Microsoft\.NETFramework\policy\v4.0');
  WizardForm.StatusLabel.Caption := 'Instalando .NET Framework 4.5.1. Aguarde.';
end;