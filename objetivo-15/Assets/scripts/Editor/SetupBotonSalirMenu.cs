using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

// Crea un boton "BotonSalir" nuevo e independiente en el panel del menu
// de la escena menuprincipal.unity, conectado a MenuPrincipal.Salir.
// No modifica ni elimina ningun boton existente (Jugar, Soluciones, Creditos).
public static class SetupBotonSalirMenu
{
    [MenuItem("Objetivo15/Agregar Botón Salir al Menú")]
    public static void AgregarBotonSalir()
    {
        string path = "Assets/Scenes/menuprincipal.unity";
        if (!File.Exists(path))
        {
            Debug.LogWarning("No existe: " + path);
            return;
        }

        // Si la escena activa no es menuprincipal, la abrimos
        if (EditorSceneManager.GetActiveScene().path != path)
            EditorSceneManager.OpenScene(path);

        // Buscar el panel del menu via la referencia de MenuPrincipal:
        // el panel puede tener cualquier nombre (en la escena se llama "menuprincipal").
        var mp = Object.FindFirstObjectByType<MenuPrincipal>();
        GameObject panel = mp != null ? mp.panelMenuPrincipal : null;

        if (panel == null)
        {
            foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if ((go.name == "PanelMenuPrincipal" || go.name == "menuprincipal") && go.scene.IsValid())
                {
                    panel = go;
                    break;
                }
            }
        }

        if (panel == null)
        {
            Debug.LogWarning("No se encontro el panel del menu principal en la escena.");
            return;
        }

        // No duplicar si ya existe
        if (panel.transform.Find("BotonSalir") != null)
        {
            Debug.Log("Ya existe 'BotonSalir' en el menu principal. No se duplico.");
            return;
        }

        // Crear el boton nuevo
        var btnGO = new GameObject("BotonSalir", typeof(RectTransform), typeof(Image), typeof(Button));
        btnGO.transform.SetParent(panel.transform, false);

        // Insertarlo ANTES del PanelInputNombre (modal fullscreen) para que
        // quede cubierto igual que Jugar/Soluciones/Creditos. SetParent lo
        // agrega al final y quedaria encima del modal (clics fantasma).
        var rtPanelNombre = panel.transform.Find("PanelInputNombre");
        if (rtPanelNombre != null)
            btnGO.transform.SetSiblingIndex(rtPanelNombre.GetSiblingIndex());

        var rt = btnGO.GetComponent<RectTransform>();
        // Posicion provisional: bajo los botones existentes (centrada).
        // El usuario la ajusta luego a su gusto.
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = Vector2.one * 0.5f;
        rt.anchoredPosition = new Vector2(0, -350);
        rt.sizeDelta = new Vector2(400, 80);

        var img = btnGO.GetComponent<Image>();
        var spriteBoton = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/botonescopia.png");
        if (spriteBoton != null)
            img.sprite = spriteBoton;
        img.color = Color.white;

        // Texto del boton
        var txtGO = new GameObject("Texto", typeof(RectTransform), typeof(TextMeshProUGUI));
        txtGO.transform.SetParent(btnGO.transform, false);
        var txtRT = txtGO.GetComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = Vector2.zero;
        txtRT.offsetMax = Vector2.zero;

        var txt = txtGO.GetComponent<TextMeshProUGUI>();
        txt.text = "SALIR";
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        txt.fontSize = 36;

        TMP_FontAsset fuente = BuscarFuente();
        if (fuente != null) txt.font = fuente;

        // Conectar onClick -> MenuPrincipal.Salir
        if (mp != null)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(
                btnGO.GetComponent<Button>().onClick, mp.Salir);
        }
        else
        {
            // No hay MenuPrincipal en escena; MenuPrincipal.Start() auto-cablea
            // el boton por nombre ("BotonSalir") en runtime, asi que igual funciona.
            Debug.LogWarning("No se encontro MenuPrincipal en la escena; el auto-cableado por nombre en Start() conectara el boton en runtime.");
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);

        Debug.Log("BotonSalir creado y conectado en menuprincipal. Ajusta su posicion/estetica en la escena.");
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "Botón SALIR agregado al menú principal.\n\nAjusta su posición y estética a tu gusto en la escena.", "OK");
    }

    private static TMP_FontAsset BuscarFuente()
    {
        TMP_FontAsset fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/fonts/PressStart2P-Regular SDF.asset");
        if (fuente == null)
            fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Resources/Transicion/fuente.asset");
        if (fuente == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            if (guids.Length > 0)
                fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
        return fuente;
    }
}
