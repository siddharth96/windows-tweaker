Windows Tweaker 5.0 has been written in C# and WPF using .net Framework 4.5.2, to allow users to easily change every possible setting in Windows without having to dig deep into registry.

Window Tweaker 4.0 (used .net 4.0) was released on March-4, 2012.
The latest version 5.0 is currently under development, and is a complete re-write from scratch.

## To Build
Head to this [Lifehacker article](http://lifehacker.com/5983680/how-the-heck-do-i-use-github "Lifehacker") if you don't know how to setup git on Windows.

1. Download [Visual Studio Express 2013 for Windows Desktop](http://msdn.microsoft.com/en-us/dn369242), in case you don't have a paid license for Visual Studio 2013. 

2. Download .net Framework 4.5.2 Developer Pack from [here](http://www.microsoft.com/en-us/download/details.aspx?id=42637)

2. Clone the repository

    ```git clone git@github.com:siddharth96/windows-tweaker.git```

3. Add references to all the DLLs located in ```WindowsTweaker/Dependencies``` directory. Also, install [Microsoft Blend SDK](http://www.microsoft.com/en-us/download/details.aspx?id=10801).

4. Create a new class called ```Keys.Local.cs```, and paste in the following code:-
```csharp
namespace WindowsTweaker {
    internal static partial class Keys { }
}
```

### Links to external libraries used
1. [WPF ToolKit](https://wpftoolkit.codeplex.com/)
2. [WPF Spark](https://wpfspark.codeplex.com/)

### Stemmers used
1. Porter Stemmer 2 algorithm for stemming English words ([C# Implementation link](http://alski.net/post/2007/09/16/0a-Porter-Stemmer-2-C-implementation0a-0a-.aspx), [Python Implementation Link](https://pypi.python.org/pypi/stemming/1.0))
2. Snowball Stemmer algorithm for stemming German and Russian words ([C# Implementation Link](http://www.iveonik.com/blog/2011/08/snowball-stemmers-on-csharp-free-download/), [Python Implementation Link](https://pypi.python.org/pypi/PyStemmer/1.0.1))

Change the branch to ```v4.0```, if you want to build the previous stable release.