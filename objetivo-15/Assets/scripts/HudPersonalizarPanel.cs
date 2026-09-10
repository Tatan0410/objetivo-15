using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Panel que se abre desde el menu de pausa para editar posicion y color de
// los botones tactiles. Cada cambio se guarda al instante via HudLayoutManager.
public class HudPersonalizarPanel : MonoBehaviour
{
    [Header("Gestor del HUD")]
    public HudLayoutManager hud;

    [Header("Selector de elemento")]
    public Button btnPrev;
    public Button btnNext;
    public TMP_Text textoElemento;

    [Header("Movimiento")]
    public Button btnArriba;
    public Button btnAbajo;
    public Button btnIzquierda;
    public Button btnDerecha;

    [Header("Acciones")]
    public Button btnRestablecer;
    public Button btnCerrar;

    int indice;

    void Start()
    {
        if (hud == null) hud = HudLayoutManager.instancia;
        if (hud == null) hud = FindFirstObjectByType<HudLayoutManager>();

        indice = 0;
        RefrescarTexto();
        ConectarSwatches();
    }

    // Conecta los botones de color (BtnColor0/1/2) a su propio color en
    // tiempo de ejecucion. Evita depender de referencias serializadas en el
    // prefab (que es donde se perdia la conexion onClic).
    void ConectarSwatches()
    {
        foreach (var btn in GetComponentsInChildren<Button>(true))
        {
            if (btn == null) continue;
            if (!btn.name.ToLowerInvariant().Contains("color")) continue;

            var img = btn.GetComponent<Image>();
            if (img == null) continue;

            Color c = img.color;
            btn.onClick.AddListener(() => AplicarColor(c));
        }
    }

    string IdActual()
    {
        if (hud == null || hud.elementos.Count == 0) return null;
        return hud.elementos[indice].id;
    }

    public void SeleccionarAnterior()
    {
        if (hud == null || hud.elementos.Count == 0) return;
        indice = (indice - 1 + hud.elementos.Count) % hud.elementos.Count;
        RefrescarTexto();
    }

    public void SeleccionarSiguiente()
    {
        if (hud == null || hud.elementos.Count == 0) return;
        indice = (indice + 1) % hud.elementos.Count;
        RefrescarTexto();
    }

    void RefrescarTexto()
    {
        if (textoElemento != null)
            textoElemento.text = IdActual() ?? "---";
    }

    // ── Movimiento ─────────────────────────────────────────────────────────

    public void MoverArriba()
    {
        var id = IdActual();
        if (id != null) hud.Mover(id, Vector2.up * HudLayoutManager.PASO_MOVIMIENTO);
    }

    public void MoverAbajo()
    {
        var id = IdActual();
        if (id != null) hud.Mover(id, Vector2.down * HudLayoutManager.PASO_MOVIMIENTO);
    }

    public void MoverIzquierda()
    {
        var id = IdActual();
        if (id != null) hud.Mover(id, Vector2.left * HudLayoutManager.PASO_MOVIMIENTO);
    }

    public void MoverDerecha()
    {
        var id = IdActual();
        if (id != null) hud.Mover(id, Vector2.right * HudLayoutManager.PASO_MOVIMIENTO);
    }

    // ── Color ──────────────────────────────────────────────────────────────

    public void AplicarColor(Color color)
    {
        var id = IdActual();
        if (id != null) hud.SetColor(id, color);
    }

    // ── Acciones ───────────────────────────────────────────────────────────

    public void Restablecer()
    {
        if (hud != null) hud.RestablecerTodo();
    }

    public void Cerrar()
    {
        gameObject.SetActive(false);
        if (MenuPausa.instancia != null && MenuPausa.instancia.panelPausa != null)
        {
            MenuPausa.instancia.panelPausa.SetActive(true);
            SeleccionUI.SeleccionarPrimero(MenuPausa.instancia.panelPausa);
        }
        else
        {
            SeleccionUI.LimpiarSeleccion();
        }
    }
}

// Se agrega a cada boton de la paleta de colores. Al hacer click lee su propio
// color (el swatch) y lo aplica al elemento seleccionado del HUD.
public class HudColorBoton : MonoBehaviour
{
    public HudPersonalizarPanel panel;

    public void Click()
    {
        var img = GetComponent<Image>();
        if (img != null && panel != null)
            panel.AplicarColor(img.color);
    }
}