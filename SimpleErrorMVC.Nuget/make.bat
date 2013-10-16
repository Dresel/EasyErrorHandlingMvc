mkdir input\lib\net40
del /Q input\lib\net40\*.*

msbuild ..\SimpleErrorMVC\SimpleErrorMVC.csproj /p:Configuration=Release;OutputPath=..\SimpleErrorMVC.Nuget\input\lib\net40

mkdir output
..\.nuget\nuget.exe pack /o output .\SimpleErrorMVC.nuspec