# TD_Game_Project

a little tower defence game project in C# from scratch for windows and linux


building the game :

bash :


cd TD_Game_Project

dotnet new console

dotnet add package Raylib-cs

dotnet restore


build 

for windows :
  dotnet publish -r win-x64 --self-contained true -c Release

for linux : 
  dotnet publish -r linux-x64 --self-contained true -c Release
