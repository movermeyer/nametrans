// Author: Martin Matusiak <numerodix@gmail.com>
// Licensed under the GNU Public License, version 3.

using System;
using System.Reflection;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

public class App {
	static string py_path = "..";
	static string pyscript = "gnametrans.py";
	static string exe_path = GetPathToExecutable();
	static string dll_path = "dll";

	static void Main(string[] args) {
		// set path for dynamic assembly loading
		AppDomain.CurrentDomain.AssemblyResolve +=
			new ResolveEventHandler(ResolveAssembly);


		string path = System.IO.Path.Combine(exe_path, py_path);
		pyscript = System.IO.Path.Combine(path, pyscript);
		pyscript = System.IO.Path.GetFullPath(pyscript); // normalize

		ScriptRuntimeSetup scriptRuntimeSetup = new ScriptRuntimeSetup();

		LanguageSetup language = Python.CreateLanguageSetup(null);
		language.Options["FullFrames"] = true;
		scriptRuntimeSetup.LanguageSetups.Add(language);

		ScriptRuntime runtime = new Microsoft.Scripting.Hosting.ScriptRuntime(scriptRuntimeSetup);
		ScriptScope scope = runtime.CreateScope();
		scope.SetVariable("__SYS_ARGV", GetArgv(pyscript, args));
		ScriptEngine engine = runtime.GetEngine("python");

		// add sys.path inside python code instead, makes it runnable also
		// with ipy.exe
/*
		var paths = engine.GetSearchPaths();
		paths.Add(path);
		paths.Add(System.IO.Path.Combine(path, "pylib"));
		engine.SetSearchPaths(paths);
*/
		ScriptSource source = engine.CreateScriptSourceFromFile(pyscript);
		source.Compile();

		try {
			source.Execute(scope);
		} catch (IronPython.Runtime.Exceptions.SystemExitException e) {
			Console.WriteLine(e.StackTrace);
		}
	}

	static string GetPathToExecutable() {
		return System.IO.Path.GetDirectoryName(
				Assembly.GetExecutingAssembly().Location);
	}

	static string[] GetArgv(string pyscript, string[] args) {
		string[] nargs = new string[args.Length + 1];
		nargs[0] = pyscript;
		for (int i=0; i<args.Length; i++) {
			nargs[i+1] = args[i];
		}
		return nargs;
	}

	static Assembly ResolveAssembly(object sender, ResolveEventArgs args) {
		string assembly_name = args.Name.Split(',')[0] + ".dll";

		Console.WriteLine("Load assembly dynamically: {0}", assembly_name);

		string path = System.IO.Path.Combine(GetPathToExecutable(), dll_path);
		string assembly_fp = System.IO.Path.Combine(path, assembly_name);
		Assembly assembly = Assembly.LoadFrom(assembly_fp);
		return assembly;
	}
}