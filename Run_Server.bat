@echo off
chcp 65001 > nul
title TCP Chat Server Core
echo ========================================================
echo          ĐANG KHỞI ĐỘNG TCP CHAT SERVER...
echo ========================================================
cd /d "%~dp0"
dotnet run --project Code\ChatApp.Server\ChatApp.Server.csproj
pause
