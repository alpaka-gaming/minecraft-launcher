dotnet restore
cd src

dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true -c Release --output ../publish/win-x64
cd ../publish/win-x64
7z a -tzip ../updater_win-x64.zip *
cd ../../src

dotnet publish -r osx-x64 -p:PublishSingleFile=true --self-contained true -c Release --output ../publish/osx-x64
cd ../publish/osx-x64
7z a -tzip ../updater_osx-x64.zip *
cd ../../src

dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained true -c Release --output ../publish/linux-x64
cd ../publish/linux-x64
7z a -tzip ../updater_linux-x64.zip *
cd ../../src