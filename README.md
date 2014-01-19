Windows Tweaker has been written in C# and WPF using .net Framework 4.5.1, to allow users to easily change every possible setting in Windows without having to dig deep into registry.

Window Tweaker 4.0 was released on March-4, 2012.
The latest version 5.0 is currently under development, and is a complete re-write from scratch.

## To Build
Head to this [Lifehacker article](http://lifehacker.com/5983680/how-the-heck-do-i-use-github "Lifehacker") if you don't know how to setup git on Windows.

1. Download [Visual Studio Express 2013 for Windows Desktop](http://wpftoolkit.codeplex.com/), in case you don't have a paid license for Visual Studio 2013. 

2. Clone the repository

    ```git clone git@github.com:siddharth96/windows-tweaker.git```

3. Add references to these three DLLs
    - [Interop.IWshRuntimeLibrary.dll](https://sites.google.com/site/suresoftwares/downloads/Interop.IWshRuntimeLibrary.dll?attredirects=0&d=1 "Interop.IWshRuntimeLibrary.dll")
    - [WPF Folder Browser](https://sites.google.com/site/suresoftwares/downloads/WPFFolderBrowser.dll?attredirects=0&d=1 "WPF Folder Browser")
    - [Extended WPF Toolkit Community Edition](http://wpftoolkit.codeplex.com/)

Change the branch to ```v4.0```, if you want to build the previous stable release.