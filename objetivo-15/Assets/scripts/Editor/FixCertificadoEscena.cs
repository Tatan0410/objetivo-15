using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class FixCertificadoEscena
{
    [MenuItem("Objetivo15/Reparar Escena Certificado")]
    public static void Reparar()
    {
        string scenePath = "Assets/Scenes/certificado.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);

        // 1. Obtener fuente recomendada (PressStart2P o fallback)
        TMP_FontAsset fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/fonts/PressStart2P-Regular SDF.asset");
        if (fuente == null)
            fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Resources/Transicion/fuente.asset");

        // 2. Corregir FondoCertificado
        var fondo = GameObject.Find("FondoCertificado");
        if (fondo != null)
        {
            fondo.transform.SetAsFirstSibling();
            var img = fondo.GetComponent<Image>();
            if (img != null)
            {
                img.raycastTarget = false;
                EditorUtility.SetDirty(img);
            }

            var rt = fondo.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;
                EditorUtility.SetDirty(rt);
            }
            EditorUtility.SetDirty(fondo);
            Debug.Log("FondoCertificado reparado: raycastTarget=false, tamaño pantalla completa y colocado al fondo.");
        }

        // 3. Revisar y reparar todos los TextMeshPro de la escena
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            var tmps = canvas.GetComponentsInChildren<TMP_Text>(true);
            foreach (var t in tmps)
            {
                if (t.font == null && fuente != null)
                {
                    t.font = fuente;
                    EditorUtility.SetDirty(t);
                    Debug.Log($"Fuente asignada a: {t.gameObject.name}");
                }
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Escena certificado guardada y reparada con éxito.");
    }
}
