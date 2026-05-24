<p align="center">
  🌐 English | <a href="README_TR.md">Türkçe</a>
</p>

<h1 align="center">🎮 Valorant Instalocker GUI v1.0.0</h1>

<p align="center">
  A fast, reliable, and visually polished automatic agent locker for Valorant — now with a full GUI.
</p>

---

## 📸 Preview

![Dashboard](https://github.com/Berkwe/Valorant-instalocker/Assets/screenshots/dashboard.png)

---

## ✨ Features

### 🖥️ GUI Interface
- Modern and clean graphical user interface — you're no longer stuck with that garbage terminal window.
- **Dark & Light theme** support — switch between themes with a single click from the settings panel.

### 🔒 Agent Locking Modes
- **Lock Mode:** Instantly selects and locks your chosen agent. Classic instalock behavior.
- **Select-Only Mode:** Selects the agent without locking. Perfect for competitive play where you don't want to hard-lock — Valorant auto-locks when the timer runs out, so no AFK penalty.

### 🌍 Multi-Language Support
- Multiple language support built in, switchable from the settings menu at any time.

### 🔄 Crash & Disconnect Protection
- If a match gets cancelled or you disconnect during agent select, the instalocker automatically locks again.

### 🚫 Match Abandonment
- After your agent is locked in, you can abandon the match with a single button directly from the instalocker — Note: standard abandon penalties still apply.

### 🔁 Automatic Updates
- Powered by **Velopack** — the app checks for new versions on startup and updates itself automatically. No manual re-downloading needed.

---

## 🔜 Coming Soon

The following features from the TUI (terminal) version are planned for the GUI:

- **Map-Based Agent Selection (Macro Mode):** Automatically pick a different agent and mode for each map using profile files.
- **Profile System:** Save and quickly switch between multiple map-based profiles.
- **Desktop Shortcut Creator:** Create desktop shortcuts pre-configured with a specific agent and mode.
- **In-Game UI Support:** Lock your agent instantly through popups that appear directly inside the game.
- **Version Check & Update Alerts:** An optional notification system that alerts you when a new version is available.
- And much more from the TUI version...

---

## 📦 Installation

### 💿 Installer (Recommended)

Installs the app to your system with automatic update support via Velopack.

1. Download the latest release:
   [**Instalocker-GUI-Setup.exe**](https://github.com/Berkwe/Valorant-instalocker/releases/latest/download/Instalocker-GUI-Setup.exe)
2. Run the installer and follow the on-screen steps.
3. The app will launch automatically when installation is complete.

> The installer registers the app for automatic updates — new versions will be applied silently on startup.

---

### 📦 Portable Version

No installation required. Download, extract, and run.

1. Download the latest release:
   [**Instalocker-GUI-Portable.zip**](https://github.com/Berkwe/Valorant-instalocker/releases/latest/download/Instalocker-GUI-Portable.zip)
2. Extract the archive to any folder.
3. Run `Instalocker-GUI.exe`.

> ⚠️ Windows Defender, your antivirus, or SmartScreen may flag the file as unlicensed. This is a false positive commonly seen with packaged C# applications. You can build from source if you prefer.

---

## 🏗️ Building from Source

### Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- Windows 10/11
- Git (optional)

### Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Berkwe/Valorant-instalocker
   cd Valorant-instalocker-TUI
   ```

2. **Build the project:**
   ```bash
   dotnet build --configuration Release
   ```

3. **Run:**
   ```bash
   dotnet run --project Instalocker.GUI
   ```

   Or navigate to `bin/Release/net8.0/` and run `Instalocker.GUI.exe` directly.

### Self-Contained Publish (Single EXE)

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

---

## 🛠️ Usage

### Selecting an Agent & Mode

1. Launch the application.
2. Choose your agent from the list.
3. Select a mode:
   - **Mode 1 – Lock:** Selects and locks the agent immediately.
   - **Mode 2 – Select Only:** Selects the agent but does not lock. Valorant auto-locks when the timer runs out.
4. Click the **Lock** button and queue up in Valorant.

### Abandoning a Match

After your agent is locked in, the **Abandon Game** button becomes visible. Click it to abandon the current match. Standard penalty rules still apply.

### Themes

Open the **Settings** menu and toggle between **Dark** and **Light** theme. The change applies instantly.

### Language

Open **Settings → Language** and choose your preferred language from the list. The UI reloads immediately.

---

## ⓘ Feedback & Bug Reports

For bug reports, feature requests, or performance issues, please use the [**Issues**](https://github.com/Berkwe/Valorant-instalocker/issues) page.

---

## 🖤 Credits & Thanks

- Thanks to the [**Radiant Connect**](https://radiantconnect.ca/) project for the Valorant client communication layer.

---

## 🌟 Other Projects

- [Valorant Instalocker TUI](https://github.com/Berkwe/Valorant-instalocker-TUI)
- [ADB Brute-Force](https://github.com/Berkwe/ADB-bruteforce)
- [Audio Converter](https://github.com/Berkwe/Audio-converter)

---

## 📞 Contact

<a href="https://discord.gg/Xagnh5aYSy" target="blank"><img align="center" src="https://raw.githubusercontent.com/rahuldkjain/github-profile-readme-generator/master/src/images/icons/Social/discord.svg" alt="Berkwe" height="30" width="40" /></a>

---

## 📝 License

This project is licensed under the [MIT License](LICENSE).

---

## ⚠️ Disclaimer

This software is developed solely for **educational and personal use**. All risks arising from the use of this software — including but not limited to in-game bans, account restrictions, or data loss — are the **sole responsibility of the user**. The developer cannot be held liable for any damages resulting from the use of this tool or from its potential violation of third-party terms of service (e.g. Riot Games). By using this tool, you agree to these terms.

---

### 🔑 Keywords
valorant instalocker, valorant gui instalocker, valorant auto lock, valorant agent locker, valorant instalock script, valorant agent picker, valorant instalocker gui, csharp valorant tool, radiant connect