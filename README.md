# NetGore

A free, open-source 2D online RPG engine built with C# and SFML. NetGore provides a strong and flexible foundation for creating side-scrolling and top-down 2D online games using a client-server architecture.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Platform](https://img.shields.io/badge/platform-Linux%20%7C%20Windows%20%7C%20macOS-informational)
![License](https://img.shields.io/badge/license-GPL-blue)

> **Note**: This project is no longer under active development by its original creator but remains maintained by the community.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Quick Start](#quick-start)
- [Architecture](#architecture)
- [Building the Project](#building-the-project)
- [Running the Game](#running-the-game)
- [Project Structure](#project-structure)
- [Recent Updates](#recent-updates)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [License](#license)

---

## 🎮 Overview

NetGore is a complete 2D game development framework designed for creating online RPGs. It provides:

- **Client**: Game client with SFML.Net 2.6.0 rendering
- **Server**: Game server with networking and world management
- **Editor**: Visual map and content editing tools
- **Database**: MySQL-backed game data storage

The engine supports both side-scrolling and top-down perspectives, making it suitable for a wide variety of 2D RPG styles.

---

## ✨ Features

### Core Engine
- **SFML.Net 2.6.0** rendering and audio
- **Client-server architecture** with network synchronization
- **MySQL database** integration for persistent game data
- **Lidgren networking** for efficient multiplayer
- **Cross-platform support** (Linux, Windows, macOS)

### Game Features
- Character management and leveling
- Inventory and equipment system
- Skill and stat systems
- Quests and NPCs
- Guilds and peer trading
- Map editing and world management
- Particle effects and animations
- Sound and music support

### Editors
- **WinForms Editor**: Original editor for Windows (fully functional)
- **Avalonia Editor**: Cross-platform editor with basic UI skeleton (work in progress)

---

## 🚀 Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- MySQL Server (for running the server)
- SFML native libraries (for graphics and audio)

### Build the Solution

```bash
# Clone the repository
git clone <repository-url>
cd netgore

# Build all projects
dotnet build

# Or build specific projects
dotnet build DemoGame.Client/DemoGame.Client.csproj
dotnet build DemoGame.Server/DemoGame.Server.csproj
dotnet build DemoGame.Editor.Avalonia/DemoGame.Editor.Avalonia.csproj
```

### Run the Server

```bash
cd DemoGame.Server
dotnet run
```

### Run the Client

```bash
cd DemoGame.Client
dotnet run
```

### Run the Editor

**Avalonia Editor (Cross-platform, work in progress):**
```bash
cd DemoGame.Editor.Avalonia
dotnet run
```

**Note**: The Avalonia editor is still a work in progress. Use the WinForms editor (`DemoGame.Editor`) for full functionality.

**WinForms Editor (Windows only):**
```bash
cd DemoGame.Editor
dotnet run
```

### Quick Launch Scripts

The repository includes batch files for quick launching:

- `Run Server.bat` - Start the game server
- `Run Client.bat` - Start the game client  
- `Run Editor.bat` - Start the WinForms editor
- `Run InstallationValidator.bat` - Validate installation

---

## 🏗️ Architecture

### Solution Structure

```
NetGore/
├── NetGore/                  # Core game engine library
├── NetGore.Db/              # Database layer
├── NetGore.Features/        # Shared features
├── NetGore.Features.Client/ # Client-specific features
├── NetGore.Features.Server/ # Server-specific features
├── NetGore.Features.Editor/ # Editor-specific features
├── NetGore.Editor/          # WinForms editor utilities
├── DemoGame/                # Game logic and database objects
├── DemoGame.Client/         # Game client
├── DemoGame.Server/         # Game server
├── DemoGame.Editor/         # WinForms editor
├── DemoGame.Editor.Avalonia/ # Cross-platform Avalonia editor
├── NetGore.Tests/           # Unit tests
└── DevContent/              # Game assets
```

### Client-Server Model

```
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│   Client    │ ◄─────► │   Server    │ ◄─────► │   MySQL    │
│   (SFML)    │  TCP    │ (Lidgren)   │   SQL   │  Database  │
└─────────────┘         └─────────────┘         └─────────────┘
```

---

## 🔨 Building the Project

### Build Requirements

- .NET 8.0 SDK
- SFML native libraries (platform-specific, via SFML.Net 2.6.0 NuGet package)
- MySQL Server (for server-side features)

### Build Output

All projects build to a shared `bin/` directory structure:

```
bin/
├── net8.0/
│   ├── NetGore.dll
│   ├── DemoGame.Client.exe
│   ├── DemoGame.Server.exe
│   └── Content/               # Game assets
```

### Content Files

Game content is automatically copied from `DevContent/` during build.

---

## 🎮 Running the Game

### Starting the Server

1. Configure the database connection in `DemoGame.Server/DbSettings.dat`
2. Run the server: `dotnet run` in `DemoGame.Server/`
3. The server will start listening for client connections

### Starting the Client

1. Ensure the server is running
2. Run the client: `dotnet run` in `DemoGame.Client/`
3. Connect to the server and create a character

### Configuration

- **Server settings**: `DemoGame.Server/app.config`
- **Client settings**: `DemoGame.Client/app.config`
- **Database settings**: `DemoGame.Server/DbSettings.dat`

---

## 📁 Project Structure

### Core Libraries

- **NetGore**: Core game engine with rendering, networking, and game logic
- **NetGore.Db**: Database abstraction layer with MySQL support
- **NetGore.Features**: Shared game features (stats, skills, quests, etc.)

### Applications

- **DemoGame.Client**: Game client using SFML.Net 2.6.0 for rendering
- **DemoGame.Server**: Game server with world simulation
- **DemoGame.Editor**: Visual map and content editor (Windows)
- **DemoGame.Editor.Avalonia**: Cross-platform editor skeleton (work in progress)

### Demo Game

- **DemoGame**: Game-specific logic, database objects, and content

---

## 🆕 Recent Updates

### .NET 8 Migration (2025) ✅

The entire project has been modernized from .NET Framework 4.0 to .NET 8.0:

- ✅ Modern SDK-style projects
- ✅ Cross-platform support (Linux, Windows, macOS)
- ✅ Updated NuGet packages
- ✅ MySqlConnector integration
- ✅ Improved performance and compatibility

**See**: `MARKDOWN NOTES/DOTNET8_MIGRATION_COMPLETE.md`

### Avalonia Editor (2025)

A cross-platform editor skeleton has been created:

- ⚠️ Basic UI structure
- ⚠️ Docking system set up
- ⚠️ Some panels implemented
- ⚠️ Work in progress - not fully functional

**Note**: This is still under active development. Use the WinForms editor for full functionality.

**See**: `DemoGame.Editor.Avalonia/README.md` for current status

### Modern Dependencies

- Migrated to SFML.Net 2.6.0 via NuGet
- Migrated from MySql.Data to MySqlConnector for cross-platform support
- All database queries updated
- Improved async/await support

---

## 📚 Documentation

### Migration Guides

Located in `MARKDOWN NOTES/`:

- **START_HERE.md** - Quick start guide for the Avalonia editor
- **DOTNET8_MIGRATION_COMPLETE.md** - .NET 8 migration summary
- **AVALONIA_MIGRATION_COMPLETE.md** - Editor migration details
- **SFML_MIGRATION_STATUS_FINAL.md** - Graphics library status

### Editor Documentation

- **DemoGame.Editor.Avalonia/README.md** - Avalonia editor status and progress
- **Note**: The Avalonia editor is still under development and not fully functional

### Code Documentation

- Inline XML documentation in source files
- Architecture patterns documented in markdown notes

---

## 🔧 Development

### Project Dependencies

**Core Libraries:**
- SFML.Net 2.6.0 via NuGet (graphics and audio)
- MySqlConnector 2.3.7 (database)
- Lidgren.Network (networking)
- log4net (logging)

**Editor (Avalonia, work in progress):**
- Avalonia 11.0 (cross-platform UI)
- Dock.Avalonia 11.0 (docking support)
- CommunityToolkit.Mvvm (MVVM helpers)

**Note**: Basic skeleton created, full functionality pending

### Building on Linux

```bash
# Install SFML libraries
sudo apt-get install libsfml-dev

# Build the solution
dotnet build NetGore.sln

# Run the Avalonia editor
cd DemoGame.Editor.Avalonia
dotnet run
```

### Building on Windows

```bash
# Visual Studio 2022 or later
# Open NetGore.sln

# Or use command line
dotnet build NetGore.sln
```

### Database Setup

1. Create a MySQL database for the server
2. Run the schema from `db.sql`
3. Configure connection string in server settings

---

## 🤝 Contributing

Contributions are welcome! This project benefits from community maintenance.

### Getting Started

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

### Development Guidelines

- Follow the existing code style
- Update documentation for new features
- Test on multiple platforms when possible
- Use meaningful commit messages

---

## 📄 License

This project is licensed under the GNU General Public License v2.0.

Original project: NetGore by NetGore contributors  
Original homepage: http://www.netgore.com/

---

## 🙏 Acknowledgments

- **NetGore creators** for the original engine
- **SFML community** for excellent graphics library
- **Avalonia team** for the cross-platform UI framework
- **Community contributors** who maintain and improve NetGore

---

## 📞 Support

- Check existing documentation in `MARKDOWN NOTES/`
- Review the Avalonia editor README
- Examine the migration guides for technical details
- Open issues for bugs or feature requests

---


