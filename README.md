# JinTerm

Windows에서 Git Bash를 사용하는 개인용 터미널 앱입니다. .NET 8 WinForms, WebView2,
xterm.js, Windows ConPTY로 구성했습니다. WSL은 사용하지 않습니다.

## 실행

`JinTerm.exe`를 실행합니다. Git for Windows와 Microsoft Edge WebView2 Runtime이
필요합니다. 빌드된 앱을 실행하려면 .NET 8 Desktop Runtime도 필요합니다.

## 입력

- 일반 입력은 Git Bash로 바로 전달됩니다. `Tab`은 Bash의 자동완성을 사용합니다.
- `Ctrl+C`: 선택 영역이 있으면 복사, 없으면 실행 중인 명령 중단.
- `Ctrl+V`: 한 줄은 현재 명령줄에 붙여넣고, 여러 줄은 편집창에서 먼저 확인합니다.
- `Ctrl+Shift+E`: 여러 줄 명령 편집창 열기. `Ctrl+Enter`: 편집창 내용 실행.
- 여러 줄 명령은 Bash의 bracketed paste 방식으로 한 번에 전달됩니다. Bash가 각
  명령을 위에서부터 처리하며, `cd` 같은 셸 상태도 유지됩니다. 실행 전 내용을 확인하세요.

터미널은 Git Bash 환경을 사용합니다. Linux 커널이나 Linux 전용 바이너리를 실행하지
않습니다. 이 앱은 Warp의 전체 기능을 복제하지 않으며, 명령 블록, AI 기능,
클라우드 동기화는 포함하지 않습니다.

## 소스 및 라이선스

`ConPtyReference`의 ConPTY 연결 코드는 Microsoft Terminal 저장소의 MiniTerm
예제를 바탕으로 했습니다. 해당 라이선스는
`ConPtyReference/MICROSOFT-TERMINAL-LICENSE`에 있습니다. 터미널 렌더러
`xterm.js`와 `addon-fit`의 라이선스는 `wwwroot/vendor`에 있습니다.

## 빌드

Windows에서 Git for Windows와 .NET 8 SDK를 설치한 뒤 저장소 루트에서
`dotnet restore JinTerm.csproj`, `dotnet build JinTerm.csproj -c Release`를 실행합니다.
`tests/BridgeCheck`는 Git Bash 멀티라인 입력과 Tab 자동완성을 확인하는 선택적
테스트 소스입니다. 실행하려면 콘솔 환경에서
`dotnet run --project tests/BridgeCheck/BridgeCheck.csproj`를 사용합니다.
Git Bash가 기본 설치 위치에 없다면 `GIT_BASH_PATH` 환경 변수를 설정합니다.
