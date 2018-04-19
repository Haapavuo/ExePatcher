# HexPatcher
A .NET command line tool to modify any binary files by their hex based address.

Targets .NET Core 1.1 Framework.

Create a configuration file that contains three rows:
- Binary file path
- Original bytes to find (can contain '\*' wildcard bytes)
- Modified bytes to replace the original bytes with
