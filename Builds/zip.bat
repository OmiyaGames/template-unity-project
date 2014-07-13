@ECHO off
IF NOT [%1]!=[] (
	CD /D %1
)
FOR /D %%G IN (*) DO "C:\Program Files\7-Zip\7z.exe" a "%%G.zip" "%%G\*"
REM FOR /D %%G IN (*) DO DEL "%%G"
