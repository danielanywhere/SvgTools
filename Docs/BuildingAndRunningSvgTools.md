# Building and Running SVG Tools

SvgTools is a managed command-line application running on the .NET Core framework on Linux, Mac, and Windows.

## Table of Contents

-   [Prerequisites](#prerequisites).

    -   [Install on Ubuntu](#install-on-ubuntu).
    -   [Install on macOS](#install-on-macos).
    -   [Install on Windows](#install-on-windows).
    -   [Verify your .NET SDK](#verify-your-net-sdk).

-   [Basic Run Only Mode](#basic-run-only-mode).

-   [Publishing and Running the Compiled Version](#publishing-and-running-the-compiled-version).

<p>&nbsp;</p>

## Prerequisites

The only major consideration that needs to be made is that you have the .NET Code SDK, version 8 or later installed on your system. Multiple methods are available for achieving this.

-   Install the .NET Core SDK directly. Available on Linux, Mac, and Windows.
-   Install VS Code and the accompanying C# Dev Kit extension, then run the VS Code Walkthrough, selecting the .NET Core version 10 or later SDK when prompted. Available on Linux, Mac, and Windows. This will not be covered.
-   Install Visual Studio, Community or Professional editions, version 2022 or later, then select .NET Core 10 or later as an active SDK. Available on Windows-only. This will not be covered.

<p>&nbsp;</p>

### Install .NET SDK on Ubuntu

Use the following command to install .NET 10 SDK on Ubuntu.

```batch
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0


```

<p>&nbsp;</p>

### Install .NET SDK on macOS

There are three methods you can use for installing the .NET SDK on Macintosh.

<p>&nbsp;</p>

#### Method 1. The Graphical Installer

-   Navigate to the official Microsoft .NET 10 Download Page at https://dotnet.microsoft.com/en-us/download/dotnet/10.0.

-   Under the **SDK** section, locate the **macOS** row.

-   Choose the appropriate **Installers** package based on your Mac's processor:

    -   **Arm64**. Click this if you have an Apple Silicon Mac (M1, M2, M3, M4 chips).
    -   **x64**. Click this if you have an older Intel-based Mac.

-   Open the downloaded .pkg file from your Downloads folder.

-   Follow the on-screen installation steps and provide your administrator password when prompted.

<p>&nbsp;</p>

#### Method 2. MacPorts Package Manager

If you use the MacPorts package manager, open your Terminal and issue the following command.

```batch
sudo port install dotnet-sdk-10


```

<p>&nbsp;</p>

#### Method 3. Direct Installation

You can install the SDK directly through the terminal without elevated administrative privileges using Microsoft's installation script by following these steps.

-   Open your Terminal and download the Script.
-   Make the script executable.
-   Run the script targeting the .NET 10 channel.

The following commands carry out that process.

```batch
curl -O https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0

```

<p>&nbsp;</p>

### Install .NET SDK on Windows

If you have Windows 10, Windows Server 2012, or newer, you can install .NET 10 SDK by following these steps.

-   Navigate to the official .NET 10 Download Page at https://dotnet.microsoft.com/en-us/download/dotnet/10.0.
-   In the **SDK** section, select the appropriate **Windows** link from the **Installers** column, matching your system's architecture (usually x64 for standard 64-bit Intel/AMD processors).
-   Open your **Downloads** folder and double-click the downloaded .exe installer file.
-   Click the **Install** button in the setup wizard then approve the administrative (UAC) prompt.
-   Close the installer window once the setup success screen appears.

<p>&nbsp;</p>

### Verify your .NET SDK

To verify that your .NET SDK is up-to-date, use the following commands.

If you have just finished installing the SDK, close your current Terminal session and open a new one.

To get a basic indication that the .NET Core SDK is installed, run the following command.

```batch
dotnet --version

```

If the .NET SDK is installed, a version number will be output.

To get more detailed information about the platform, use the following command.

```batch
dotnet --info

```

<p>&nbsp;</p>

## Basic Run Only Mode

If you only want to immediately download and run the application for testing or occasional use, you can use these steps on Linux, Mac, or Windows.

-   Open your Terminal application.
-   Change the directory to something where new temporary files won't clutter your file system, as in the following example for Ubuntu or Windows, for example.

```batch
cd Downloads

```

-   Download the source code from GitHub then run the application using the dotnet run command as shown in the following command-lines.

```batch
git clone https://github.com/danielanywhere/SvgTools
cd SvgTools/Source/SvgTools
dotnet run -- /? /wait

```

The above example is the same as running the fully compiled version with the parameters of **/?** and **/wait**., where **/?** displays the application syntax and **/wait** waits for the user to press \[Enter\] before exiting the application.

The first time you run the application, there will be some delay because it will be compiled from scratch using the source files. Each additional time you run it without changing the source files, however, it will start significantly faster than the first time.

<p>&nbsp;</p>

## Publishing and Running the Compiled Version

If you intend to use SvgTools multiple times, you can compile it for use in release mode and run the compiled file directly using the dotnet command, as shown in the example commands below.

Note that the following cd Downloads command will only have an effect in certain cases, like Ubuntu and Windows, for example.

<p>&nbsp;</p>

```batch
sudo apt-get update
sudo apt-get install -y clang zlib1g-dev
cd Downloads
git clone https://github.com/danielanywhere/SvgTools
cd SvgTools/Source/SvgTools
dotnet publish ./SvgTools.csproj -r linux-x64 -c Release
cd bin/Release/net8.0/linux-x64/native
./SvgTools



```

<p>&nbsp;</p>

In the above example for Ubuntu, the following actions are used.

-   Update the package installer, if necessary.
-   Install the necessary compilation tools, if not already present.
-   Change to a directory where clutter can be managed.
-   Clone the source code from GitHub.
-   Change directories to the one containing the SvgTools.csproj file.
-   Execute the publish command, which creates a natively executable version of SvgTools for Linux.
-   Change to the directory where the compiled native application has been output. In this case, **~/Downloads/SvgTools/Source/SvgTools/bin/Release/net8.0/linux-x64/native**
-   Test-run the application, using the current relative directory prefix. **./SvgTools**

<p>&nbsp;</p>

Note that the native application file can now either be moved to another directory, or the current directory can be added to your global $PATH environment variable.
