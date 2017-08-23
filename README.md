## Automatic Volume Mixer
![Main window](/Documentation/1.PNG?raw=true "Main window")

Automatic Volume Mixer is a tool that allows automatization of Windows Volume Mixer based on user's rules. You can open the Volume Mixer by right-clicking on the speaker icon in the system tray and selecting Open Volume Mixer. This application is an automatic version of that applet.

## Common usage examples
* Pausing your audio player (e.g. foobar2000) whenever any other application makes a noise,
* and resuming playback once the noise is gone. This enables you to keep your audio player running in the background at all times.
* Briefly muting all applications while a notification is playing.
* Forcing application's volume to a set level
* Automatically lowering volume during night
* Launching processes/popups when some application makes noise

## Download
Check the [Releases](https://github.com/Klocman/Automatic-Volume-Mixer/releases) for the latest version.

## Getting started
After launching AVM it will appear in the system tray. Double-click it to open the configuration window.

By default there are no behaviours set. You can check an example of how to set up AVM with foobar2000 by clicking on "Import" and selecting "Foobar Example.xml" in the program directory.

To enable the "start on boot" setting you might need to run AVM as an administrator.

## Screenshots
![Event editor](/Documentation/2.PNG?raw=true "Event editor")
![Example action setup](/Documentation/3.PNG?raw=true "Example action setup")

## Compiling
Any modern version of Visual Studio should work.

CSCore newer than v1.1 is required (old versions will crash).

You might need to download [this library](https://sourceforge.net/p/kloctoolslibrary/) separately.
