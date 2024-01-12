### Make targets of this file by Windows NMAKE tool.
### NMAKE tool is installed alongside Visual Studio 2022, for example.
### By Windows start menu, launch:
### `Developer Command Prompt for VS 2022`

build:
	cd %userprofile%\go\src\Hollow\RhinoCommon\Hollow
	dotnet build Hollow.sln -c Release -f net48 -p:Version=1.0.0.0

# To delete outdated files.
clean:
	cd %userprofile%\go\src\Hollow\RhinoCommon\pkg\dist
	del *.yak 2> NUL
	del *.rhp 2> NUL
	del *.dll 2> NUL
	del *.exe 2> NUL
	del *.bat 2> NUL
	del NULL 2> NUL
	del *.pdb 2> NUL
	
# To create the manifest file, if `manifest.yml` doesn't exist already.
# If manifest exists, the version must be incremented before each push to server.
manifest:
	"C:\Program Files\Rhino 7\System\Yak.exe" spec

# Install the binaries inside the distribution folder.
install:
	cd %userprofile%\go\src\Hollow\RhinoCommon\pkg\dist
	xcopy /E "%userprofile%\go\src\Hollow\RhinoCommon\Hollow\Hollow\bin\Release\net48\*" .>NUL
	del Hollow.pdb

# To create plugin package.
pkg:
	cd %userprofile%\go\src\Hollow\RhinoCommon\pkg\dist
	"C:\Program Files\Rhino 7\System\Yak.exe" build --platform win

# To log in before push to server, if not already logged in:
login:
	"C:\Program Files\Rhino 7\System\Yak.exe" login

# To push plugin package to Rhino3D server.
push:
	cd %userprofile%\go\src\Hollow\RhinoCommon\pkg\dist
	echo off > temp.bat
    for /f "delims=" %%i in ('dir /b /od /a-d hollow-*-win.yak') do echo "C:\Program Files\Rhino 7\System\Yak.exe" push "%%i" > temp.bat
    call temp.bat
    del temp.bat

# To search for plugin on Rhino3D server.
search:
	"C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease Hollow

# To create a ZIP file to be uploaded to Patreon shop.
zip:
	cd %userprofile%\go\src\Hollow\RhinoCommon\pkg
	powershell Compress-Archive -Force dist Hollow.zip

# To create and distribute the release as a Rhino3D plugin package.
distribute-server: build clean install pkg push search

# To create and distribute the release as a Rhino3D plugin ZIP file.
distribute: build clean install pkg zip
