# Polar bash

[한국어](#한국어) · [English](#english) · [日本語](#日本語)

## 한국어

쓰레기 같고 재앙처럼 불편한 Windows 터미널 환경을 개선하기 위해 만든 Git Bash 터미널입니다.
복사·붙여넣기와 여러 줄 명령 입력을 다듬고, 북극곰 아이콘과 남색·크림색 테마를 입혔습니다.
.NET 8 WinForms, WebView2, xterm.js, Windows ConPTY로 구성되며 WSL은 사용하지 않습니다.

### PowerShell·기존 Git Bash와 비교

| 불편했던 점 | Polar bash에서 제공하는 방식 |
| --- | --- |
| PowerShell과 Bash의 명령 문법이 달라 Git Bash용 명령을 그대로 쓰기 어려움 | 실제 Git for Windows의 Bash를 실행하므로 Bash 명령, Git 도구, `Tab` 자동완성을 그대로 사용합니다. |
| 터미널에서 복사와 명령 중단을 구분하기 번거로움 | `Ctrl+C`는 선택한 텍스트가 있으면 복사하고, 없으면 실행 중인 명령에 중단 신호를 보냅니다. |
| 여러 줄을 붙여넣을 때 바로 실행될까 걱정됨 | `Ctrl+V`로 붙여넣은 여러 줄은 먼저 편집창에 표시합니다. 내용을 확인·수정한 뒤 `Ctrl+Enter`로 실행합니다. 한 줄은 현재 입력줄에 붙여넣습니다. |
| Git Bash를 원하는 폴더에서 열고 화면을 취향에 맞추고 싶음 | 탐색기 우클릭으로 해당 폴더에서 열고, 밝은·어두운 테마와 글자 크기를 앱 안에서 바꿉니다. |

PowerShell 자체를 대체하는 셸은 아닙니다. 기존 Git Bash의 Bash를 더 편하게 사용하는
인터페이스이며, 위 비교는 Polar bash가 제공하는 작업 흐름을 설명합니다.

### 설치 전 확인

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

### 설치 및 실행

`PolarBash-Setup-0.3.0-win-x64.exe`를 실행하면 현재 사용자 계정에 설치되고,
시작 메뉴에 Polar bash가 추가됩니다. 설치 제거는 Windows 설정의 설치된 앱에서
할 수 있습니다.

설치 후 탐색기의 폴더 빈 공간, 폴더 자체, 드라이브를 오른쪽 클릭하여
**여기서 Polar bash 실행하기**를 선택할 수 있습니다. 이 메뉴는 해당 폴더를
`--cwd` 인자로 전달하고, Git Bash는 그 폴더에서 시작합니다. 폴더 이름에 공백이나
한글이 있어도 따옴표로 감싸 전달합니다. Windows 11에서는 이 기본 레지스트리
메뉴가 **추가 옵션 표시** 안에 나타날 수 있습니다.

### 입력과 화면

- 터미널 입력은 Git Bash에 직접 전달되며 `Tab`은 Bash 자동완성을 사용합니다.
- `Ctrl+C`: 선택 영역이 있으면 복사, 없으면 실행 중인 명령을 중단합니다.
- `Ctrl+V`: 한 줄은 현재 명령줄에 붙여넣고, 여러 줄은 편집창에서 먼저 확인합니다.
- `Ctrl+Shift+E`: 여러 줄 명령 편집창. `Ctrl+Enter`: 편집창 내용 실행.
- 여러 줄 명령은 Bash의 bracketed paste 방식으로 같은 셸에 전달됩니다.
- 왼쪽 메뉴에서 시작 폴더를 탐색기로 열고, 밝은/어두운 테마와 글자 크기를 바꿀 수 있습니다.

Git Bash는 Linux 커널이나 Linux 전용 바이너리를 실행하지 않습니다. 여러 줄 붙여넣기는
Bash에 한 번에 전달하므로, 앱이 각 명령의 완료를 따로 확인하는 실행 큐는 아닙니다.

### 빌드

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

### 소스와 라이선스

`ConPtyReference`의 ConPTY 연결 코드는 Microsoft Terminal 저장소의 MiniTerm
예제를 바탕으로 했습니다. 라이선스는
`ConPtyReference/MICROSOFT-TERMINAL-LICENSE`에 있습니다. xterm.js와 addon-fit의
라이선스는 `wwwroot/vendor`에 있습니다.

---

## English

Polar bash was built to improve the trash-fire experience that the Windows terminal environment can be. It runs the real Git for Windows Bash while making copying, pasting, and entering multiline commands easier. The app has a polar bear icon and navy-and-cream themes. It uses .NET 8 WinForms, WebView2, xterm.js, and Windows ConPTY; it does not use WSL.

### Compared with PowerShell and Git Bash

| Friction | What Polar bash does |
| --- | --- |
| PowerShell and Bash use different command syntax. | Runs Git for Windows Bash, so Bash commands, Git tools, and Bash `Tab` completion work as expected. |
| Copying text and interrupting a command compete for `Ctrl+C`. | Copies selected text when there is a selection; otherwise sends an interrupt to the running command. |
| A multiline paste can be hard to inspect before it runs. | Opens multiline text in an editor first. Review or change it, then press `Ctrl+Enter` to run it. A single line is pasted into the current command line. |
| Opening Git Bash in a particular folder and adjusting its appearance takes extra steps. | Adds an Explorer context menu for the selected folder and offers light/dark themes and font-size controls. |

Polar bash is a UI for Git Bash, not a replacement for the PowerShell language. The comparison describes the workflow this app provides; it does not imply that other terminals lack every listed capability.

### Requirements

| Component | Requirement |
| --- | --- |
| Windows 10 version 1809 or later, or Windows 11; x64 | Required. ConPTY is built into these Windows versions. |
| [Git for Windows](https://git-scm.com/install/windows) | Required for Bash and Git commands. If missing, the setup wizard asks for consent, downloads the official Git 2.56.0 installer, installs it, and checks again. It detects standard install locations and registered custom install paths, but not portable Git. |
| [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) | Required for the terminal display. If missing, setup asks for consent and installs it with Microsoft's Evergreen Bootstrapper. The ordinary Edge browser is not a substitute. |
| .NET 8 Runtime / .NET 8 SDK | No separate installation is needed to use the setup file: the .NET runtime is included. The SDK is needed only to build from source. |
| WSL / standalone MSYS2 / Inno Setup | Not needed to use the app. Inno Setup is needed only to build the installer. |

Missing prerequisites are downloaded from their official sources during setup. An internet connection is required for this automatic installation. If a download or prerequisite installation fails, Polar bash setup stops; install the component manually from the links above and try again.

### Install and use

Run [`PolarBash-Setup-0.3.0-win-x64.exe`](https://github.com/logan-repo/polar-bash/releases/tag/v0.3.0). It installs for the current user and adds Polar bash to the Start menu. Remove it through Windows Settings > Installed apps.

In File Explorer, right-click inside a folder, on a folder, or on a drive and choose **여기서 Polar bash 실행하기** (“Open Polar bash here”). Bash starts in that location, including paths with spaces or Korean characters. On Windows 11, this classic context-menu entry may be under **Show more options**.

### Keyboard and display

- `Tab` uses Bash completion.
- `Ctrl+C` copies a terminal selection or interrupts the running command when nothing is selected.
- `Ctrl+V` pastes one line into Bash; multiline text opens the review editor.
- `Ctrl+Shift+E` opens the multiline editor; `Ctrl+Enter` runs its contents.
- The sidebar can open the starting folder in Explorer. Themes and font size can be changed in the app.

Multiline text is sent to the same Bash session using bracketed paste. Polar bash does not track each line's completion as a separate job queue. Git Bash does not provide a Linux kernel or run Linux-only binaries.

### Build from source

On Windows, install Git for Windows and the .NET 8 SDK, then run:

```powershell
dotnet restore PolarBash.csproj
dotnet build PolarBash.csproj -c Release
```

To build the installer, install Inno Setup 6 and run `./build-installer.ps1` in PowerShell. The script publishes a self-contained x64 app to `artifacts/publish` and writes the installer to `artifacts/installer`. If it cannot find `ISCC.exe`, pass `-InnoCompiler 'C:\path\to\ISCC.exe'`. To regenerate the icon after editing it, run `python tools/make_icon.py`.

### Source and licenses

The ConPTY bridge in `ConPtyReference` is based on Microsoft's Terminal MiniTerm sample; see `ConPtyReference/MICROSOFT-TERMINAL-LICENSE`. Licenses for xterm.js and addon-fit are in `wwwroot/vendor`.

---

## 日本語

Polar bash は、ゴミ同然で災難のように感じる Windows のターミナル環境を改善するために作りました。Git for Windows の本物の Bash を使いながら、コピー・貼り付けと複数行のコマンド入力を扱いやすくします。シロクマのアイコンと紺色・クリーム色のテーマを備えています。.NET 8 WinForms、WebView2、xterm.js、Windows ConPTY を使用し、WSL は使用しません。

### PowerShell・従来の Git Bash との比較

| 不便な点 | Polar bash の動作 |
| --- | --- |
| PowerShell と Bash ではコマンドの構文が異なる。 | Git for Windows の Bash を実行するため、Bash コマンド、Git ツール、`Tab` による Bash の補完をそのまま使えます。 |
| `Ctrl+C` をコピーとコマンド中断で使い分けたい。 | テキスト選択中はコピーし、選択がなければ実行中のコマンドに中断を送ります。 |
| 複数行を貼り付ける前に内容を確認したい。 | 複数行は先に編集画面で表示します。確認・修正してから `Ctrl+Enter` で実行できます。1 行だけなら現在の入力行に貼り付けます。 |
| 指定したフォルダーで Git Bash を開き、見た目も調整したい。 | エクスプローラーの右クリックメニューからその場所で開けます。明暗テーマと文字サイズもアプリ内で変更できます。 |

Polar bash は PowerShell 言語を置き換えるものではなく、Git Bash を使いやすくする UI です。この比較は本アプリの操作方法を説明するもので、他のターミナルに各機能が一切ないという意味ではありません。

### 動作要件

| 項目 | 必要条件 |
| --- | --- |
| Windows 10 バージョン 1809 以降、または Windows 11 の x64 環境 | 必須です。ConPTY は対応する Windows に含まれます。 |
| [Git for Windows](https://git-scm.com/install/windows) | Bash と Git コマンドに必須です。未導入の場合、セットアップが同意を求め、公式の Git 2.56.0 インストーラーをダウンロード・インストールして再確認します。標準の場所とレジストリに登録されたカスタムのインストール先を検出しますが、ポータブル版は自動検出しません。 |
| [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) | ターミナル画面の表示に必須です。未導入の場合、同意を求めて Microsoft の Evergreen Bootstrapper でインストールします。通常の Edge ブラウザーでは代用できません。 |
| .NET 8 Runtime / .NET 8 SDK | インストーラーの利用時に別途導入する必要はありません。.NET ランタイムは同梱されています。SDK はソースからビルドする場合だけ必要です。 |
| WSL / 単独の MSYS2 / Inno Setup | アプリの利用には不要です。Inno Setup はインストーラーをビルドする場合だけ必要です。 |

不足している必須プログラムは、セットアップ中に公式の配布元からダウンロードします。自動インストールにはインターネット接続が必要です。ダウンロードまたはインストールに失敗すると Polar bash のセットアップも中止します。その場合は上記のリンクから手動で導入し、再試行してください。

### インストールと起動

[`PolarBash-Setup-0.3.0-win-x64.exe`](https://github.com/logan-repo/polar-bash/releases/tag/v0.3.0) を実行します。現在のユーザー向けにインストールされ、スタートメニューに Polar bash が追加されます。削除は Windows の「設定」→「インストールされているアプリ」から行えます。

エクスプローラーでフォルダー内の空白部分、フォルダー自体、またはドライブを右クリックし、**여기서 Polar bash 실행하기**（「ここで Polar bash を開く」）を選択します。その場所から Bash が起動し、パスに空白やハングルが含まれていても渡せます。Windows 11 では従来形式のメニューとして **その他のオプションを表示** の中に出る場合があります。

### キー操作と画面

- `Tab`: Bash の補完を使います。
- `Ctrl+C`: 選択範囲があればコピーし、なければ実行中のコマンドを中断します。
- `Ctrl+V`: 1 行は Bash に貼り付け、複数行は確認用の編集画面を開きます。
- `Ctrl+Shift+E`: 複数行の編集画面を開きます。`Ctrl+Enter`: 内容を実行します。
- サイドバーから開始フォルダーをエクスプローラーで開けます。テーマと文字サイズも変更できます。

複数行の内容は bracketed paste で同じ Bash セッションに送られます。各行の完了を個別に追跡する実行キューではありません。Git Bash は Linux カーネルや Linux 専用バイナリを提供しません。

### ソースからのビルド

Windows に Git for Windows と .NET 8 SDK を導入してから実行します。

```powershell
dotnet restore PolarBash.csproj
dotnet build PolarBash.csproj -c Release
```

インストーラーのビルドには Inno Setup 6 が必要です。PowerShell で `./build-installer.ps1` を実行してください。スクリプトは x64 の自己完結型アプリを `artifacts/publish` に発行し、インストーラーを `artifacts/installer` に作成します。`ISCC.exe` が見つからない場合は `-InnoCompiler 'C:\path\to\ISCC.exe'` を指定します。アイコンの変更後に `.ico` を再生成する場合は `python tools/make_icon.py` を実行します。

### ソースとライセンス

`ConPtyReference` の ConPTY 接続コードは Microsoft Terminal の MiniTerm サンプルを基にしています。ライセンスは `ConPtyReference/MICROSOFT-TERMINAL-LICENSE` を参照してください。xterm.js と addon-fit のライセンスは `wwwroot/vendor` にあります。
