param(
    [string]$version,
    [string]$path
)

$chromedriverUrl = "https://storage.googleapis.com/chrome-for-testing-public/$version/win32/chromedriver-win32.zip"
$zipPath = Join-Path $path "chromedriver-win32.zip"
Invoke-WebRequest -Uri $chromedriverUrl -OutFile $zipPath

Expand-Archive -Path $zipPath -DestinationPath $path -Force

$chromedriverPath = Join-Path $path "chromedriver-win32"
Move-Item -Path (Join-Path $chromedriverPath "chromedriver.exe") -Destination $path -Force

Remove-Item -Path $zipPath
Remove-Item -Path $chromedriverPath -Recurse -Force
