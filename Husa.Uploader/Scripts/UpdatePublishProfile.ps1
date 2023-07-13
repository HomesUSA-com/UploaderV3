param(
    [string]$publishDir,
    [string]$publishProfilePath,
    [string]$publishUrl
)

# Load the publish profile XML
[xml]$publishProfile = Get-Content -Path $publishProfilePath

# Update the PublishDir property
$publishProfile.Project.PropertyGroup.PublishDir = $publishDir

# Update the PublishUrl property
$publishProfile.Project.PropertyGroup.PublishUrl = $publishUrl

# Save the updated publish profile
$publishProfile.Save($publishProfilePath)
