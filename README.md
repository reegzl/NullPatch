<p align="center">
  <pre align="center">
███╗   ██╗██╗   ██╗██╗     ██╗     ██████╗  █████╗ ████████╗ ██████╗██╗  ██╗
████╗  ██║██║   ██║██║     ██║     ██╔══██╗██╔══██╗╚══██╔══╝██╔════╝██║  ██║
██╔██╗ ██║██║   ██║██║     ██║     ██████╔╝███████║   ██║   ██║     ███████║
██║╚██╗██║██║   ██║██║     ██║     ██╔═══╝ ██╔══██║   ██║   ██║     ██╔══██║
██║ ╚████║╚██████╔╝███████╗███████╗██║     ██║  ██║   ██║   ╚██████╗██║  ██║
╚═╝  ╚═══╝ ╚═════╝ ╚══════╝╚══════╝╚═╝     ╚═╝  ╚═╝   ╚═╝    ╚═════╝╚═╝  ╚═╝
  </pre>
</p>

<p align="center">
  <b>An advanced, colored CLI utility to streamline Windows Package Manager (winget) updates, package pins, interactive upgrades, and system safety guardrails. Created by REEGZL.</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows-blue?style=flat-square" alt="Platform Windows">
  <img src="https://img.shields.io/badge/Language-C%23-purple?style=flat-square" alt="Language C#">
  <img src="https://img.shields.io/badge/Version-v1.11-orange?style=flat-square" alt="Version v1.11">
  <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="License MIT">
</p>

---

## Overview

**NullPatch** is a custom terminal wrapper built in C# by **REEGZL**, designed to enhance the native Windows Package Manager (`winget`) experience. By default, standard terminal output can look monotonous and hard to parse. NullPatch intercepts `winget` execution, parses its output streams in real-time, and renders a clean, color-coded interface making it much easier to track upgrades, package IDs, and source pins at a glance. 

Version **v1.10** / **v1.11** introduces robust new capabilities including an interactive upgrades submenu, smart interactive uninstallation with system safety guardrails, background silent modes, and self-destruct routines.

---

## Features

* **Real-Time Stream Parsing:** Intercepts `winget` stdout/stderr seamlessly without sacrificing execution speed.
* **Interactive Upgrades & Management:** Features a dedicated `[UPGRADES]` submenu for interactive package selection, batch upgrades, and update checks.
* **Smart Interactive Uninstall & Guardrails:** Safely select and remove installed packages by number or batch. Built-in filters automatically block essential system components, core runtimes, and Windows internals to prevent system breakage.
* **Silent Background Mode:** Toggle background task execution for system-modifying commands (installs, updates, uninstalls) without opening redundant terminal windows or stealing window focus.
* **Self-Destruct Mode:** Cleanly remove the tool from your system with a built-in uninstallation sequence that purges local binaries and unregisters components.
* **Color-Coded CLI Theme:** 
  * Distinct coloring for execution flags, software versions, available updates, package IDs, and sources.
  * Clean dark cyan borders and yellow operational completion notifications.
* **Lightweight Utility:** Built natively in C# utilizing standard process redirection with isolated background window architecture.

---

## How It Works

NullPatch acts as an intelligent bridge between your inputs and the native `winget` binary:
1. **Token Processing:** When you pass commands, the utility splits the arguments, evaluating keywords (like `winget`, `upgrade`, or flags like `--`) to assign precise console colors (`ConsoleColor.Gray`, `Cyan`, `White`, etc.).
2. **Process Execution:** It spins up a background `ProcessStartInfo` pointing to `winget` with `UTF-8` output encoding enabled, safely capturing asynchronous data and error lines.
3. **Table Reconstruction:** When a data table is detected (such as list or upgrade commands), NullPatch identifies column boundaries (`Id`, `Version`, `Available`, `Source`) and redraws the output row-by-row with custom formatting before closing with clean structural separators.
---

## Getting Started

### Prerequisites
* Windows 10 / 11
* [.NET SDK](https://dotnet.microsoft.com/) installed on your system (only required if building from source)
* Native `winget` client (App Installer) available via the Microsoft Store or Windows.

### Installation & Usage

#### Option A: Download Pre-compiled Binary (Recommended)
1. Head over to the [NullPatch Releases](https://github.com/reegzl/NullPatch/releases/latest) page.
2. Download the latest `NullPatch.exe` asset.
3. Run the executable directly on your system.

#### Option B: Build From Source
1. Clone the repository:
```bash
   git clone [https://github.com/reegzl/NullPatch.git](https://github.com/reegzl/NullPatch.git)
```
2. Navigate into the project directory and build a standalone release executable via the .NET CLI:
```bash
   cd NullPatch/NullPatch
```
```bash
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
 Your compiled .exe will be generated inside the bin/Release/net10.0/win-x64/publish/ folder.

---

## Support the Developer

If NullPatch has made managing your system updates easier or saved you some time, consider supporting its development with crypto:

* **Bitcoin (BTC):** `bc1qm427zm2jxmesulwjd4j95k82ck9h7l9n7wqemt`
* **Ethereum (ETH):** `0xf6bf5446Efe20f1404016895c6deaf0F22EF76CE`
* **Stellar (XLM):** `GBDLBCAE75FO3QNB5VWCWMEOIV2GEP7UFPP3CQICPM3KOZ2YVY55E7OJ`


Thanks for checking out the tool!
