# SDK-style Conversion Plan

## Overview
Convert DumpAssemblyInfo project from legacy .NET Framework project format to SDK-style format while maintaining .NET Framework 4.8 target and all functionality.

## Scope
- **1 project** to convert: DumpAssemblyInfo
- **Target framework**: net48 (no change)
- **Key challenges**: ILRepack custom targets, AssemblyInfo.cs handling, packages.config migration

## Approach
The conversion will use the `convert_project_to_sdk_style` tool which handles:
- Converting project XML to SDK-style format
- Migrating packages.config to PackageReference
- Handling file globbing (removing explicit Compile includes)
- Preserving custom imports and build logic
- Managing AssemblyInfo.cs conflicts

## Tasks

### 01-convert-project
**Description**: Convert DumpAssemblyInfo.csproj to SDK-style format

**Actions**:
- Unload the project in Visual Studio to release file lock
- Run `convert_project_to_sdk_style` tool
- Verify the converted project structure:
  - Sdk attribute present on Project element
  - TargetFramework = net48 (unchanged)
  - PackageReferences instead of packages.config
  - ILRepack.Lib.MSBuild.Task package preserved
  - No explicit file includes (uses SDK globbing)
- Handle AssemblyInfo.cs:
  - Check for duplicate assembly attributes
  - Either remove Properties\AssemblyInfo.cs or add `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>`
- Reload the project in Visual Studio

**Validation**:
- Project file is SDK-style format
- Solution builds successfully
- Zero warnings (maintain baseline)
- packages.config file is removed
- All 3 NuGet packages are present as PackageReferences
- Output exe is created successfully

**Risk**: Medium - ILRepack custom target behavior needs verification

**Done when**:
- ✅ Project uses SDK-style format with `Sdk` attribute
- ✅ TargetFramework = net48 (unchanged)
- ✅ No packages.config file exists
- ✅ All packages migrated to PackageReference
- ✅ Solution builds with zero errors and zero warnings
- ✅ Project is reloaded in Visual Studio

### 02-verify-functionality
**Description**: Verify the converted project maintains all original functionality

**Actions**:
- Build the solution in Release configuration
- Check that exe output is produced
- Verify ILRepack is still functioning (if it merges assemblies, check the output)
- Compare output folder contents with pre-conversion baseline
- Run the application with sample arguments to verify it works

**Validation**:
- Release build succeeds
- Application runs without errors
- Output matches expected behavior

**Done when**:
- ✅ Release build succeeds with zero warnings
- ✅ Application executable runs successfully
- ✅ All functionality preserved from original project

## Success Criteria
- [x] DumpAssemblyInfo.csproj is SDK-style format
- [x] TargetFramework remains net48
- [x] All packages migrated from packages.config to PackageReference
- [x] Solution builds with zero errors and zero warnings in both Debug and Release
- [x] ILRepack integration still works
- [x] No functionality regressions
