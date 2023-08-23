param(
    [string]$version,
    [string]$path
)

$chromedriverUrl = "https://edgedl.me.gvt1.com/edgedl/chrome/chrome-for-testing/$version/win32/chromedriver-win32.zip"
$zipPath = Join-Path $path "chromedriver-win32.zip"
Invoke-WebRequest -Uri $chromedriverUrl -OutFile $zipPath

Expand-Archive -Path $zipPath -DestinationPath $path -Force

$chromedriverPath = Join-Path $path "chromedriver-win32"
Move-Item -Path (Join-Path $chromedriverPath "chromedriver.exe") -Destination $path -Force

Remove-Item -Path $zipPath
Remove-Item -Path $chromedriverPath -Recurse -Force
