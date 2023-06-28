param(
    [string]$publishDir,
    [string]$publishProfilePath
)

# Load the publish profile XML
[xml]$publishProfile = Get-Content -Path $publishProfilePath

# Update the PublishDir property
$publishProfile.Project.PropertyGroup.PublishDir = $publishDir

# Save the updated publish profile
$publishProfile.Save($publishProfilePath)
