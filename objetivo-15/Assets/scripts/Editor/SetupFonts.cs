using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;
using UnityEngine.TextCore.LowLevel;

public static class SetupFonts
{
    public static void GenerarFontAssets()
    {
        string[] ttfPaths = Directory.GetFiles("Assets/fonts", "*.ttf", SearchOption.AllDirectories);

        if (ttfPaths.Length == 0)
        {
            Debug.Log("No se encontraron archivos .ttf en Assets/fonts/");
            return;
        }

        foreach (string ttfPath in ttfPaths)
        {
            string assetPath = Path.ChangeExtension(ttfPath, ".asset");
            if (File.Exists(assetPath))
            {
                Debug.Log($"Ya existe: {assetPath}");
                continue;
            }

            string recursoPath = Path.Combine("Assets/Resources/Transicion", Path.GetFileName(assetPath));
            if (File.Exists(recursoPath))
            {
                Debug.Log($"Ya existe en Resources: {recursoPath}");
                continue;
            }

            Font unityFont = AssetDatabase.LoadAssetAtPath<Font>(ttfPath);
            if (unityFont == null)
            {
                Debug.LogWarning($"No se pudo cargar fuente: {ttfPath}");
                continue;
            }

            FontEngine.InitializeFontEngine();
            FontEngine.LoadFontFace(unityFont);

            TMP_FontAsset tmpFont = TMP_FontAsset.CreateFontAsset(unityFont, 90, 9, GlyphRenderMode.SDFAA, 2048, 2048);
            if (tmpFont == null)
            {
                Debug.LogWarning($"No se pudo crear TMP_FontAsset desde: {ttfPath}");
                continue;
            }

            tmpFont.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            tmpFont.ReadFontAssetDefinition();

            AssetDatabase.CreateAsset(tmpFont, assetPath);
            AssetDatabase.SaveAssets();

            if (tmpFont.atlasTexture == null || tmpFont.atlasTexture.width == 0)
            {
                AssetDatabase.DeleteAsset(assetPath);
                Debug.LogWarning($"Atlas inválido (0x0). Reintentando con GlyphRenderMode.SMOOTH...");

                FontEngine.InitializeFontEngine();
                FontEngine.LoadFontFace(unityFont);

                tmpFont = TMP_FontAsset.CreateFontAsset(unityFont, 36, 5, GlyphRenderMode.SMOOTH, 1024, 1024);
                if (tmpFont != null)
                {
                    tmpFont.atlasPopulationMode = AtlasPopulationMode.Dynamic;
                    tmpFont.ReadFontAssetDefinition();
                    AssetDatabase.CreateAsset(tmpFont, assetPath);
                    AssetDatabase.SaveAssets();
                }
            }

            if (tmpFont.atlasTexture == null || tmpFont.atlasTexture.width == 0)
            {
                if (tmpFont != null)
                    Object.DestroyImmediate(tmpFont, true);
                Debug.LogError($"Font Asset NO generado en batch mode: {assetPath}.\nAbre Unity y corre: Tools → Generar Font Assets TMP desde .ttf");
            }
            else
            {
                tmpFont.material.mainTexture.filterMode = FilterMode.Point;
                Debug.Log($"Font Asset creado: {assetPath} ({tmpFont.atlasTexture.width}x{tmpFont.atlasTexture.height})");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Proceso completado.");
    }
}
