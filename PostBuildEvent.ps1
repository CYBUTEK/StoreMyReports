# initialise variables
$version = ([System.Diagnostics.FileVersionInfo]::GetVersionInfo("StoreMyReports.dll").FileVersion)
$solutionDir = (Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path)
$outputDir = ($solutionDir + "\Output\*")
$docsDir = ($solutionDir + "\Docs\*")
$gameDir = ($solutionDir + "\..\..\KSP-Environment")
$gameDataDir = ($gameDir + "\GameData")
$gameFile = ($gameDir + "\KSP_x64.exe")
$zipFile = ($solutionDir + "\Release\StoreMyReports-" + $version + ".zip")

# create distributable zip archive
Compress-Archive -Path $outputDir -DestinationPath $zipFile -Force
Compress-Archive -Path $docsDir -DestinationPath $zipFile -Update

# check if game installation exists
if (Test-Path $gameFile)
{
    # copy to game install location for debugging
    Copy-Item -Path $outputDir -Destination $gameDataDir -Recurse -Force
}