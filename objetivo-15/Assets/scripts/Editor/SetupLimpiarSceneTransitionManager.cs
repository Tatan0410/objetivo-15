using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

// Limpia los SceneTransitionManager EMBEBIDOS en las escenas.
// Razon: el manager real lo crea el Bootstrapper al arranque (duracionMinima
// central de 4s), y los embebidos siempre se autodestruian en Awake, por lo
// que sus valores (como el 10 de menuprincipal) nunca aplicaban y solo
// generaban confusion. Despues de esto, existe UN SOLO SceneTransitionManager
// centralizado.
public static class SetupLimpiarSceneTransitionManager
{
    private static readonly string[] ESCENAS = new string[]
    {
        "Assets/Scenes/menuprincipal.unity",
        "Assets/Scenes/nivel1_colegio.unity",
        "Assets/Scenes/nivel2_hipodromo.unity",
        "Assets/Scenes/nivel3_mercado.unity",
        "Assets/Scenes/nivel4_basurero.unity",
        "Assets/Scenes/nivel5_subterraneo.unity",
        "Assets/Scenes/nivel6_empresa.unity",
        "Assets/Scenes/splash.unity"
    };

    [MenuItem("Objetivo15/Limpiar SceneTransitionManager de Escenas")]
    public static void Limpiar()
    {
        int escenasLimpiadas = 0;
        int totales = 0;

        foreach (string ruta in ESCENAS)
        {
            if (!File.Exists(ruta))
            {
                Debug.LogWarning("[LimpiarSTM] No existe: " + ruta);
                continue;
            }

            var escena = EditorSceneManager.OpenScene(ruta);

            // Buscar TODOS los SceneTransitionManager de la escena (pueden ser varios)
            var managers = Resources.FindObjectsOfTypeAll<SceneTransitionManager>();
            int eliminados = 0;
            foreach (var m in managers)
            {
                if (m.gameObject.scene.path == escena.path)
                {
                    Object.DestroyImmediate(m.gameObject);
                    eliminados++;
                }
            }

            if (eliminados > 0)
            {
                EditorSceneManager.MarkSceneDirty(escena);
                EditorSceneManager.SaveScene(escena, ruta);
                escenasLimpiadas++;
            }
            totales += eliminados;
        }

        string msg = $"[LimpiarSTM] Listo. Eliminados {totales} SceneTransitionManager de {escenasLimpiadas} escenas.";
        Debug.Log(msg);
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", msg, "OK");
    }
}
