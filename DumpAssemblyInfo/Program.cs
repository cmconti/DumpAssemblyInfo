using System;
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

			var assembly = Assembly.LoadFrom(path);

			//name
			Console.WriteLine();
			Console.WriteLine($"Full Name:\t{assembly}");

			//runtime
			Console.WriteLine($"Runtime version:\t{assembly.ImageRuntimeVersion}");

			//attributes
			Console.WriteLine("Attributes:");
			foreach (var attr in assembly.CustomAttributes)
			{
				Console.WriteLine($"\t{attr}");
			}

			//references
			Console.WriteLine("References:");
			foreach (var reference in assembly.GetReferencedAssemblies())
			{
				Console.WriteLine($"\t{reference.FullName}");
			}

			//version
			var versionInfo = FileVersionInfo.GetVersionInfo(path);
			string ver = versionInfo.ToString().Replace("\n", "\n\t");
			Console.WriteLine("Win32 Version Info:");
			Console.WriteLine($"\t{ver}");
		}
	}
}
