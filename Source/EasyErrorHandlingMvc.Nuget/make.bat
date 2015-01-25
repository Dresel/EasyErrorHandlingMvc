mkdir input\lib\net40
del /Q input\lib\net40\*.*
msbuild .\..\EasyErrorHandlingMvc\EasyErrorHandlingMvc.csproj /p:Configuration=Release;OutputPath=.\..\EasyErrorHandlingMvc.Nuget\input\lib\net40
mkdir output
..\.nuget\nuget.exe pack /o output .\EasyErrorHandlingMvc.nuspec