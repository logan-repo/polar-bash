#define AppName "Polar bash"
#define AppVersion "0.3.0"
#define AppExe "PolarBash.exe"
#define GitInstallerUrl "https://github.com/git-for-windows/git/releases/download/v2.56.0.windows.1/Git-2.56.0-64-bit.exe"
#define GitInstallerSha256 "bfe94e7b419b16eee9fecbd1253a98e3d4f49ba8f029630549052278ffe286a6"
#define WebView2BootstrapperUrl "https://go.microsoft.com/fwlink/p/?LinkId=2124703"

[Setup]
AppId={{B3E9B70F-6AE6-4E83-9C1B-15B5C348E244}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=Polar bash
AppPublisherURL=https://github.com/logan-repo/polar-cmd
AppSupportURL=https://github.com/logan-repo/polar-cmd/issues
DefaultDirName={localappdata}\Programs\Polar bash
DefaultGroupName=Polar bash
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.17763
SetupIconFile=..\assets\PolarBash.ico
UninstallDisplayIcon={app}\{#AppExe}
OutputDir=..\artifacts\installer
OutputBaseFilename=PolarBash-Setup-{#AppVersion}-win-x64
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ChangesAssociations=yes
CloseApplications=yes

[Languages]
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\artifacts\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Polar bash"; Filename: "{app}\{#AppExe}"; IconFilename: "{app}\assets\PolarBash.ico"
Name: "{autodesktop}\Polar bash"; Filename: "{app}\{#AppExe}"; IconFilename: "{app}\assets\PolarBash.ico"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "바탕 화면에 Polar bash 바로 가기 만들기"; GroupDescription: "추가 아이콘:"; Flags: unchecked

[Registry]
; Appending \. keeps a drive-root backslash from escaping the closing quote.
; Right-click inside an open folder. Explorer replaces %V with that folder's path.
Root: HKCU; Subkey: "Software\Classes\Directory\Background\shell\PolarBash"; ValueType: string; ValueName: "MUIVerb"; ValueData: "여기서 Polar bash 실행하기"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\Directory\Background\shell\PolarBash"; ValueType: string; ValueName: "Icon"; ValueData: """{app}\{#AppExe}"",0"
Root: HKCU; Subkey: "Software\Classes\Directory\Background\shell\PolarBash\command"; ValueType: string; ValueData: """{app}\{#AppExe}"" --cwd ""%V\."""

; Right-click a folder itself. Explorer replaces %1 with the selected folder.
Root: HKCU; Subkey: "Software\Classes\Directory\shell\PolarBash"; ValueType: string; ValueName: "MUIVerb"; ValueData: "여기서 Polar bash 실행하기"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\Directory\shell\PolarBash"; ValueType: string; ValueName: "Icon"; ValueData: """{app}\{#AppExe}"",0"
Root: HKCU; Subkey: "Software\Classes\Directory\shell\PolarBash\command"; ValueType: string; ValueData: """{app}\{#AppExe}"" --cwd ""%1\."""

; A drive root is also a directory, but Explorer uses the Drive class for it.
Root: HKCU; Subkey: "Software\Classes\Drive\shell\PolarBash"; ValueType: string; ValueName: "MUIVerb"; ValueData: "여기서 Polar bash 실행하기"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\Drive\shell\PolarBash"; ValueType: string; ValueName: "Icon"; ValueData: """{app}\{#AppExe}"",0"
Root: HKCU; Subkey: "Software\Classes\Drive\shell\PolarBash\command"; ValueType: string; ValueData: """{app}\{#AppExe}"" --cwd ""%1\."""

[Run]
Filename: "{app}\{#AppExe}"; Description: "Polar bash 실행"; Flags: nowait postinstall skipifsilent

[Code]
var
  DownloadPage: TDownloadWizardPage;

function GitAt(const Root: String): Boolean;
begin
  Result := (Root <> '') and FileExists(AddBackslash(Root) + 'bin\bash.exe');
end;

function GitAvailable: Boolean;
var
  Root: String;
begin
  Result := GitAt(ExpandConstant('{pf}\Git')) or
    GitAt(ExpandConstant('{pf32}\Git')) or
    GitAt(ExpandConstant('{localappdata}\Programs\Git'));
  if Result then Exit;

  if RegQueryStringValue(HKEY_CURRENT_USER, 'Software\GitForWindows', 'InstallPath', Root) and GitAt(Root) then
  begin
    Result := True;
    Exit;
  end;
  if RegQueryStringValue(HKEY_LOCAL_MACHINE_64, 'Software\GitForWindows', 'InstallPath', Root) and GitAt(Root) then
  begin
    Result := True;
    Exit;
  end;
  Result := RegQueryStringValue(HKEY_LOCAL_MACHINE_32, 'Software\GitForWindows', 'InstallPath', Root) and GitAt(Root);
end;

function WebView2Available: Boolean;
var
  Version: String;
  Subkey: String;
begin
  Subkey := 'Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}';
  Result := RegQueryStringValue(HKEY_CURRENT_USER, Subkey, 'pv', Version) and
    (Version <> '') and (Version <> '0.0.0.0');
  if not Result then
    Result := RegQueryStringValue(HKEY_LOCAL_MACHINE_32, Subkey, 'pv', Version) and
      (Version <> '') and (Version <> '0.0.0.0');
end;

procedure InitializeWizard;
begin
  DownloadPage := CreateDownloadPage('필수 프로그램 다운로드',
    'Polar bash에 필요한 프로그램을 다운로드하고 있습니다.', nil);
  DownloadPage.ShowBaseNameInsteadOfUrl := True;
end;

procedure ShowPrerequisiteError(const MessageText: String);
begin
  Log(MessageText);
  if not WizardSilent then
    MsgBox(MessageText, mbCriticalError, MB_OK);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  NeedGit, NeedWebView2: Boolean;
  Missing, ErrorText: String;
  ErrorCode: Integer;
begin
  Result := True;
  if CurPageID <> wpReady then Exit;

  NeedGit := not GitAvailable;
  NeedWebView2 := not WebView2Available;
  if not (NeedGit or NeedWebView2) then Exit;

  Missing := '';
  if NeedGit then Missing := '- Git for Windows'#13#10;
  if NeedWebView2 then Missing := Missing + '- Microsoft Edge WebView2 Runtime'#13#10;
  if not WizardSilent then
    if MsgBox('다음 필수 프로그램이 없습니다:'#13#10#13#10 +
      Missing + #13#10 + '공식 설치 파일을 다운로드하여 자동으로 설치합니다. 계속할까요?',
      mbConfirmation, MB_YESNO) <> IDYES then
    begin
      Result := False;
      Exit;
    end;

  DownloadPage.Clear;
  if NeedGit then
    DownloadPage.Add('{#GitInstallerUrl}', 'Git-2.56.0-64-bit.exe', '{#GitInstallerSha256}');
  if NeedWebView2 then
    DownloadPage.Add('{#WebView2BootstrapperUrl}', 'MicrosoftEdgeWebview2Setup.exe', '');
  DownloadPage.Show;
  try
    try
      DownloadPage.Download;
    except
      ErrorText := '필수 프로그램 다운로드 실패: ' + GetExceptionMessage + #13#10 +
        '인터넷 연결을 확인하고 다시 시도해 주세요.';
      ShowPrerequisiteError(ErrorText);
      Result := False;
      Exit;
    end;
  finally
    DownloadPage.Hide;
  end;

  if NeedGit then
  begin
    Log('Installing Git for Windows');
    if not ShellExec('', ExpandConstant('{tmp}\Git-2.56.0-64-bit.exe'),
      '/SP- /VERYSILENT /NORESTART /CURRENTUSER /DIR="' +
      ExpandConstant('{localappdata}\Programs\Git') + '"', '',
      SW_SHOWNORMAL, ewWaitUntilTerminated, ErrorCode) or not GitAvailable then
    begin
      ShowPrerequisiteError('Git for Windows 설치가 완료되지 않았습니다. ' +
        'https://git-scm.com/install/windows 에서 직접 설치한 뒤 다시 시도해 주세요.');
      Result := False;
      Exit;
    end;
  end;

  if NeedWebView2 then
  begin
    Log('Installing Microsoft Edge WebView2 Runtime');
    if not ShellExec('', ExpandConstant('{tmp}\MicrosoftEdgeWebview2Setup.exe'),
      '/silent /install', '', SW_SHOWNORMAL, ewWaitUntilTerminated, ErrorCode) or
      not WebView2Available then
    begin
      ShowPrerequisiteError('WebView2 Runtime 설치가 완료되지 않았습니다. ' +
        'https://developer.microsoft.com/en-us/microsoft-edge/webview2/ 에서 ' +
        '직접 설치한 뒤 다시 시도해 주세요.');
      Result := False;
    end;
  end;
end;
