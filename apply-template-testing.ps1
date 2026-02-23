# This uses a Windows-based secure way to store and retrieve your credentials to execute the intent-cli tool.
# To encode and encrypt your password run the following Powershell script:
# $encodedEncryptedPassword = [System.Convert]::ToBase64String([System.Security.Cryptography.ProtectedData]::Protect([System.Text.Encoding]::Unicode.GetBytes("Your password"), $null, "CurrentUser"))
$intent_architect_user = $Env:INTENT_PACKAGER_USERNAME
$intent_architect_password = [System.Text.Encoding]::Unicode.GetString([System.Security.Cryptography.ProtectedData]::Unprotect([System.Convert]::FromBase64String($Env:INTENT_PACKAGER_PASSWORD), $null, "CurrentUser"))
$intent_solution = 'Tests/Intent.Modules.NET.Tests.isln'


intent-cli "apply-pending-changes" "$($intent_architect_user)" "$($intent_architect_password)" "$($intent_solution)" 
