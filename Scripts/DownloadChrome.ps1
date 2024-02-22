param(
    [Parameter(Mandatory=$true)]
    [string]$version,
    
    [Parameter(Mandatory=$true)]
    [string]$path
)

$chromeUrl = "https://storage.googleapis.com/chrome-for-testing-public/$version/win32/chrome-win32.zip"
$zipPath = Join-Path $path "chrome-win32.zip"
Invoke-WebRequest -Uri $chromeUrl -OutFile $zipPath

Expand-Archive -Path $zipPath -DestinationPath $path -Force

$destinationDirectory = Join-Path $path "ChromeForTesting"

if (-not (Test-Path $destinationDirectory)) {
    New-Item -Path $destinationDirectory -ItemType Directory
}

$chromePath = Join-Path $path "chrome-win32"
Get-ChildItem -Path $chromePath | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination $destinationDirectory -Recurse -Force
}

Remove-Item -Path $zipPath -Force
Remove-Item -Path $chromePath -Force -Recurse
