@echo off
chcp 65001 > nul
title TCP Chat Client
echo Đang khởi động giao diện Chat Client...
cd /d "%~dp0"
dotnet run --project Code\ChatApp.Client\ChatApp.Client.csproj
