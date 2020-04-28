using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using BepInEx.Logging;

namespace BepInEx.MelonPreloader
{
	internal static class InnerMain
	{
		public static void Start()
		{
			Paths.SetExecutablePath(Process.GetCurrentProcess().MainModule.FileName);

			ConsoleManager.CreateConsole();

			Logger.Listeners.Add(new ConsoleLogListener());

			var Log = Logger.CreateLogSource("BepInEx");

			string consoleTile = $"BepInEx {typeof(Paths).Assembly.GetName().Version} - {Process.GetCurrentProcess().ProcessName}";
			Log.LogMessage(consoleTile);

			if (ConsoleManager.ConsoleActive)
				ConsoleManager.SetConsoleTitle(consoleTile);

			//See BuildInfoAttribute for more information about this section.
			object[] attributes = typeof(BuildInfoAttribute).Assembly.GetCustomAttributes(typeof(BuildInfoAttribute), false);

			if (attributes.Length > 0)
			{
				var attribute = (BuildInfoAttribute)attributes[0];
				Log.LogMessage(attribute.Info);
			}

			Log.LogInfo($"Running under Unity v{FileVersionInfo.GetVersionInfo(Paths.ExecutablePath).FileVersion}");
			Log.LogInfo($"CLR runtime version: {Environment.Version}");
			Log.LogInfo($"Runtime type: IL2CPP");
			Log.LogInfo($"Supports SRE: False");


			Log.LogMessage($"Delegating launch to MelonLoader...");

			MelonLoader.Main.Initialize();
		}
	}

    public static class Main
    {
		public static void Start()
		{
			var dir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
				{
					var name = new AssemblyName(args.Name);

					var path = Path.Combine(dir, "MelonLoader", name.Name + ".dll");

					if (File.Exists(path))
					{
						return Assembly.LoadFrom(path);
					}

					File.WriteAllText(Path.Combine(dir, "fail.txt"), $"Failed to load {name.FullName} : {path}");

					return null;
				};

				InnerMain.Start();


			}
			catch (Exception ex)
			{
                Console.WriteLine(ex);

				File.WriteAllText(Path.Combine(dir, "fail.txt"), ex.ToString());
			}
		}



        internal static void OnApplicationStart()
        {

        }

        internal static void OnLevelWasLoaded(int level)
        {

        }

        internal static void OnLevelWasInitialized(int level)
        {

        }

        internal static void OnUpdate()
        {

        }

        internal static void OnFixedUpdate()
        {

        }

        internal static void OnLateUpdate()
        {

        }

        internal static void OnGUI()
        {

        }

        internal static void OnApplicationQuit()
        {

        }
    }
}
