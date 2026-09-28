using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Utilidades del certificado y del mapa:
//  - Agregar/ver el botón "Ver Certificado" en el mapa limpio
//  - Revertir el botón "Imprimir" (no deseado)
//  - Preview del mapa limpio en el Editor para editar el botón
// Batch mode: Unity -batchmode -quit -executeMethod SetupCertificadoBotones.EjecutarBatch
public static class SetupCertificadoBotones
{
    const string EscenaCertificado = "Assets/Scenes/certificado.unity";
    const string EscenaMapa = "Assets/Scenes/Mapamundial.unity";
    const string SpriteMapaLimpio = "Assets/Resources/mapamundiallimpio.png";

    // Claves SessionState para el preview (se borran al cerrar el Editor)
    const string kPrevActivo = "MapaPreview_Activo";
    const string kPrevNivel = "MapaPreview_Nivel";
    const string kPrevSprite = "MapaPreview_SpritePath";
    const string kPrevContenedor = "MapaPreview_Contenedor";
    const string kPrevNodos = "MapaPreview_Nodos";
    const string kPrevRejugar = "MapaPreview_Rejugar";
    const string kPrevMenu = "MapaPreview_Menu";
    const string kPrevCert = "MapaPreview_Cert";

    // ─────────────────────────────────────────────────────────────────────────
    //  Revertir botón Imprimir (deshacer)
    // ─────────────────────────────────────────────────────────────────────────
    [MenuItem("Objetivo15/Quitar Boton Imprimir Certificado")]
    public static void RevertirBotonImprimir()
    {
        if (!System.IO.File.Exists(EscenaCertificado)) { Debug.LogError("No existe: " + EscenaCertificado); return; }

        EditorSceneManager.OpenScene(EscenaCertificado);

        DestruirPorNombre("BotonImprimir");

        var btnDesc = GameObject.Find("BotonDescargar");
        if (btnDesc != null)
        {
            var rt = btnDesc.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EscenaCertificado);
        Debug.Log("[Setup] Boton 'Imprimir' eliminado; BotonDescargar restaurado al centro");
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "Botón 'Imprimir' eliminado y Descargar restaurado al centro.", "OK");
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Botón "Ver Certificado" en el mapa limpio
    // ─────────────────────────────────────────────────────────────────────────
    [MenuItem("Objetivo15/Agregar Boton Ver Certificado en Mapa Limpio")]
    public static void AgregarBotonVerCertificadoMapa()
    {
        if (!System.IO.File.Exists(EscenaMapa)) { Debug.LogError("No existe: " + EscenaMapa); return; }

        EditorSceneManager.OpenScene(EscenaMapa);

        var mgr = Object.FindFirstObjectByType<MapamundialEstadoManager>();
        if (mgr == null) { Debug.LogError("No hay MapamundialEstadoManager en Mapamundial"); return; }

        // Canvas UI: el que contiene a SelectorNivelesMapa / BotonMenuPrincipal
        var selector = Object.FindFirstObjectByType<SelectorNivelesMapa>();
        Transform canvasT = selector != null && selector.transform.parent != null
            ? selector.transform.parent
            : (Object.FindFirstObjectByType<Canvas>() != null ? Object.FindFirstObjectByType<Canvas>().transform : null);
        if (canvasT == null) { Debug.LogError("No hay Canvas en Mapamundial"); return; }

        // Idempotente: limpiar versión previa
        DestruirPorNombre("BotonCertificado");

        // Fuente: copiarla del BotonRejugar si existe
        TMP_FontAsset fuente = null;
        var rejugar = GameObject.Find("BotonRejugar");
        if (rejugar != null)
        {
            var tmpR = rejugar.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpR != null) fuente = tmpR.font;
        }
        if (fuente == null)
        {
            var tmpMenu = Object.FindFirstObjectByType<TextMeshProUGUI>();
            if (tmpMenu != null) fuente = tmpMenu.font;
        }

        // Botón Ver Certificado (esquina inferior izquierda)
        var btn = new GameObject("BotonCertificado", typeof(RectTransform));
        btn.transform.SetParent(canvasT, false);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;
        rt.anchoredPosition = new Vector2(20, 20);
        rt.sizeDelta = new Vector2(300, 55);

        var img = btn.AddComponent<Image>();
        img.color = new Color(0.2f, 0.4f, 0.6f, 1f);
        var btnComp = btn.AddComponent<Button>();
        btnComp.targetGraphic = img;

        var txtGO = new GameObject("Text", typeof(RectTransform));
        txtGO.transform.SetParent(btn.transform, false);
        var txtRT = txtGO.GetComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = Vector2.zero;
        txtRT.offsetMax = Vector2.zero;
        var txt = txtGO.AddComponent<TextMeshProUGUI>();
        txt.text = "📜 VER CERTIFICADO";
        txt.fontSize = 18;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.Center;
        if (fuente != null) txt.font = fuente;

        UnityEventTools.AddPersistentListener(btnComp.onClick, mgr.AbrirCertificado);
        mgr.botonCertificado = btn;

        // Estado inicial: oculto; AplicarMapa() lo muestra solo si el mapa está limpio
        btn.SetActive(false);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EscenaMapa);
        Debug.Log("[Setup] Boton 'Ver Certificado' agregado a Mapamundial.unity");
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "Botón 'Ver Certificado' agregado a Mapamundial.", "OK");
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Preview del mapa limpio (modo edición) para editar el botón
    // ─────────────────────────────────────────────────────────────────────────
    [MenuItem("Objetivo15/Preview Mapa Limpio")]
    public static void PreviewMapaLimpio()
    {
        if (Application.isPlaying) { Debug.LogWarning("[Preview] Sale del Play Mode primero"); return; }
        if (!AbrirMapaSiHaceFalta()) return;

        var mgr = Object.FindFirstObjectByType<MapamundialEstadoManager>();
        if (mgr == null) { Debug.LogError("[Preview] No hay MapamundialEstadoManager"); return; }

        var fondo = FondoSpriteRenderer();
        if (fondo == null) { Debug.LogError("[Preview] No se encontró el SpriteRenderer 'fondo'"); return; }

        // Guardar estado previo solo la primera vez
        if (!SessionState.GetBool(kPrevActivo, false))
        {
            SessionState.SetBool(kPrevActivo, true);
            SessionState.SetInt(kPrevNivel, PlayerPrefs.GetInt("NivelDesbloqueado", 0));
            SessionState.SetString(kPrevSprite, AssetDatabase.GetAssetPath(fondo.sprite));
            SessionState.SetString(kPrevContenedor, mgr.contenedorNiveles != null && mgr.contenedorNiveles.activeSelf ? "1" : "0");
            SessionState.SetBool(kPrevRejugar, mgr.botonRejugar != null && mgr.botonRejugar.activeSelf);
            SessionState.SetBool(kPrevMenu, mgr.botonMenuPrincipal != null && mgr.botonMenuPrincipal.activeSelf);
            SessionState.SetBool(kPrevCert, mgr.botonCertificado != null && mgr.botonCertificado.activeSelf);

            var sel0 = Object.FindFirstObjectByType<SelectorNivelesMapa>();
            if (sel0 != null && sel0.nodos != null)
            {
                var sb = new StringBuilder();
                foreach (var n in sel0.nodos)
                    sb.Append(n != null && n.boton != null && n.boton.gameObject.activeSelf ? "1" : "0");
                SessionState.SetString(kPrevNodos, sb.ToString());
            }
        }

        // ── Estado limpio ──
        PlayerPrefs.SetInt("NivelDesbloqueado", 6);
        PlayerPrefs.Save();

        var limpio = AssetDatabase.LoadAssetAtPath<Sprite>(SpriteMapaLimpio);
        if (limpio != null) fondo.sprite = limpio;

        var sel = Object.FindFirstObjectByType<SelectorNivelesMapa>();
        if (sel != null && sel.nodos != null)
            foreach (var n in sel.nodos)
                if (n != null && n.boton != null) n.boton.gameObject.SetActive(false);

        if (mgr.contenedorNiveles != null) mgr.contenedorNiveles.SetActive(false);
        if (mgr.botonRejugar != null) mgr.botonRejugar.SetActive(true);
        if (mgr.botonMenuPrincipal != null) mgr.botonMenuPrincipal.SetActive(true);

        if (mgr.botonCertificado != null)
        {
            mgr.botonCertificado.SetActive(true);
            Selection.activeGameObject = mgr.botonCertificado;
        }

        Debug.Log("[Preview] Mapa limpio aplicado; BotonCertificado seleccionado para editar");
        EditorUtility.DisplayDialog("Preview: Mapa Limpio",
            "Fondo limpio aplicado y BotonCertificado seleccionado y activo.\n\n" +
            "Edítalo en la vista Scene (posición, tamaño, sprite/color del Image) y guarda la escena.\n\n" +
            "⚠️ ANTES de guardar, ejecuta:\nObjetivo15 → Preview Restaurar Mapa Normal\n" +
            "para devolver el fondo sucio y los niveles.",
            "OK");
    }

    [MenuItem("Objetivo15/Preview Restaurar Mapa Normal")]
    public static void PreviewRestaurarMapaNormal()
    {
        if (!SessionState.GetBool(kPrevActivo, false))
        {
            Debug.LogWarning("[Preview] No hay preview activo");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Preview", "No hay preview activo.", "OK");
            return;
        }

        if (Application.isPlaying) { Debug.LogWarning("[Preview] Sale del Play Mode primero"); return; }
        if (!AbrirMapaSiHaceFalta()) return;

        var mgr = Object.FindFirstObjectByType<MapamundialEstadoManager>();
        var fondo = FondoSpriteRenderer();

        // Nivel previo
        PlayerPrefs.SetInt("NivelDesbloqueado", SessionState.GetInt(kPrevNivel, 0));
        PlayerPrefs.Save();

        // Sprite previo del fondo
        string spritePath = SessionState.GetString(kPrevSprite, "");
        if (fondo != null && !string.IsNullOrEmpty(spritePath))
            fondo.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

        // Contenedor de niveles
        if (mgr != null && mgr.contenedorNiveles != null)
            mgr.contenedorNiveles.SetActive(SessionState.GetString(kPrevContenedor, "1") == "1");

        // Nodos
        string estados = SessionState.GetString(kPrevNodos, "");
        var sel = Object.FindFirstObjectByType<SelectorNivelesMapa>();
        if (sel != null && sel.nodos != null && estados.Length == sel.nodos.Length)
        {
            for (int i = 0; i < sel.nodos.Length; i++)
                if (sel.nodos[i] != null && sel.nodos[i].boton != null)
                    sel.nodos[i].boton.gameObject.SetActive(estados[i] == '1');
        }

        // Botones
        if (mgr != null)
        {
            if (mgr.botonRejugar != null) mgr.botonRejugar.SetActive(SessionState.GetBool(kPrevRejugar, true));
            if (mgr.botonMenuPrincipal != null) mgr.botonMenuPrincipal.SetActive(SessionState.GetBool(kPrevMenu, true));
            if (mgr.botonCertificado != null) mgr.botonCertificado.SetActive(SessionState.GetBool(kPrevCert, false));
        }

        SessionState.EraseBool(kPrevActivo);
        SessionState.EraseInt(kPrevNivel);
        SessionState.EraseString(kPrevSprite);
        SessionState.EraseString(kPrevContenedor);
        SessionState.EraseString(kPrevNodos);
        SessionState.EraseBool(kPrevRejugar);
        SessionState.EraseBool(kPrevMenu);
        SessionState.EraseBool(kPrevCert);

        Debug.Log("[Preview] Mapa restaurado al estado normal");
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Preview", "Mapa restaurado al estado normal. Ya puedes guardar la escena.", "OK");
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Reparar escena guardada con el estado del preview (fondo limpio, nodos
    //  ocultos, boton certificado activo) → volver al comportamiento original
    // ─────────────────────────────────────────────────────────────────────────
    [MenuItem("Objetivo15/Reparar Mapa Guardado por Preview")]
    public static void RepararMapamundialPostPreview()
    {
        if (Application.isPlaying) { Debug.LogWarning("[Reparar] Sale del Play Mode primero"); return; }
        if (!System.IO.File.Exists(EscenaMapa)) { Debug.LogError("No existe: " + EscenaMapa); return; }

        EditorSceneManager.OpenScene(EscenaMapa);

        // 1) Fondo de vuelta al mapa contaminado original
        var fondo = FondoSpriteRenderer();
        var sucio = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/mapamundialbeta.png");
        if (fondo != null && sucio != null)
        {
            fondo.sprite = sucio;
            Debug.Log("[Reparar] Fondo restaurado a mapamundialbeta");
        }
        else
            Debug.LogError("[Reparar] No se pudo restaurar el fondo (fondo=" + (fondo != null) + " sprite=" + (sucio != null) + ")");

        // 2) Nodos y contenedor visibles, boton certificado oculto
        var mgr = Object.FindFirstObjectByType<MapamundialEstadoManager>();
        if (mgr != null)
        {
            if (mgr.contenedorNiveles != null) mgr.contenedorNiveles.SetActive(true);

            var sel = Object.FindFirstObjectByType<SelectorNivelesMapa>();
            if (sel != null && sel.nodos != null)
                foreach (var n in sel.nodos)
                    if (n != null && n.boton != null) n.boton.gameObject.SetActive(true);

            // Solo cambia el estado activo: posicion, tamano, sprite y evento se conservan
            if (mgr.botonCertificado != null) mgr.botonCertificado.SetActive(false);
            Debug.Log("[Reparar] Nodos/contenedor activos; BotonCertificado oculto");
        }
        else
            Debug.LogError("[Reparar] No hay MapamundialEstadoManager");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EscenaMapa);

        // 3) PlayerPrefs del Editor: borrar progreso/estadisticas del preview
        PlayerPrefs.DeleteKey("NivelDesbloqueado");
        PlayerPrefs.DeleteKey("EstadisticasGuardadas");
        PlayerPrefs.DeleteKey("StatsPlasticos");
        PlayerPrefs.DeleteKey("StatsEnemigos");
        PlayerPrefs.DeleteKey("StatsTiempo");
        PlayerPrefs.Save();
        Debug.Log("[Reparar] PlayerPrefs del editor limpiadas (NivelDesbloqueado, Stats)");

        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Reparación completa",
                "Mapa restaurado (fondo contaminado, nodos visibles, certificado oculto) " +
                "y progreso/estadísticas del editor borrados.", "OK");
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Auxiliares
    // ─────────────────────────────────────────────────────────────────────────
    static bool AbrirMapaSiHaceFalta()
    {
        if (EditorSceneManager.GetActiveScene().path == EscenaMapa) return true;
        if (!System.IO.File.Exists(EscenaMapa)) { Debug.LogError("No existe: " + EscenaMapa); return false; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;
        EditorSceneManager.OpenScene(EscenaMapa);
        return true;
    }

    static SpriteRenderer FondoSpriteRenderer()
    {
        var go = GameObject.Find("fondo");
        return go != null ? go.GetComponent<SpriteRenderer>() : null;
    }

    static void DestruirPorNombre(string nombre)
    {
        var toDestroy = new List<GameObject>();
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            if (go.name == nombre)
                toDestroy.Add(go);
        }
        foreach (var go in toDestroy) Object.DestroyImmediate(go);
    }

    // Entrada única para batch mode (Unity -batchmode -quit -executeMethod SetupCertificadoBotones.EjecutarBatch)
    public static void EjecutarBatch()
    {
        RevertirBotonImprimir();
        RepararMapamundialPostPreview();
        Debug.Log("[SetupCertificadoBotones] EjecutarBatch completado");
    }
}
