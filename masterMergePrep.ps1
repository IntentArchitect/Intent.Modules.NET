Write-Host "Swapping to Master and cleaning:"

Read-Host "Press any key to continue"

git reset --hard
git clean -fdx
git checkout master
git pull