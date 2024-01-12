### Make targets of this file by Windows NMAKE tool.
### NMAKE tool is installed alongside Visual Studio 2022, for example.
### By Windows start menu, launch:
### `Developer Command Prompt for VS 2022`

build:
	go build

clean:
	go clean

prepare:
	go mod tidy

install:
	copy "logic.exe" "%userprofile%\go\src\Hollow\RhinoCommon\Hollow\Hollow\bin\Release\net48"
	copy "logic.exe" "%userprofile%\go\src\Hollow\RhinoCommon\Hollow\Hollow\bin\Debug\net48"
