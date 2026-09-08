using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildAndroid
{
    const string APK_PATH = "Builds/objetivo15.apk";

    [MenuItem("Objetivo15/Build Android (APK)")]
    public static void Construir()
    {
        SetupAndroid.Configurar();

        string[] escenas = EditorBuildSettingsScene.GetActiveSceneList(
            EditorBuildSettings.scenes);

        if (escenas == null || escenas.Length == 0)
        {
            Debug.LogError("[BuildAndroid] No hay escenas activas en Build Settings.");
            return;
        }

        var report = BuildPipeline.BuildPlayer(escenas, APK_PATH, BuildTarget.Android, BuildOptions.None);
        BuildSummary resumen = report.summary;
        if (resumen.result == BuildResult.Succeeded)
        {
            Debug.Log("[BuildAndroid] APK generado: " + APK_PATH + " (" + resumen.totalSize + " bytes)");
        }
        else
        {
            Debug.LogError("[BuildAndroid] Build fallo: " + resumen.result);
        }
    }

    public static void EjecutarTodoBatch()
    {
        Construir();
    }
}