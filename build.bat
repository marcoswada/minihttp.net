@echo off
if exist minihttpserver.exe del minihttpserver.exe
c:\windows\Microsoft.NET\Framework64\v4.0.30319\vbc minihttpserver.vb /win32icon:ico/logo.ico
if exist minihttpserver.exe minihttpserver