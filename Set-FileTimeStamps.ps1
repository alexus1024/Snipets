Function Set-FileTimeStamps
{

 Param (

    [Parameter(mandatory=$true)]

    [string[]]$path,

    [datetime]$date = (Get-Date))

    Get-ChildItem -Recurse -Path $path |

    ForEach-Object {

     $_.CreationTime = $date

     $_.LastAccessTime = $date

     $_.LastWriteTime = $date }

} #end function Set-FileTimeStamp

