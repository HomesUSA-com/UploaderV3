param(
    [string]$revision,
    [string]$csprojPath,
    [string]$htmlFilePath,
    [string]$destinationFilePath
)

# Read .csproj file and extract application version
[xml]$csprojContent = Get-Content -Path $csprojPath
$applicationVersion = $csprojContent.Project.PropertyGroup.ApplicationVersion

# Split the application version on asterisk and keep the part before it
$applicationVersion = ($applicationVersion -split '\*')[0]

# Combine application version with revision
$version = "$applicationVersion$revision"

# Update HTML file
(Get-Content -Path $htmlFilePath -Raw) -replace '{VERSION}', $version |
Set-Content -Path $htmlFilePath

# Copy the updated HTML file to the destination directory
Copy-Item -Path $htmlFilePath -Destination $destinationFilePath
