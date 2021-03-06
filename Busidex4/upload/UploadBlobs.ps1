function UploadBlobs($container, $directory, $virtualPath)
{  
    Write-Output "Uploading $directory files..."
    Foreach ($file in Get-ChildItem $directory)
    {
        if($file.PSIsContainer)
        {
            UploadBlobs $container $file.fullname ($virtualPath + $file.name + '/')
        }
        else
        {
            Write-Output "Uploading $file"
            $blob = $container.GetBlobReference($virtualPath + $file.name)
            $blob.Properties.ContentType = GetContentTypeFromExtension $file.extension
            $blob.Properties.CacheControl = "public, max-age=31556926"
            $blob.UploadFile($file.fullName)
        }
        SetCacheControlNoCache $container $file.fullname
    }
}

function SetCacheControlNoCache($container, $resource)
{
    $blob = $container.GetBlobReference($resource)
    $blob.Properties.CacheControl = "public, max-age=0";
    #$blob.SetProperties()
}

function GetContentTypeFromExtension([string]$extension)
{   
    switch ($extension)
    {
        ".png" { return "image/png" }
        ".htm" { return "text/html" }
        ".pfx" { return "application/x-pkcs12" }
        ".xml" { return "text/xml" }
		".css" { return "text/css" }
		".jpg" { return "image/jpeg" }
		".jpeg" { return "image/jpeg" }
		".bmp" { return "image/bmp" }
		".js" { return "text/x-javascript" }
		".zip" { return "application/zip" }
	    ".flv" { return "video/x-flv" }
        ".mp4" { return " video/mp4" }
    }
	
    Write-Output "application/octet-stream"
}

$scriptDir = (split-path $myinvocation.mycommand.path -parent)
Set-Location $scriptDir

$sdkPath = resolve-path "C:\Program Files\Microsoft SDKs\Windows Azure\.NET SDK\v2.3\ref"

write-host $sdkPath

if ($sdkPath -is [Array]) 
  { $refFolder = $sdkPath[-1] }
else 
  {$refFolder = [string]$sdkPath}

[Reflection.Assembly]::LoadFile($refFolder + ‘\Microsoft.WindowsAzure.StorageClient.dll’)

[xml] $xml = get-Content "./configuration.xml"
$subId = $xml.settings.subscriptionId
$storageAccount = $xml.settings.storageAccount.name
$storageAccountKey = $xml.settings.storageAccount.key
$containerName = $xml.settings.containerName
$sourceFolder = $xml.settings.sourceFolder

Write-Host "Uploading files..."

$credentials = New-Object Microsoft.WindowsAzure.StorageCredentialsAccountAndKey -ArgumentList $storageAccount, $storageAccountKey
$account = New-Object Microsoft.WindowsAzure.CloudStorageAccount -ArgumentList $credentials, TRUE
$client = [Microsoft.WindowsAzure.StorageClient.CloudStorageAccountStorageClientExtensions]::CreateCloudBlobClient($account)

$timeout = New-Object System.TimeSpan -ArgumentList 0, 10, 0
#set the timeout to 5 minutes. this allows us to upload large blobs.
$client.Timeout = $timeout

$container = $client.GetContainerReference($containerName)
$container.CreateIfNotExist()
# set public permissions, only if necessary
$publicPermission = New-Object Microsoft.WindowsAzure.StorageClient.BlobContainerPermissions
$publicPermission.PublicAccess = [Microsoft.WindowsAzure.StorageClient.BlobContainerPublicAccessType]::Blob
#$container.SetPermissions($publicPermission)
UploadBlobs $container $sourceFolder ''
          

Write-Host "Done!"
