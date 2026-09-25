# Fracaz v1.0.0

## Overview
This is a C#-port of the original Fracas game which was written in VB6. I tried to keep to the original code as much as possible, including comments, structures and naming. A form of the original code is available on GitHub at https://github.com/Planet-Source-Code/eric-o-sullivan-fracas-strategy-game__1-69474. 

I did make some changes to the code when they were necessary to make it work in C#. I also added some features and fixed some bugs that I found along the way. I will document those changes later more in detail perhaps.

My initial reason for porting this game was so I could have a game with which I could improve my programming and software engineering skills, as well as learning more about GitHub, open source development and things like design patterns.

I'm sure I missed some things in the original game, so if you find any bugs or have any suggestions for improvements, please let me know.

## Running the solutions
Requirements:
- Windows (10+?) - Because of WinForms, you *need* Windows.
- .NET 10 SDK
- Visual Studio 2022 (or later) or .NET CLI

Just open the solution in Visual Studio and run the project. You can also use the .NET CLI to run the project by navigating to the project directory and running `dotnet run`.

## Changes versus the original
I tried to port to code as-is, but I had to make some changes to make it work in C#. I did not fix any bugs I encountered, nor did I introduce any code improvements. The changes I had to make are:
- I created a new GDIOperations class that handles the BitBlt and SetPixel stuff that is not as straightforward in C# as it is in VB6.
- In VB, the main Form class was accessed from everywhere in the code, but in C# the form is an instance. I had to create a static .Refs class that holds certain references.
- Instead of using just WinSock, I had to create a TcpClient and a TcpListener.
- The way some files are read from and written to use different APIs in C# and so the code is completely different.
- And some more minor changes.

## How to play
It's a turn based strategy game. First you select a HQ, then you select countries to annex or attack. You can also build ports to be able to reach countries across water. The combined attack strength of the adjacent countries determines the attack strength of your country, the same goes for the defense strength. It's all pretty straight forward, :-). I'll probably add a more extensive manual at some point. Just click around and see what happens. 

## Roadmap
My intention is to first update the architecture of this game, while keeping the gameplay and features intact as they are in the original game. After that I will start adding new features and improving the gameplay. All initial architectural changes will be part of the 1.X.Y releases, while new features and gameplay improvements will be part of the 2.X.Y releases. If I find some bugs along the way, I will fix them and make them part of the 1.X.Y releases as well.

I'm not sure yet what those architectural changes will be, but it'll be something like the following:
- Separate the game logic from the UI and graphics.
- Use more appropriate C# features and structures, such as classes, interfaces, enums, etc.
- Make the player logic more abstract, so it can be easily extended to support different types of players, such as AI players or network players.
- Et cetera?

## AI
I started this project a while ago and did not use any AI agents during development. I did use AI to automatically generate commit message in my private repository and inline as "code completion" tool. The latter use case was really helpful, at some moment I just had to paste the original method, tab the generated C# into completion and then check the results.

## Credits
- Original game: Fracas by Smozzie. I tried to find the original author online, but I couldn't find any contact information. Given that is HAS been released as open source and is available on GitHub with his permission (see https://github.com/Planet-Source-Code/eric-o-sullivan-fracas-strategy-game__1-69474), and that this game is now about 20, 25 years old, I think it's safe to assume it's OK to release this port as open source as well. If you are the original author and have any concerns about this, please let me know.

## License
Just your regular open source license, all standard disclaimers apply, so I guess that would mean the MIT License, see LICENSE.md for details.