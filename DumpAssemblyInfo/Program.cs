﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using CommandLine;
using CommandLine.Text;

using Mono.Cecil;

namespace DumpAssemblyInfo
{
	class Options
	{
		[Value(index: 0, MetaName = "<path>", Required = true, HelpText = "Assembly file path.")]
		public string Path { get; set; }

		[Option(shortName: 's', longName: "Sort", Required = false, Default = false, HelpText = "Sort assembly attributes and references alphabetically.")]
		public bool Sort { get; set; }
	}

	class Program
	{
		static ParserResult<Options> parserResult;

		static void Usage()
		{
			Console.WriteLine(HelpText.AutoBuild<Options>(parserResult, h => h, e => e));
			Console.WriteLine();
		}

		static int Main(string[] args)
		{
			parserResult = Parser.Default.ParseArguments<Options>(args);

			return parserResult.MapResult(
							(Options opts) => DumpInfo(opts.Path, opts.Sort),
							errs => 1
						);
		}

		static int DumpInfo(string path, bool sort)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			path = Path.GetFullPath(path);

			if (!File.Exists(path))
			{
				Console.WriteLine();
				Console.WriteLine($"File not found: '{path}'");
				Console.WriteLine();
				Usage();
				return 1;
			}

			Assembly assembly = null;

			try
			{
				assembly = Assembly.ReflectionOnlyLoadFrom(path);
			}
			catch (BadImageFormatException ex)
			{
				if (ex.Message.Contains("expected to contain an assembly manifest"))
				{
					Console.WriteLine($"'{path}' is not an assembly");
					return 1;
				}
				throw;
			}

			var cecilAssembly = AssemblyDefinition.ReadAssembly(path);

			//name
			Console.WriteLine();
			Console.WriteLine($"Full Name:\t{assembly}");

			//runtime
			Console.WriteLine($"Runtime version:\t{assembly.ImageRuntimeVersion}");

			//attributes
			Console.WriteLine("Attributes:");
			try
			{
				var attrDisplayList = new List<string>();

				//can't use assembly.CustomAttributes for a ReflectionOnlyLoadFrom loaded assembly
				//var attrs = assembly.CustomAttributes;
				var attrs = cecilAssembly.CustomAttributes;
				foreach (var attr in attrs)
				{
					var attrType = attr.Constructor.DeclaringType.FullName;

					var ctrArgList = new List<string>();
					foreach (var arg in attr.ConstructorArguments)
					{
						string val = arg.Value.ToString();
						if (arg.Type.FullName.Contains("System.String"))
						{
							val = $"\"{val}\"";
						}
						ctrArgList.Add(val);
					}

					var attrArgs = string.Join(", ", ctrArgList);

					var propList = new List<string>();
					foreach (var prop in attr.Properties)
					{
						string val = prop.Argument.Value.ToString();
						if (prop.Argument.Type.FullName.Contains("System.String"))
						{
							val = $"\"{val}\"";
						}
						propList.Add($"{prop.Name} = {val}");
					}
					var attrParams = string.Join(", ", propList);

					var argDisplayList = new List<string>();
					if (ctrArgList.Count > 0)
					{
						argDisplayList.Add(attrArgs);
					}
					if (propList.Count > 0)
					{
						argDisplayList.Add(attrParams);
					}

					var ctrDisplayArgs = string.Join(", ", argDisplayList);

					attrDisplayList.Add($"\t[{attrType}({ctrDisplayArgs})]");
				}

				if (sort)
					attrDisplayList.Sort();

				attrDisplayList.ForEach(attr => Console.WriteLine(attr));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\tUnavailable (exception:{ex.Message})");
			}

			//references
			Console.WriteLine("References:");
			try
			{
				var refDisplayList = new List<string>();

				foreach (var reference in assembly.GetReferencedAssemblies())
				{
					refDisplayList.Add($"\t{reference.FullName}");
				}

				if (sort)
					refDisplayList.Sort();

				refDisplayList.ForEach(attr => Console.WriteLine(attr));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\tUnavailable (exception:{ex.Message})");
			}

			//version
			var versionInfo = FileVersionInfo.GetVersionInfo(path);
			string ver = versionInfo.ToString().Trim().Replace("\n", "\n\t");
			Console.WriteLine("Win32 Version Info:");
			Console.WriteLine($"\t{ver}");

			//module info
			//based on https://github.com/sushihangover/CorFlags/blob/master/CorFlags/Program.cs
			Console.WriteLine("Module Info:");
			try
			{
				var module = ModuleDefinition.ReadModule(path);

				var pe = (module.Architecture == TargetArchitecture.AMD64 || module.Architecture == TargetArchitecture.IA64) ? "PE32+" : "PE32";
				//	anycpu: PE = PE32  and  32BIT = 0
				//	   x86: PE = PE32  and  32BIT = 1
				//	64-bit: PE = PE32+ and  32BIT = 0
				string platform = string.Empty;
				switch (pe)
				{
					case "PE32":
						platform = (module.Attributes.HasFlag(ModuleAttributes.Required32Bit)) ? "x86" : "AnyCPU";
						break;
					case "PE32+":
						platform = "x64";
						break;
				}

				Console.WriteLine($"\tAssembly Version : {module.Assembly.Name.Version}");
				Console.WriteLine($"\tRuntime Version  : {module.RuntimeVersion}");
				Console.WriteLine($"\tCLR Header: {module.Runtime}");
				Console.WriteLine($"\tPE(raw)   : {module.Architecture}");
				Console.WriteLine($"\tPE        : {pe}");
				Console.WriteLine($"\tCorFlags  : 0x{(int)module.Attributes:X}");
				Console.WriteLine($"\tILONLY    : {module.Attributes.HasFlag(ModuleAttributes.ILOnly)}");
				Console.WriteLine($"\t32BITREQ  : {module.Attributes.HasFlag(ModuleAttributes.Required32Bit)}");
				Console.WriteLine($"\t32BITPREF : {module.Attributes.HasFlag(ModuleAttributes.Preferred32Bit)}");
				Console.WriteLine($"\tSigned    : {module.Attributes.HasFlag(ModuleAttributes.StrongNameSigned)}");
				Console.WriteLine($"\tPlatform  : {platform}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\tUnavailable (exception:{ex.Message})");
			}
			Console.WriteLine();

			return 0;
		}
	}
}
