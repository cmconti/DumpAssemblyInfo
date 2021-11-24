using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
				Usage();
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

			//version
			var versionInfo = FileVersionInfo.GetVersionInfo(path);
			string ver = versionInfo.ToString().Replace("\n", "\n\t");
			Console.WriteLine("Version Info:");
			Console.WriteLine($"\t{ver}");
		}
	}
}
