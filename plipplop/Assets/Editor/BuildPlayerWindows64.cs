
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildPlayerWindows64 : MonoBehaviour
{
    static string buildLocation = Path.Combine(Directory.GetCurrentDirectory(), "_BUILD");

    [MenuItem("Tools/Build diorama")]
    public static void BuildDiorama()
    {
        Build(buildLocation, new[] { "Assets/Scenes/Debug/Diorama.unity" });
    }

    [MenuItem("Tools/Build world scene")]
    public static void BuildWorldScene()
    {
        Build(buildLocation, new[] { "Assets/Scenes/Main.unity" });
    }

    static void Build(string location, params string[] scenes)
    {
        if (Directory.Exists(location)) {
            Directory.Delete(location, true);
        }
        Directory.CreateDirectory(location);
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = Path.Combine(location, "Build.exe");
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded) {
            Debug.Log("Built "+string.Join(", ", scenes)+ " to ["+ location + "] (" + (summary.totalSize/1000f) + " KB)");
        }

        if (summary.result == BuildResult.Failed) {
            Debug.LogError("Build failed");
        }
    }
}
