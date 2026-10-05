param(
    [string]$InnoCompiler
)

$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath($PSScriptRoot)
$publish = [IO.Path]::GetFullPath((Join-Path $projectRoot 'artifacts\publish'))
if (-not $publish.StartsWith($projectRoot + [IO.Path]::DirectorySeparatorChar,
        [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Publish directory must stay inside the project.'
}
if (Test-Path -LiteralPath $publish) {
    Remove-Item -LiteralPath $publish -Recurse -Force
}

$project = Join-Path $projectRoot 'PolarBash.csproj'
& dotnet restore $project -r win-x64 --ignore-failed-sources -p:NuGetAudit=false
if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed.' }
& dotnet publish $project -c Release -r win-x64 --self-contained true --no-restore -o $publish
if ($LASTEXITCODE -ne 0) { throw 'dotnet publish failed.' }

if (-not $InnoCompiler) {
    $known = @(
        (Join-Path $env:LOCALAPPDATA 'Programs\Inno Setup 6\ISCC.exe'),
        (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'),
        (Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe')
    )
    $InnoCompiler = $known | Where-Object { $_ -and (Test-Path -LiteralPath $_) } | Select-Object -First 1
    if (-not $InnoCompiler) {
        $command = Get-Command ISCC.exe -ErrorAction SilentlyContinue
        if ($command) { $InnoCompiler = $command.Source }
    }
}
if (-not $InnoCompiler -or -not (Test-Path -LiteralPath $InnoCompiler)) {
    throw 'Inno Setup 6 ISCC.exe was not found. Install Inno Setup or pass -InnoCompiler.'
}

& $InnoCompiler /Q (Join-Path $projectRoot 'installer\PolarBash.iss')
if ($LASTEXITCODE -ne 0) { throw 'Inno Setup compilation failed.' }
