# SourceMapAnalyzer

This is a tool for porting a map from a Source mod to Garry's Mod.

- Base FGDs - these are where the base FGDs for the mod you're porting from go (for example, for a HL2MP mod this would be hl2mp.fgd in hl2/bin in the Half Life 2 directory).
- VPKs - these are the VPKs from all the games which your mod references (HL2, EP2, cstrike, whatever)
- Maps - these are the maps from the mod that you want to be analyzed
- Process - Game FGD is the FGD that defines the mod you're porting from. Game Dir is the root directory for the game (the one with models, maps, etc). Check "package" if you want the content these maps depend on to be copied to the tool's directory when analyzing.

This is licensed under an MIT license.