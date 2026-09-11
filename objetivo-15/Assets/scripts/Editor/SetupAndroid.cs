using UnityEditor;
using UnityEngine;

public static class SetupAndroid
{
    const string PACKAGE_ID = "com.objetivo15.game";
    const string LOGO_ICONO = "Assets/sprites/logojuego.jpg";

    [MenuItem("Objetivo15/Configurar Ajustes Android")]
    public static void Configurar()
    {
        PlayerSettings.companyName = "objetivo15";
        PlayerSettings.productName = "Objetivo 15";

        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, PACKAGE_ID);

        ConfigurarIcono();

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

    // Configura el icono de la app (launcher) para que no haya que hacerlo
    // a mano en cada build. Se usa el logo logojuego.jpg.
    static void ConfigurarIcono()
    {
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(LOGO_ICONO);
        if (tex == null)
        {
            Debug.LogError("[SetupAndroid] No se encontro el icono en " + LOGO_ICONO);
            return;
        }

        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new Texture2D[] { tex });

        Debug.Log("[SetupAndroid] Icono de app configurado desde " + LOGO_ICONO);
    }

    public static void EjecutarTodoBatch()
    {
        Configurar();
    }

    // Aplica el mismo logo (logojuego.jpg) como icono del .exe de Windows.
    [MenuItem("Objetivo15/Configurar Icono Windows")]
    public static void ConfigurarIconoWindows()
    {
        if (AsegurarLegible())
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(LOGO_ICONO);
            if (tex == null)
            {
                Debug.LogError("[SetupWindows] No se encontro el icono en " + LOGO_ICONO);
                return;
            }

            // Standalone (Windows) espera exactamente 8 iconos (un set de 8 tamaños).
            var icons = new Texture2D[8];
            for (int i = 0; i < icons.Length; i++) icons[i] = tex;
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, icons);
            AssetDatabase.SaveAssets();
        }

        var sa = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Standalone);
        Debug.Log("[SetupWindows] Standalone icons=" + sa.Length + " [0]=" + (sa.Length > 0 && sa[0] != null ? sa[0].name : "null"));
    }

    // Unity no puede usar una textura como icono si no tiene Read/Write habilitado.
    // logojuego.jpg viene con isReadable=false, asi que lo activamos y reimportamos.
    static bool AsegurarLegible()
    {
        var importer = AssetImporter.GetAtPath(LOGO_ICONO) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError("[SetupWindows] No se pudo obtener el TextureImporter de " + LOGO_ICONO);
            return false;
        }

        if (!importer.isReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }
        return true;
    }
}