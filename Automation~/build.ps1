param (
    [string]$projectPath,
    [ValidateSet("Windows", "Linux", "OSX", "ItchIO", "Yandex", "Android")]
    [string]$platform
)

# Original version:
# https://github.com/microsoft/unitysetup.powershell/blob/adec4336eec5b987e025782b6622fdb9cd6e7399/UnitySetup/UnitySetup.psm1#L2083
function Get-IsUnityError {
    param([string] $LogLine)

    # Detect Unity License error, for example:
    # BatchMode: Unity has not been activated with a valid License. Could be a new activation or renewal...
    if ( $LogLine -match 'Unity has not been activated with a valid License' ) {
        return $true
    }

    # Detect that the method specified by -ExecuteMethod doesn't exist, for example:
    # executeMethod method 'Invoke' in class 'Build' could not be found.
    if ( $LogLine -match 'executeMethod method .* could not be found' ) {
        return $true
    }

    # Detect compilation error, for example:
    #   Assets/Errors.cs(7,9): error CS0103: The name `NonexistentFunction' does not exist in the current context
    if ( $LogLine -match '\.cs\(\d+,\d+\): error ' ) {
        return $true
    }

    # In the future, additional kinds of errors that can be found in Unity logs could be added here:
    # ...

    return $false
}


# Function to read new portion of the log file
function Read-NewLog {
    param (
        [string]$filePath,
        [int]$startPosition
    )
    # Open the file for reading
    $fs = [System.IO.File]::Open($filePath, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
    try {
        # Seek to the saved position
        $fs.Seek($startPosition, [System.IO.SeekOrigin]::Begin) | Out-Null

        # Read the new portion of the file
        $sr = New-Object System.IO.StreamReader($fs)

        $endPosition = (Get-Item $filePath).Length
        while ($fs.Position -lt $endPosition) {
            $line = $sr.ReadLine()

            if (Get-IsUnityError -LogLine $line) {
                Write-Host $line -ForegroundColor Red -Stream Error
            } else {
                Write-Host $line
            }
        }
        $startPosition = $fs.Position

        # Close the StreamReader
        $sr.Close()
        Write-Host $newContent
    }
    finally {
        # Close the FileStream
        $fs.Close()
    }
	return $startPosition
}

# Function to check and read the log file
function Check-AndReadLog {
    param (
        [string]$filePath,
        [int]$startPosition
    )
    if (Test-Path $filePath) {
        $currentLength = (Get-Item $filePath).Length
        if ($currentLength -ne $startPosition) {
            $startPosition = Read-NewLog -filePath $filePath -startPosition $startPosition
        }
    }
    return $startPosition
}

# If FolderPath is not provided, set it to the parent folder of the script's folder
if (-not $projectPath) {
    $projectPath = Get-Location
}

# Extract Unity version from ProjectVersion.txt
$versionFilePath = "$projectPath/ProjectSettings/ProjectVersion.txt"
$unityVersion = (Select-String -Path $versionFilePath -Pattern "m_EditorVersion: (.+)" | ForEach-Object { $_.Matches.Groups[1].Value }).Trim()

# Define Unity executable path based on extracted version
$unityPath = "C:\Program Files\Unity\Hub\Editor\$unityVersion\Editor\Unity.exe"

if (!(Test-Path $unityPath)) {
    Write-Host "Unity version $unityVersion not found at expected path: $unityPath"
    exit 1
}

# Define output paths for builds
$outputPath = "$projectPath/Builds/$platform"
$logFile = "$projectPath/Builds/$platform.log"

# Define build target argument
$buildTarget = switch ($platform) {
    "Windows" { "StandaloneWindows64" }
    "Linux" { "StandaloneLinux64" }
    "OSX" { "StandaloneOSX" }
    "ItchIO" { "WebGL" }
    "Yandex" { "WebGL" }
    "Android" { "Android" }
}

# Run Unity build process
$arguments = @(
    "-quit",
    "-batchmode",
    "-projectPath", $projectPath,
    "-executeMethod", "Leap.Forward.BuildScript.PerformBuild",
    "-buildTarget", $buildTarget,
    "-buildTargetName", $platform,
    "-logFile", $logFile
)

Write-Host "Starting Unity build for $platform using Unity version $unityVersion..."

if (Test-Path $logFile) {
    Remove-Item -Path $logFile -Force
}

# Start process asynchronously
$process = Start-Process -FilePath $unityPath -ArgumentList $arguments -NoNewWindow -PassThru

$lastPosition = 0
while (!$process.HasExited) {
    $lastPosition = Check-AndReadLog -filePath $logFile -startPosition $lastPosition
    Start-Sleep -Seconds 1
}
$lastPosition = Check-AndReadLog -filePath $logFile -startPosition $lastPosition

# Wait for Unity process to complete
$process | Wait-Process

# $exitCode = $process.ExitCode
# Write-Host "Unity quit with non-zero exit code: $exitCode"
# Write-Host $process.ExitCode

# if ( $exitCode -ne 0 ) {
#     Write-Error "Unity quit with non-zero exit code: $exitCode"
# }

Write-Host "Check logs at $logFile"

# Exit the script with the same exit code
exit $exitCode
