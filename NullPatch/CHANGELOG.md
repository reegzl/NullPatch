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
  <b>An advanced, colored CLI utility to streamline Windows Package Manager (winget) updates and package pins. Created by REEGZL.</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows-blue?style=flat-square" alt="Platform Windows">
  <img src="https://img.shields.io/badge/Language-C%23-purple?style=flat-square" alt="Language C#">
  <img src="https://img.shields.io/badge/Version-v1.11-orange?style=flat-square" alt="Version v1.10">
  <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="License MIT">
</p>

---

## What's New in v1.11

* **Documentation & Installation Refinement:** Fixed and completed the Getting Started guide in the README, adding precise instructions for downloading pre-compiled binaries directly from the releases page alongside streamlined command-line build instructions.

* **Executable Naming Convention:** Cleaned up published binary filenames from dot-separated formats (NullPatch.v1.10.exe) to a simple, standard NullPatch.exe for a cleaner look and easier command usage.

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
