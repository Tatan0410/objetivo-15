using UnityEngine;
using UnityEditor;
using TMPro;

public class SetUpNpc
{
    const string rutaPrefab = "Assets/prefabs/NPC_Ambientalin.prefab";
    const string rutaPrefabFondo = "Assets/prefabs/NPC_Fondo_Saltarin.prefab";

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
        npc.dialogos = new DialogoNPC[]
        {
            new DialogoNPC { esJugador = false, texto = "!Hola, joven! Me alegra que quieras aprender sobre el ODS 15. Yo soy Ambientalin." },
            new DialogoNPC { esJugador = true, texto = "Encantado! Por donde empezamos?" },
            new DialogoNPC { esJugador = false, texto = "Sigue el camino y recolecta plasticos. Cada uno te ensenara algo nuevo sobre como cuidar nuestro planeta." },
        };
        npc.radioDeteccion = 3f;

        var spritePanel = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/nubecitadialogo-removebg-preview.png");
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

    static Sprite CargarSprite(string relativePath)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>("Assets/" + relativePath);
    }

    [MenuItem("Tools/Crear NPC Fondo Saltarin Prefab")]
    static void CrearPrefabFondo()
    {
        GameObject go = new GameObject("NPC_Fondo_Saltarin");

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = -1;

        var sprite = CargarSprite("sprites/king-removebg-preview.png");
        if (sprite != null) sr.sprite = sprite;

        var spriteIdle = CargarSprite("sprites/jotyquieto-removebg-preview.png");
        var spriteSalto = CargarSprite("sprites/jotybrincoteador-removebg-preview.png");

        NPCSaltoFondo npc = go.AddComponent<NPCSaltoFondo>();
        npc.alturaSalto = 0.8f;
        npc.duracionSalto = 0.8f;
        npc.pausaEntreSaltos = 2f;
        npc.spriteIdle = spriteIdle;
        npc.spriteSalto = spriteSalto;

        PrefabUtility.SaveAsPrefabAsset(go, rutaPrefabFondo);
        Object.DestroyImmediate(go);

        Debug.Log("NPC Fondo Saltarin prefab creado en " + rutaPrefabFondo);
    }

    [MenuItem("Tools/Colocar NPCs Fondo en nivel1_colegio")]
    static void ColocarNPCsFondo()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(rutaPrefabFondo);
        if (prefab == null)
        {
            Debug.LogError("Primero ejecuta Tools > Crear NPC Fondo Saltarin Prefab");
            return;
        }

        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/nivel1_colegio.unity");

        var existente = GameObject.Find("NPCs_Fondo");
        if (existente != null)
            Object.DestroyImmediate(existente);

        GameObject contenedor = new GameObject("NPCs_Fondo");

        var spriteIdle = CargarSprite("sprites/jotyquieto-removebg-preview.png");
        var spriteSalto = CargarSprite("sprites/jotybrincoteador-removebg-preview.png");

        Vector3[] posiciones = new Vector3[]
        {
            new Vector3(3f, 0.5f, 0f),
            new Vector3(14f, 1f, 0f),
            new Vector3(25f, 0.8f, 0f),
            new Vector3(36f, 1.2f, 0f),
            new Vector3(48f, 0.7f, 0f),
        };

        for (int i = 0; i < posiciones.Length; i++)
        {
            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            instance.name = "NPC_Fondo_" + (i + 1);
            instance.transform.SetParent(contenedor.transform);
            instance.transform.position = posiciones[i];

            var npc = instance.GetComponent<NPCSaltoFondo>();
            if (npc != null)
            {
                npc.spriteIdle = spriteIdle;
                npc.spriteSalto = spriteSalto;
                var sr = instance.GetComponent<SpriteRenderer>();
                if (sr != null && spriteIdle != null) sr.sprite = spriteIdle;
                npc.alturaSalto = Random.Range(0.5f, 1f);
                npc.duracionSalto = Random.Range(0.6f, 1f);
                npc.pausaEntreSaltos = Random.Range(1.5f, 3f);
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        Debug.Log("NPCs de fondo colocados (" + posiciones.Length + ") en nivel1_colegio");
    }

    const string rutaPrefabThaliana = "Assets/prefabs/NPC_Thaliana.prefab";

    [MenuItem("Tools/Crear NPC Thaliana Prefab")]
    static void CrearPrefabThaliana()
    {
        GameObject go = new GameObject("NPC_Thaliana");

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = -1;

        var spriteIdle = CargarSprite("sprites/thalianapixelart-removebg-preview.png");
        var spriteSalto = CargarSprite("sprites/thalianabrincoteo-removebg-preview.png");

        if (spriteIdle != null) sr.sprite = spriteIdle;

        NPCSaltoFondo npc = go.AddComponent<NPCSaltoFondo>();
        npc.spriteIdle = spriteIdle;
        npc.spriteSalto = spriteSalto;

        PrefabUtility.SaveAsPrefabAsset(go, rutaPrefabThaliana);
        Object.DestroyImmediate(go);

        Debug.Log("NPC Thaliana prefab creado en " + rutaPrefabThaliana);
    }

    [MenuItem("Tools/Colocar NPCs Thaliana en nivel1_colegio")]
    static void ColocarNPCsThaliana()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(rutaPrefabThaliana);
        if (prefab == null)
        {
            Debug.LogError("Primero ejecuta Tools > Crear NPC Thaliana Prefab");
            return;
        }

        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/nivel1_colegio.unity");

        var existente = GameObject.Find("NPCs_Thaliana");
        if (existente != null)
            Object.DestroyImmediate(existente);

        GameObject contenedor = new GameObject("NPCs_Thaliana");

        var spriteIdle = CargarSprite("sprites/thalianapixelart-removebg-preview.png");
        var spriteSalto = CargarSprite("sprites/thalianabrincoteo-removebg-preview.png");

        Vector3[] posiciones = new Vector3[]
        {
            new Vector3(6f, 0.6f, 0f),
            new Vector3(18f, 0.9f, 0f),
            new Vector3(30f, 0.7f, 0f),
            new Vector3(42f, 1.1f, 0f),
            new Vector3(55f, 0.5f, 0f),
        };

        for (int i = 0; i < posiciones.Length; i++)
        {
            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            instance.name = "Thaliana_" + (i + 1);
            instance.transform.SetParent(contenedor.transform);
            instance.transform.position = posiciones[i];

            var npc = instance.GetComponent<NPCSaltoFondo>();
            if (npc != null)
            {
                npc.spriteIdle = spriteIdle;
                npc.spriteSalto = spriteSalto;
                var sr = instance.GetComponent<SpriteRenderer>();
                if (sr != null && spriteIdle != null) sr.sprite = spriteIdle;
                npc.alturaSalto = Random.Range(0.4f, 0.8f);
                npc.duracionSalto = Random.Range(0.5f, 0.9f);
                npc.pausaEntreSaltos = Random.Range(1.5f, 3f);
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        Debug.Log("NPCs Thaliana colocados (" + posiciones.Length + ") en nivel1_colegio");
    }
}
