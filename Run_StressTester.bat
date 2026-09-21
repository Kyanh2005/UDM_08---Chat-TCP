@echo off
chcp 65001 > nul
title TCP Stress Test Tool
echo Đang khởi động công cụ Stress Test...
cd /d "%~dp0"
dotnet run --project Code\ChatApp.StressTester\ChatApp.StressTester.csproj
pause
