# About
A template layout for new .NET Solutions using Visual Studio 2022 Community or greater. Includes only commonly used folders and configuration files.

This template was inspired by David Fowler's [gist](https://gist.github.com/davidfowl/ed7564297c61fe9ab814) and can be adjusted as needed.

# How to Use
- Click on the 'Use this template' button.
- Provide your new repository information and settings.
- Update the existing `LICENSE` with your own.
- Delete folders and files you may not need.
- Rename the `Layout.sln` as needed.
- Update this readme file.

## Extras
Starting from .NET 5, it is possible to create a `.cs` file in your projects that will hold global `using` statements. E.g.:

File: `Usings.cs`
```csharp
global using System.Buffers.Binary;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Text;
```
