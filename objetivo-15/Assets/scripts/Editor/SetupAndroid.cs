using UnityEditor;
using UnityEngine;

public static class SetupAndroid
{
    const string PACKAGE_ID = "com.objetivo15.game";

    [MenuItem("Objetivo15/Configurar Ajustes Android")]
    public static void Configurar()
    {
        PlayerSettings.companyName = "objetivo15";
        PlayerSettings.productName = "Objetivo 15";

        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, PACKAGE_ID);

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34;
        PlayerSettings.Android.bundleVersionCode = 1;

        EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;

        if (BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Android, BuildTarget.Android);
        else
            Debug.LogWarning("[SetupAndroid] Modulo Android no instalado; los ajustes se guardaron pero no se cambio el target de build. Instala Android Build Support desde Unity Hub y vuelve a correr esto.");

        AssetDatabase.SaveAssets();
        Debug.Log("[SetupAndroid] Ajustes aplicados. Package ID: " + PACKAGE_ID);
    }

    public static void EjecutarTodoBatch()
    {
        Configurar();
    }
}