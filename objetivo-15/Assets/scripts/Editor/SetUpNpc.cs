using UnityEngine;
using UnityEditor;
using TMPro;

public class SetUpNpc
{
    const string rutaPrefab = "Assets/prefabs/NPC_Ambientalin.prefab";

    [MenuItem("Tools/Crear NPC Ambientalin Prefab")]
    static void CrearPrefab()
    {
        GameObject go = new GameObject("NPC_Ambientalin");

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        var spriteAmbientalin = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/AMBIENTALINBEMBA-removebg-preview.png");
        if (spriteAmbientalin != null)
            sr.sprite = spriteAmbientalin;
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 0;

        NPCDialogoJRPG npc = go.AddComponent<NPCDialogoJRPG>();
        npc.textoNPC = "!Recuerda separar los residuos: organicos, reciclables y no aprovechables. Cada material cuenta!";
        npc.textoJugador = "Tienes razon, !empecemos!";
        npc.radioDeteccion = 3f;
        npc.cooldownReaparicion = 2f;
        npc.retratoNPC = spriteAmbientalin;

        var spritePanel = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/nubecitadialogo-removebg-preview.png");
        if (spritePanel != null)
            npc.spritePanelDialogo = spritePanel;

        var guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            npc.fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        }

        PrefabUtility.SaveAsPrefabAsset(go, rutaPrefab);
        Object.DestroyImmediate(go);

        Debug.Log("NPC prefab creado en " + rutaPrefab);
    }

    [MenuItem("Tools/Colocar NPC Ambientalin en nivel1_colegio")]
    static void ColocarEnNivel1()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(rutaPrefab);
        if (prefab == null)
        {
            Debug.LogError("Primero ejecuta Tools > Crear NPC Ambientalin Prefab");
            return;
        }

        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/nivel1_colegio.unity");

        var existente = GameObject.Find("NPC_Ambientalin");
        if (existente != null)
            Object.DestroyImmediate(existente);

        var npc = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        npc.name = "NPC_Ambientalin";
        npc.transform.position = new Vector3(8f, 2f, 0f);

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        Debug.Log("NPC colocado en nivel1_colegio en posicion (8, 2, 0)");
    }
}
