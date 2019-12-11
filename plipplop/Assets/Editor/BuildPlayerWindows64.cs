
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
public class BuildPlayerWindows64 : MonoBehaviour
{
    static string buildLocation = @"D:\PROJETS\PLIPPLOP\plipplop\PLIP_PLOP_BUILD\Build.exe";

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
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Debug/Diorama.unity" };
        buildPlayerOptions.locationPathName = location;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded) {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed) {
            Debug.LogError("Build failed");
        }
    }
}
