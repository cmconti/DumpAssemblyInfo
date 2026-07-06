# Assessment: SDK-style Conversion

## Projects to Convert

| Project | Path | packages.config | Custom Imports | Special Type | Risk |
|---------|------|----------------|----------------|-------------|------|
| DumpAssemblyInfo | DumpAssemblyInfo\DumpAssemblyInfo.csproj | Yes (3 packages) | ILRepack.Lib.MSBuild.Task.targets | Console app | Medium |

### Details for DumpAssemblyInfo

**Current state:**
- Target framework: .NET Framework 4.8
- Output type: Console application (Exe)
- Uses packages.config with 3 packages:
  - CommandLineParser 2.9.1
  - ILRepack.Lib.MSBuild.Task 2.0.18.2
  - Mono.Cecil 0.11.5
- Has 2 explicit compile includes:
  - Program.cs
  - Properties\AssemblyInfo.cs
- Custom import: ILRepack.Lib.MSBuild.Task.targets (used for merging assemblies)

**Complexity factors:**
- **ILRepack integration**: The project uses ILRepack.Lib.MSBuild.Task which imports custom targets. This needs to be preserved during conversion.
- **AssemblyInfo.cs**: Properties\AssemblyInfo.cs exists and may conflict with SDK-style auto-generation of assembly attributes
- **packages.config**: Must be migrated to PackageReference format

## Already SDK-style
None

## Baseline
- Solution builds: **Yes** (successful clean build)
- Warning count: 0
- Build time: ~0.8s

## Key Findings

1. **Simple project structure**: Only 2 source files makes conversion straightforward
2. **ILRepack dependency**: The custom MSBuild target for ILRepack needs special attention. In SDK-style projects, the package should automatically work, but we need to verify the import is preserved
3. **No special project types**: Standard console application - no WPF/WinForms/ASP.NET complications
4. **Clean baseline**: Zero warnings in current state - we should maintain this after conversion

## Conversion Strategy
1. Convert to SDK-style format using the convert_project_to_sdk_style tool
2. Migrate packages.config to PackageReference
3. Handle AssemblyInfo.cs (either remove or disable auto-generation)
4. Verify ILRepack.Lib.MSBuild.Task still functions correctly
5. Verify build succeeds with zero warnings
6. Maintain target framework at net48 (no framework upgrade)
