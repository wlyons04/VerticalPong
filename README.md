# VerticalPong

--- Build Instructions ---

Pre-built binaries for Windows and Linux can be downloaded from the Releases section of this repository.

To build from source code use the corresponding commands below:

Windows: dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained

Linux: dotnet publish -c Release -r linux-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained

macOS: dotnet publish -c Release -r osx-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained

Official MonoGame build instructions: https://docs.monogame.net/articles/packaging_games.html
