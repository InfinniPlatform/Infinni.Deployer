# Infinni.Deployer

## Commands

### List

`deployer list App --available` - lists available versions of application package

`deployer list App --available --prerelease` - lists available prerelese versions of application package (e.g App.1.2.3.4-prerelease)

`deployer list App --installed` - lists installed application packages

`deployer list App --installed --prerelease` - lists installed prerelese application packages (e.g App.1.2.3.4-prerelease)

### Install

`deployer install App.1.2.3.4` - installs application package

`deployer install App.1.2.3.4 App2.1.2.3.4 App3.1.2.3.4` - installs multiple application packages

### Start

`deployer start App.1.2.3.4` - starts installed application package

`deployer start App.1.2.3.4 App2.1.2.3.4 App3.1.2.3.4` - starts multiple installed application packages

### Stop

`deployer stop App.1.2.3.4` - stop installed application package

`deployer stop App.1.2.3.4 App2.1.2.3.4 App3.1.2.3.4` - stop multiple installed application packages


### Delete

`deployer uninstall App.1.2.3.4` - uninstalls installed application package

`deployer uninstall App.1.2.3.4 App2.1.2.3.4 App3.1.2.3.4` - uninstalls multiple installed application packages