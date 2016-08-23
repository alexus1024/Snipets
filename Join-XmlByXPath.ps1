# Search itemes by xpath in input files and saves them into output file
Function Join-XmlByXPath
{
 Param (
    [string]$inputDir = "./",
    [string]$inputFilter = "*.xml",
    [Parameter(Mandatory=$True)]
    [string]$outputFile,
    [Parameter(Mandatory=$True)]
    [string]$xpath,
    [string]$namespaces = @{s="http://www.w3.org/2003/05/soap-envelope"},
    [string]$prefix = '<?xml version="1.0" encoding="utf-8"?><root>',
    [string]$postfix = '</root>'
)

    if($prefix){
        $prefix | Out-File -FilePath $outputFile -Encoding utf8
    }

    foreach($file in Get-ChildItem -Path $inputDir -Filter $inputFilter){   

    $tnvedNodes = Select-Xml $file.FullName -XPath $xpath -Namespace $namespaces
    
    Write-Host "$($file.Name) ($($tnvedNodes.Length) items)" 


        foreach($node in $tnvedNodes){
           $node.Node.OuterXml | Out-File -FilePath $outputFile -Append -Encoding utf8

        }
    }

    if($postfix){
        $postfix | Out-File -FilePath $outputFile -Append -Encoding utf8
    }


} #end function  Join-XmlByXPath
