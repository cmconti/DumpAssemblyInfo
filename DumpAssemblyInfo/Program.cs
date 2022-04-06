using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DumpAssemblyInfo
{
    class Program
	{
		static void Usage()
		{
			Console.WriteLine("Usage: DumpAssemblyInfo <path>");
		}

		static void Main(string[] args)
		{
			if (args.Length == 0 || !File.Exists(args[0]))
			{
				if (!File.Exists(args[0]))
				{
					Console.WriteLine();
					Console.WriteLine($"File not found: '{args[0]}'");
					Console.WriteLine();
				}

				Usage();
				Console.WriteLine();
				return;
			}
			var path = Path.GetFullPath(args[0]);

			var assembly = Assembly.ReflectionOnlyLoadFrom(path);
			var cecilAssembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(path);

			//name
			Console.WriteLine();
			Console.WriteLine($"Full Name:\t{assembly}");

			//runtime
			Console.WriteLine($"Runtime version:\t{assembly.ImageRuntimeVersion}");

			//attributes
			Console.WriteLine("Attributes:");
			try
			{
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

					Console.WriteLine($"\t[{attrType}({ctrDisplayArgs})]");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\tUnavailable (exception:{ex.Message})");
			}

			//references
			Console.WriteLine("References:");
			try
			{
				foreach (var reference in assembly.GetReferencedAssemblies())
				{
					Console.WriteLine($"\t{reference.FullName}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\tUnavailable (exception:{ex.Message})");
			}

			//version
			var versionInfo = FileVersionInfo.GetVersionInfo(path);
			string ver = versionInfo.ToString().Replace("\n", "\n\t");
			Console.WriteLine("Win32 Version Info:");
			Console.WriteLine($"\t{ver}");
		}
	}
}
