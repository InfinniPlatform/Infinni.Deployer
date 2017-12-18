param
(
  [Parameter(HelpMessage = "Build number.")]
  [int] $BuildNumber = 0,

  [Parameter(HelpMessage = "Commit.")]
  [string] $Commit = 'null',

  [Parameter(HelpMessage = "Path to the Infinni.Deployer.csproj.")]
  [string] $ProjectPath = '..\Infinni.Deployer\Infinni.Deployer.csproj'
)

[xml] $csproj = Get-Content $ProjectPath

foreach ($group in $csproj.Project.PropertyGroup | Where-Object {$_.AssemblyVersion -ne $null}) {
  $group.AssemblyVersion = "$($group.Version).$BuildNumber"
  $group.FileVersion = "$($group.Version).$BuildNumber"

  $appVersion = $group.AssemblyVersion
}

$csproj.Save($ProjectPath)

Write-Host "##teamcity[setParameter name='app.version' value='$appVersion']"