using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public class OVRConfig : ScriptableObject
{
	[SerializeField]
	private string androidSDKPath = "";

	[SerializeField]
	private string gradlePath = "";

	[SerializeField]
	private string jdkPath = "";

	private static OVRConfig instance;

	public static OVRConfig Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load<OVRConfig>("OVRConfig");
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<OVRConfig>();
					string resourcePath = Path.Combine(UnityEngine.Application.dataPath, "Resources");
					if (!Directory.Exists(resourcePath))
					{
						UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
					}

					string fullPath = Path.Combine(Path.Combine("Assets", "Resources"), "OVRBuildConfig.asset");
					UnityEditor.AssetDatabase.CreateAsset(instance, fullPath);
				}
			}
			return instance;
		}
		set
		{
			instance = value;
		}
	}

	// Returns the path to the base directory of the Android SDK
	public string GetAndroidSDKPath(bool throwError = true)
	{
#if UNITY_2019_1_OR_NEWER
		// Check for use of embedded path or user defined 
		bool useEmbedded = EditorPrefs.GetBool("SdkUseEmbedded");
		if (useEmbedded)
		{
			androidSDKPath = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None), "SDK");
		}
		else
#endif
		{
			androidSDKPath = EditorPrefs.GetString("AndroidSdkRoot");
		}

		androidSDKPath = androidSDKPath.Replace('/', Path.DirectorySeparatorChar);
		// Validate sdk path and notify user if path does not exist.
		if (!Directory.Exists(androidSDKPath))
		{
			androidSDKPath = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
			if (!string.IsNullOrEmpty(androidSDKPath))
			{
				return androidSDKPath;
			}

			if (throwError)
			{
				EditorUtility.DisplayDialog("Android SDK not Found",
						"Android SDK not found. Please ensure that the path is set correctly in (Edit -> Preferences -> External Tools) or that the Untiy Android module is installed correctly.",
						"Ok");
			}
			return string.Empty;
		}

		EditorUtility.SetDirty(Instance);
		return androidSDKPath;
	}

	private string GetAndroidPlayerPath()
	{
		string basePath = EditorApplication.applicationContentsPath;
		while (!String.IsNullOrEmpty(basePath))
		{
			string path = Path.Combine(basePath, "PlaybackEngines", "AndroidPlayer");
			if (Directory.Exists(path))
			{
				return path;
			}
			basePath = Path.GetDirectoryName(basePath);
		}

		EditorUtility.DisplayDialog("Android module not Found",
				"Android module not found. Please that the Unity Android module is installed correctly.",
				"Ok");
		return string.Empty;
	}

	// Returns the path to the gradle-launcher-*.jar
	public string GetGradlePath(bool throwError = true)
	{
		string libPath = "";
#if UNITY_2019_1_OR_NEWER
		// Check for use of embedded path or user defined 
		bool useEmbedded = EditorPrefs.GetBool("GradleUseEmbedded");

		if (useEmbedded)
		{
			libPath = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None), "Tools","gradle","lib");
		}
		else
		{
			libPath = Path.Combine(EditorPrefs.GetString("GradlePath"), "lib");
		}
#else
		libPath = Path.Combine(GetAndroidPlayerPath(), "Tools", "gradle", "lib");
#endif

		libPath = libPath.Replace('/', Path.DirectorySeparatorChar);
		if (!string.IsNullOrEmpty(libPath) && Directory.Exists(libPath))
		{
			string[] gradleJar = Directory.GetFiles(libPath, "gradle-launcher-*.jar");
			if (gradleJar.Length == 1)
			{
				gradlePath = gradleJar[0];
			}
		}

		// Validate gradle path and notify user if path does not exist.
		if (!File.Exists(gradlePath))
		{
			if (throwError)
			{
				EditorUtility.DisplayDialog("Gradle not Found",
						"Gradle not found. Please ensure that the path is set correctly in (Edit -> Preferences -> External Tools) or that the Untiy Android module is installed correctly.",
						"Ok");
			}
			return string.Empty;
		}

		EditorUtility.SetDirty(Instance);
		return gradlePath;
	}

	// Returns path to the Java executable in the JDK
	public string GetJDKPath(bool throwError = true)
	{
#if UNITY_EDITOR_WIN
		const string exeExtension = ".exe";
#else
		const string exeExtension = "";
#endif

#if UNITY_EDITOR_WIN
		const string osDirName = "Windows";
#elif UNITY_EDITOR_OSX
		const string osDirName = "MacOS";
#elif UNITY_EDITOR_LINUX
		const string osDirName = "Linux";
#endif

#if UNITY_EDITOR
		// Check for use of embedded path or user defined 
		bool useEmbedded = EditorPrefs.GetBool("JdkUseEmbedded");

		string exePath = "";
		if (useEmbedded)
		{
#if UNITY_2019_1_OR_NEWER
			exePath = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None), "Tools", "OpenJDK", osDirName, "bin");
#else
			exePath = Path.Combine(GetAndroidPlayerPath(), "Tools", "OpenJDK", osDirName, "bin");
#endif
		}
		else
		{
			exePath = Path.Combine(EditorPrefs.GetString("JdkPath"), "bin");
		}

		jdkPath = Path.Combine(exePath, $"java{exeExtension}");
		jdkPath = jdkPath.Replace('/', Path.DirectorySeparatorChar);

		// Validate gradle path and notify user if path does not exist.
		if (!File.Exists(jdkPath))
		{
			// Check the enviornment variable as a backup to see if the JDK is there.
			string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
			if(!string.IsNullOrEmpty(javaHome))
			{
				jdkPath = Path.Combine(javaHome, "bin", $"java{exeExtension}");
				if(File.Exists(jdkPath))
				{
					EditorUtility.SetDirty(Instance);
					return jdkPath;
				}
			}

			if (throwError)
			{
				EditorUtility.DisplayDialog("JDK not Found",
					"JDK not found. Please ensure that the path is set correctly in (Edit -> Preferences -> External Tools) or that the Untiy Android module is installed correctly.",
					"Ok");
			}
			return string.Empty;
		}
#endif
		EditorUtility.SetDirty(Instance);
		return jdkPath;
	}
}
