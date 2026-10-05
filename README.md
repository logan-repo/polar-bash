# Polar bash

Windows에서 Git Bash를 사용하는 터미널 앱입니다. 참고 이미지의 북극곰 아이콘과
남색·크림색 테마를 적용했습니다. .NET 8 WinForms, WebView2, xterm.js, Windows
ConPTY로 구성되며 WSL은 사용하지 않습니다.

## 설치 전 확인

| 항목 | 필요 여부 | 설명 |
| --- | --- | --- |
| Windows 10 버전 1809 이상 또는 Windows 11, x64 | 필수 | 터미널 연결에 쓰는 ConPTY는 Windows 10 1809부터 제공됩니다. 별도 설치는 필요하지 않습니다. |
| [Git for Windows](https://git-scm.com/install/windows) | 필수 | 실제 Bash와 Git 명령을 제공합니다. 없으면 Polar bash 설치 중 동의를 받은 뒤 공식 Git 2.56.0 설치 파일을 다운로드해 설치합니다. 일반 설치 경로와 Git for Windows의 설치 경로 레지스트리를 확인합니다. 포터블 버전은 자동 탐색하지 않습니다. |
| [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) | 필수 | 터미널 화면을 표시합니다. 없으면 Polar bash 설치 중 동의를 받은 뒤 Microsoft의 Evergreen Bootstrapper로 설치합니다. 일반 Edge 브라우저만으로 대체할 수 없습니다. |
| .NET 8 Runtime / .NET 8 SDK | 별도 설치 불필요 | 아래의 최종 설치 파일은 .NET 런타임을 포함하는 자체 포함 배포입니다. SDK는 소스를 직접 빌드할 때만 필요합니다. |
| WSL / MSYS2 / Inno Setup | 사용 시 설치 불필요 | WSL과 MSYS2는 실행에 사용하지 않습니다. Inno Setup은 설치 파일을 직접 빌드할 때만 필요합니다. |

Git for Windows나 WebView2 Runtime이 없으면 설치 마법사가 설치 시작 전에 알려주고,
동의하면 인터넷에서 공식 설치 파일을 받아 순서대로 설치한 뒤 다시 검사합니다.
다운로드 또는 설치가 실패하면 Polar bash 설치도 중단됩니다. 인터넷을 사용할 수 없는
PC에서는 위 링크에서 필요한 프로그램을 별도로 준비해 먼저 설치하세요. .NET 런타임은
Polar bash 설치 파일에 포함되어 있어 이 과정에서 다운로드하지 않습니다.

## 설치 및 실행

`PolarBash-Setup-0.3.0-win-x64.exe`를 실행하면 현재 사용자 계정에 설치되고,
시작 메뉴에 Polar bash가 추가됩니다. 설치 제거는 Windows 설정의 설치된 앱에서
할 수 있습니다.

설치 후 탐색기의 폴더 빈 공간, 폴더 자체, 드라이브를 오른쪽 클릭하여
**여기서 Polar bash 실행하기**를 선택할 수 있습니다. 이 메뉴는 해당 폴더를
`--cwd` 인자로 전달하고, Git Bash는 그 폴더에서 시작합니다. 폴더 이름에 공백이나
한글이 있어도 따옴표로 감싸 전달합니다. Windows 11에서는 이 기본 레지스트리
메뉴가 **추가 옵션 표시** 안에 나타날 수 있습니다.

## 입력과 화면

- 터미널 입력은 Git Bash에 직접 전달되며 `Tab`은 Bash 자동완성을 사용합니다.
- `Ctrl+C`: 선택 영역이 있으면 복사, 없으면 실행 중인 명령을 중단합니다.
- `Ctrl+V`: 한 줄은 현재 명령줄에 붙여넣고, 여러 줄은 편집창에서 먼저 확인합니다.
- `Ctrl+Shift+E`: 여러 줄 명령 편집창. `Ctrl+Enter`: 편집창 내용 실행.
- 여러 줄 명령은 Bash의 bracketed paste 방식으로 같은 셸에 전달됩니다.
- 왼쪽 메뉴에서 시작 폴더를 탐색기로 열고, 밝은/어두운 테마와 글자 크기를 바꿀 수 있습니다.

Git Bash는 Linux 커널이나 Linux 전용 바이너리를 실행하지 않습니다. 여러 줄 붙여넣기는
Bash에 한 번에 전달하므로, 앱이 각 명령의 완료를 따로 확인하는 실행 큐는 아닙니다.

## 빌드

Windows에서 Git for Windows와 .NET 8 SDK를 설치한 뒤:

```powershell
dotnet restore PolarBash.csproj
dotnet build PolarBash.csproj -c Release
```

Inno Setup 6의 `ISCC.exe`를 설치한 뒤 설치 파일을 만들려면:

```powershell
.\build-installer.ps1
```

스크립트는 x64 자체 포함 앱을 `artifacts/publish`에 게시하고, 설치 파일을
`artifacts/installer`에 만듭니다. `ISCC.exe` 위치를 자동으로 찾지 못하면
`.\build-installer.ps1 -InnoCompiler 'C:\path\to\ISCC.exe'`로 지정할 수 있습니다.
아이콘을 수정한 경우 `python tools/make_icon.py`로 `.ico`를 다시 만듭니다.

## 소스와 라이선스

`ConPtyReference`의 ConPTY 연결 코드는 Microsoft Terminal 저장소의 MiniTerm
예제를 바탕으로 했습니다. 라이선스는
`ConPtyReference/MICROSOFT-TERMINAL-LICENSE`에 있습니다. xterm.js와 addon-fit의
라이선스는 `wwwroot/vendor`에 있습니다.
