param(
    [string]$version,
    [string]$path
)

$chromedriverUrl = "https://chromedriver.storage.googleapis.com/$version/chromedriver_win32.zip"
$zipPath = Join-Path $path "chromedriver_win32.zip"

Invoke-WebRequest -Uri $chromedriverUrl -OutFile $zipPath

Expand-Archive -Path $zipPath -DestinationPath $path -Force
Remove-Item -Path $zipPath
