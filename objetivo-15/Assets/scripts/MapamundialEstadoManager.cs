using UnityEngine;
using UnityEngine.UI;

public class MapamundialEstadoManager : MonoBehaviour
{
    [Header("Fondo del mapa")]
    public Image imagenFondo;

    [Header("Botones (asignados por Setup)")]
    public GameObject botonRejugar;
    public GameObject botonMenuPrincipal;
    public GameObject contenedorNiveles;

    private Sprite spriteContaminado;
    private SpriteRenderer srFondo;

    void OnEnable() => AplicarMapa();
    void Start() => AplicarMapa();

    public void AplicarMapa()
    {
        int nivel = PlayerPrefs.GetInt("NivelDesbloqueado", 0);
        bool limpio = nivel >= 6;
        Debug.Log($"[MapamundialEstado] NivelDesbloqueado={nivel} limpio={limpio} escena={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");

        // Buscar fondo si no está asignado (SpriteRenderer o Image)
        if (imagenFondo == null && srFondo == null)
        {
            var goFondo = GameObject.Find("fondo");
            if (goFondo != null)
            {
                srFondo = goFondo.GetComponent<SpriteRenderer>();
                imagenFondo = goFondo.GetComponent<Image>();
                if (srFondo == null && imagenFondo == null)
                    BuscarFondoFallback();
            }
            else
                BuscarFondoFallback();
        }
        if (imagenFondo == null && srFondo == null)
        {
            Debug.LogWarning("[MapamundialEstado] No se encontró fondo (SpriteRenderer/Image)");
            return;
        }

        if (srFondo != null)
        {
            if (spriteContaminado == null) spriteContaminado = srFondo.sprite;
            if (limpio)
            {
                Sprite limpioSprite = Resources.Load<Sprite>("mapamundiallimpio");
                Debug.Log($"[MapamundialEstado] Cargando mapamundiallimpio SpriteRenderer: {(limpioSprite != null ? limpioSprite.name : "NULL")}");
                if (limpioSprite != null) srFondo.sprite = limpioSprite;
            }
            else if (spriteContaminado != null) srFondo.sprite = spriteContaminado;

            var selectorSR = FindFirstObjectByType<SelectorNivelesMapa>();
            if (selectorSR != null && selectorSR.nodos != null)
                foreach (var nodo in selectorSR.nodos) if (nodo.boton != null) nodo.boton.gameObject.SetActive(!limpio);
            if (contenedorNiveles != null) contenedorNiveles.SetActive(!limpio);
            if (botonRejugar != null) botonRejugar.SetActive(true);
            if (botonMenuPrincipal != null) botonMenuPrincipal.SetActive(true);
            if (limpio && botonRejugar != null) SeleccionUI.SeleccionarPrimero(botonRejugar.transform.parent != null ? botonRejugar.transform.parent.gameObject : botonRejugar);
            return;
        }

        if (spriteContaminado == null) spriteContaminado = imagenFondo.sprite;
        if (limpio)
        {
            Sprite limpioSprite = Resources.Load<Sprite>("mapamundiallimpio");
            Debug.Log($"[MapamundialEstado] Cargando mapamundiallimpio Image: {(limpioSprite != null ? limpioSprite.name : "NULL")}");
            if (limpioSprite != null) imagenFondo.sprite = limpioSprite;
        }
        else if (spriteContaminado != null) imagenFondo.sprite = spriteContaminado;

        // Ocultar/mostrar niveles y botones según estado limpio
        var selector = FindFirstObjectByType<SelectorNivelesMapa>();
        if (selector != null && selector.nodos != null)
        {
            foreach (var nodo in selector.nodos)
                if (nodo.boton != null) nodo.boton.gameObject.SetActive(!limpio);
        }
        if (contenedorNiveles != null) contenedorNiveles.SetActive(!limpio);
        if (botonRejugar != null) botonRejugar.SetActive(true);
        if (botonMenuPrincipal != null) botonMenuPrincipal.SetActive(true);
        if (limpio && botonRejugar != null)
            SeleccionUI.SeleccionarPrimero(botonRejugar.transform.parent != null ? botonRejugar.transform.parent.gameObject : botonRejugar);
    }

    void BuscarFondoFallback()
    {
        // Fallback por si GameObject.Find falla (nombre distinto)
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        Image mejor = null;
        float maxArea = 0f;
        foreach (var img in canvas.GetComponentsInChildren<Image>(true))
        {
            if (img.gameObject.name.ToLower().Contains("panel") || img.gameObject.name.ToLower().Contains("confirmacion")) continue;
            var rt = img.GetComponent<RectTransform>();
            if (rt == null) continue;
            float area = rt.rect.width * rt.rect.height;
            if (area <= 1f) area = rt.sizeDelta.x * rt.sizeDelta.y;
            if (area > maxArea) { maxArea = area; mejor = img; }
        }
        imagenFondo = mejor;
        if (mejor != null) Debug.Log($"[MapamundialEstado] Fallback Image fondo: {mejor.gameObject.name}");
    }
}
