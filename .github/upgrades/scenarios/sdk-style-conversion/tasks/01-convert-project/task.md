# 01-convert-project: Convert Project

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
