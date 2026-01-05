:: GeneratePackages.cmd
:: Generate the Linux and Windows packages for download at the repository.
SET FRF=C:\Files\Dropbox\Develop\Active\FindAndReplaceInFiles\FindAndReplaceInFiles\bin\Debug\net8.0\FindAndReplaceInFiles.exe

"%FRF% /patternfile:VersionTransfer.json"

dotnet publish -c Release -r win-x64 --self-contained false
dotnet publish -c Release -r linux-x64 --self-contained true
