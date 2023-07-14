param(
    [string]$publishDir,
    [string]$publishProfilePath,
    [string]$publishUrl
)

# Load the publish profile XML
[xml]$publishProfile = Get-Content -Path $publishProfilePath

# Update the PublishDir property
$publishProfile.Project.PropertyGroup.PublishDir = $publishDir

# Update the PublishUrl and InstallUrl properties to the same value
$publishProfile.Project.PropertyGroup.PublishUrl = $publishUrl
$publishProfile.Project.PropertyGroup.InstallUrl = $publishUrl

# Save the updated publish profile
$publishProfile.Save($publishProfilePath)
