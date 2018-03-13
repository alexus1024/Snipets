$remoteScript = {
cd "SomeDir"
. "MSBuild.exe" SomeProj.csproj /t:Package

}
  
$s = New-PSSession -ComputerName crm-vsdev

# Copy-Item C:\file.txt -ToSession $s -Destination C:\

Invoke-Command -Session $s $remoteScript 

Disconnect-PSSession $s
