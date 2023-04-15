# SourceMapAnalyzer

This is a tool for porting a map from a Source mod to Garry's Mod.

- Base FGDs - these are where the base FGDs for the mod you're porting from go (for example, for a HL2MP mod this would be hl2mp.fgd in hl2/bin in the Half Life 2 directory).
- VPKs - these are the VPKs from all the games which your mod references (HL2, EP2, cstrike, whatever)
- Maps - these are the maps from the mod that you want to be analyzed
- Process - Game FGD is the FGD that defines the mod you're porting from. Game Dir is the root directory for the game (the one with models, maps, etc).

There are four packaging options:

- None - no content will be copied, only text files will be generated
- Folder Per Map - one folder will be created for each map containing each map's content
- Combined Folder - all content for all maps will be output into the same folder
- Combined Addon - all content for all maps will be output into the same folder, and it will be setup as a Garry's Mod addon with an addon.json and a lua file generated that will add the appropriate content using `resource.AddFile` when the given map is loaded.

You can also check "VPKs to pack content from" to pack content from any of the dependent VPKs into the output. For example, check "ep2" to copy HL2:EP2 content into the output so people using the map don't need Episode 2 installed.

This is licensed under an MIT license.