using UnityEngine;
using UnityEditor;

public static class FixEnemigos
{
    static readonly string[] PREFABS_ENEMIGO =
    {
        "Assets/prefabs/enemigo.prefab",
        "Assets/prefabs/EnemigoVolador.prefab",
        "Assets/prefabs/rata.prefab"
    };

    static readonly string[] PREFABS_PLASTICO =
    {
        "Assets/prefabs/botella.prefab",
        "Assets/prefabs/bolsaplastica.prefab",
        "Assets/prefabs/icopor.prefab"
    };

    public static void Ejecutar()
    {
        GameObject[] plasticos = new GameObject[PREFABS_PLASTICO.Length];
        for (int i = 0; i < PREFABS_PLASTICO.Length; i++)
        {
            plasticos[i] = AssetDatabase.LoadAssetAtPath<GameObject>(PREFABS_PLASTICO[i]);
            if (plasticos[i] == null)
                Debug.LogError("No se encontro el prefab de plastico: " + PREFABS_PLASTICO[i]);
        }

        foreach (string path in PREFABS_ENEMIGO)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null)
            {
                Debug.LogError("No se encontro el prefab: " + path);
                continue;
            }

            bool modificado = false;

            var en = go.GetComponent<Enemigo>();
            if (en != null)
            {
                en.prefabsPlasticos = plasticos;
                en.probabilidadDrop = 1f;
                EditorUtility.SetDirty(en);
                modificado = true;
            }

            var vol = go.GetComponent<EnemigoVolador>();
            if (vol != null)
            {
                vol.prefabsPlasticos = plasticos;
                vol.probabilidadDrop = 1f;
                EditorUtility.SetDirty(vol);
                modificado = true;
            }

            var rata = go.GetComponent<rata>();
            if (rata != null)
            {
                rata.prefabsPlasticos = plasticos;
                rata.probabilidadDrop = 1f;
                EditorUtility.SetDirty(rata);
                modificado = true;
            }

            if (modificado)
            {
                PrefabUtility.SavePrefabAsset(go);
                Debug.Log("Prefab arreglado: " + path);
            }
            else
            {
                Debug.LogWarning("Sin componente de enemigo en: " + path);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[FixEnemigos] Drops de plasticos configurados en los prefabs de enemigos.");
    }

    public static void EjecutarTodoBatch()
    {
        Ejecutar();
    }
}
