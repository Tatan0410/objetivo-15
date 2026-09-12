using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public static class BuildPC
{
    const string VERSION = "1.11";
    const string BUILD_ROOT = "Build";

    [MenuItem("Objetivo15/Build PC (Windows)")]
    public static void Construir()
    {
        Configurar();

        string[] escenas = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        if (escenas == null || escenas.Length == 0)
        {
            Debug.LogError("[BuildPC] No hay escenas activas en Build Settings.");
            return;
        }

        // Carpeta de salida en la raiz del repositorio (fuera del proyecto Unity),
        // misma ubicacion que el build 1.10.
        string carpetaBase = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", BUILD_ROOT));
        string carpeta = Path.Combine(carpetaBase, "Objetivo 15 version " + VERSION);
        string exe = Path.Combine(carpeta, "Objetivo 15 version " + VERSION + ".exe");

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        var opciones = new BuildPlayerOptions
        {
            scenes = escenas,
            locationPathName = exe,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(opciones);
        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log("[BuildPC] Build OK: " + exe + " (" + report.summary.totalSize + " bytes)");
        else
            Debug.LogError("[BuildPC] Build fallo: " + report.summary.result);
    }

    static void Configurar()
    {
        PlayerSettings.companyName = "objetivo15";
        PlayerSettings.productName = "Objetivo 15 version " + VERSION;

        SetupAndroid.ConfigurarIconoWindows();

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

        AssetDatabase.SaveAssets();
    }
}