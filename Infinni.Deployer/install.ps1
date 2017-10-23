param
(
  [Parameter(HelpMessage = "Teamcity username.")]
  [string] $UserName
)

$fileName = 'Infinni.Deployer.win.zip'

Remove-Item Infinni.Deployer.dist -Recurse -Force
Remove-Item $fileName

$cred = Get-Credential -UserName $UserName -Message Enter TeamCity password

Invoke-WebRequest `
  -Uri "http://teamcity.infinnity.ru/app/rest/builds/buildType:%28id:InfinniDeployer_Publish%29,status:SUCCESS/artifacts/content/$fileName" `
  -ContentType 'application/zip' `
  -Method GET `
  -Credential $cred `
  -OutFile $fileName

Expand-Archive $fileName -DestinationPath 'Infinni.Deployer.dist'