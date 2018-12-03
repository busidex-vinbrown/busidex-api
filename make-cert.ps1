$cert = New-SelfSignedCertificate -DnsName busidex.cloudapp.net -CertStoreLocation "cert:\LocalMachine\My" -KeyLength 2048 -KeySpec "KeyExchange"
$password = ConvertTo-SecureString -String "ride9736" -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath "D:\Documents\busidex_servicebus.pfx" -Password $password
Export-Certificate -Type CERT -Cert $cert -FilePath D:\Documents\busidex_servicebus.cer