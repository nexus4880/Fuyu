# Fuyu: Build

## Setup

### Local project

> Terminal

```
git clone https://github.com/project-fika/fuyu
dotnet restore
```

> Visual Studio Code

1. Explorer > Clone repository > https://github.com/project-fika/fuyu
2. Terminal > Run Task... > "dotnet: restore"

### Repository CI/CD

1. Github > Generate new PAT
   1. Go [here](https://github.com/settings/tokens/new)
      - note: `FUYU_WORKFLOWS`
      - permissions: `repo` (all), `workflow` (all)
   2. Once PAT is generated, copy the token to clipboard
2. Github > Repositories > Fuyu > Settings > Secrets > Actions
3. New repository secret
   - name: `FUYU_WORKFLOWS`
   - field: paste your token

## Build

> Terminal

```
dotnet build
```

> Visual Studio Code

Terminal > Run Build Task...

## Test

> Terminal

```
dotnet build
```

> Visual Studio Code

Terminal > Run Task... > Fuyu: Test

## Usage (Escape From Tarkov)

### 1. Build release

> Terminal

1. `dotnet publish`

> Visual Studio Code

1. Terminal > Run Build Task... dotnet: publish

### 2. Putting everything in place

1. Copy-paste `Fuyu.Backend/bin/Release/net9.0/win-x64/publish/Fuyu.Backend.exe` into `<gamedir>`
2. Copy-paste `Fuyu.Launcher/bin/Release/net9.0-windows/win-x64/publish/Fuyu.Launcher.exe` into `<gamedir>`
3. Create folder `<gamedir>/Fuyu/`
4. Copy-paste `Mods/` into `<gamedir>/Fuyu/`
5. Delete `bin` and `obj` inside `<gamedir>/Fuyu/Mods/**/*`
6. Copy-paste `Fuyu.Modding.NLog/bin/Release/net48/publish/*.dll` into `<gamedir>/EscapeFromTarkov_data/Managed/`
7. Copy-paste `Fuyu.Common.Client/bin/Release/net48/publish/*.dll`  into `<gamedir>/EscapeFromTarkov_data/Managed/`
