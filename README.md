## Automatic Volume Mixer
Automatic Volume Mixer is a tool that allows automatization of Windows Volume Mixer based on user's rules. You can open the Volume Mixer by right-clicking on the speaker icon in the system tray and selecting Open Volume Mixer. This application is an automatic version of that applet.

## Common usage examples
* Pausing your audio player (e.g. foobar2000) whenever any other application makes a noise,
* and resuming playback once the noise is gone. This enables you to keep your audio player running in the background at all times.
* Briefly muting all applications while a notification is playing.
* Forcing application's volume to a set level
* Automatically lowering volume during night
* Launching processes/popups when some application makes noise

## Screenshots
![Main window](/Documentation/1.PNG?raw=true "Main window")
![Event editor](/Documentation/2.PNG?raw=true "Event editor")
![Example action setup](/Documentation/3.PNG?raw=true "Example action setup")

## Compiling
Any modern version of Visual Studio should work.

CSCore newer than v1.1 is required (old versions will crash).

You might need to download [this library](https://sourceforge.net/p/kloctoolslibrary/) separately.
