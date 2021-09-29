# CCompiler
Compiler for the C written on C#

# Building compiler in VS2019
The compiler is developed on the .NET Core 3.1 platform.

To build it in Windows, I use Microsoft Visual Studio 2019.

After downloading the source, open CCompiler.sln in Visual Studio.

# Building compiler in console
```
git clone https://github.com/ALMikhai/CCompiler
cd CCompiler/CCompiler/
dotnet build --configuration Release
```

# Compiler using
Execution file will be place in the same directory as the code file
```
.\CCompiler.exe "Add.c" -net
dotnet Add.exe
```

# Implemented
* Lexical analyzer
* Parser for simple math expressions
* Parser (AST)
* Semantic analyzer
* Compilation to .net format (Mono.Cecil)

# Author
Alexander Mikhailenko - third year student, Far Eastern Federal University
