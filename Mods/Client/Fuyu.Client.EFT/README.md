# Fuyu.Client.EFT

A simple fuyu mod that only disables and modifies EFT code that prevent
Fuyu.Backend from functioning without those changes. Can run on obfuscated
`Assembly-CSharp.dll`.

## Modifications

### BattlEyePatch

Since BattlEye would prevent our assembly from loading, we disable it here.

### ConsistencyGeneralPatch

Technically not required if you don't modify `Assembly-CSharp.dll` or don't
delete unused files (like `EscapeFromTarkov-BE.exe`) but I do this often enough
that it warrants inclusion.

Disables file integrity checking for general files (excludes unity bundles).

### ProtocolUtil

The game splits the scheme from the url and reconstructs the url later, but it
doesn't seem to do much. I just made it blank. Might cause issues in the
future and needs a proper replacement.