:: �������ַ���
echo %1
:: ��Ŀ������ַ
echo %2

:: ������
set nupkg=""

:: ����
dotnet msbuild %2 /p:Configuration=Release

:: ���
dotnet pack %2 -c Release --output nupkgs

:: ���°�����
for %%a in (dir /s /a /b "./nupkgs/%1") do (set nupkg=%%a)

:: ���Ͱ�
nuget push nupkgs/%nupkg% bc97b237-abc2-33c3-a0cd-c34dde4c5cf9 -source http://192.168.15.118:8099/repository/nuget-hosted/