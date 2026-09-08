using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SetupContadoresRecursos
{
    static readonly string[] NIVELES =
    {
        "Assets/Scenes/nivel1_colegio.unity",
        "Assets/Scenes/nivel2_hipodromo.unity",
        "Assets/Scenes/nivel3_mercado.unity",
        "Assets/Scenes/nivel4_basurero.unity",
        "Assets/Scenes/nivel5_subterraneo.unity"
    };

    [MenuItem("Objetivo15/Conectar Contadores de Recursos")]
    public static void Ejecutar()
    {
        int ok = 0;
        foreach (string escena in NIVELES)
        {
            EditorSceneManager.OpenScene(escena, OpenSceneMode.Single);
            bool hecho = false;

            Inventario inv = Object.FindFirstObjectByType<Inventario>();
            if (inv != null)
            {
                if (inv.contadorPET == null) inv.contadorPET = BuscarContador("ContPET");
                if (inv.contadorBolsa == null) inv.contadorBolsa = BuscarContador("ContBolsa");
                if (inv.contadorIcopor == null) inv.contadorIcopor = BuscarContador("ContIcopor");
                if (inv.contadorBanana == null) inv.contadorBanana = BuscarContador("ContBanana");
                if (inv.contadorManzana == null) inv.contadorManzana = BuscarContador("ContManzana");

                hecho = inv.contadorPET != null && inv.contadorBolsa != null &&
                        inv.contadorIcopor != null && inv.contadorBanana != null &&
                        inv.contadorManzana != null;
            }
            else
            {
                Debug.LogWarning($"[SetupContadoresRecursos] {escena}: no se encontro Inventario");
            }

            if (hecho)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), escena);
                ok++;
            }
            Debug.Log($"[SetupContadoresRecursos] {escena}: {(hecho ? "contadores conectados" : "sin cambios")}");
        }
        AssetDatabase.Refresh();
        Debug.Log($"[SetupContadoresRecursos] Contadores conectados en {ok} de {NIVELES.Length} escenas.");
    }

    static ContadorHUD BuscarContador(string nombre)
    {
        Transform contenedor = GameObject.Find("ContenedorRecursos")?.transform
                               ?? GameObject.Find("ContenedorRecursos 1")?.transform;
        if (contenedor == null) return null;

        Transform t = contenedor.Find(nombre);
        if (t == null)
        {
            foreach (Transform child in contenedor)
                if (child.name == nombre) { t = child; break; }
        }
        return t != null ? t.GetComponent<ContadorHUD>() : null;
    }

    public static void EjecutarTodoBatch()
    {
        Ejecutar();
    }
}