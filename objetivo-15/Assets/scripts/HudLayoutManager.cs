using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Gestiona el layout (posicion + color) de los botones tactiles del HUD.
// Se agrega al prefab TouchControls, por lo que existe en cada nivel y aplica
// la configuracion guardada del jugador al cargar la escena.
public class HudLayoutManager : MonoBehaviour
{
    public static HudLayoutManager instancia;

    public const float PASO_MOVIMIENTO = 10f;

    [System.Serializable]
    public class Elemento
    {
        public string id;
        public RectTransform rt;
        public Image imagen;
        public TextMeshProUGUI etiqueta;

        public Vector2 posicionPorDefecto;
        public Color colorPorDefecto;
        public Color colorEtiquetaPorDefecto;
    }

    public List<Elemento> elementos = new List<Elemento>();

    static readonly string[] IDS =
    {
        "Dpad", "Botones",
        "BtnIzquierda", "BtnDerecha", "BtnSaltar", "BtnAtaque",
        "BtnDisparo", "BtnCambioBala"
    };

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        DescubrirElementos();
        AplicarGuardado();
    }

    void OnDestroy()
    {
        if (instancia == this) instancia = null;
    }

    void DescubrirElementos()
    {
        elementos.Clear();
        foreach (string id in IDS)
        {
            Transform t = BuscarPorNombre(id);
            if (t == null) continue;

            var e = new Elemento();
            e.id = id;
            e.rt = t as RectTransform;
            e.imagen = t.GetComponent<Image>();

            Transform etq = t.Find("Etiqueta");
            if (etq != null) e.etiqueta = etq.GetComponent<TextMeshProUGUI>();

            if (e.rt != null) e.posicionPorDefecto = e.rt.anchoredPosition;
            e.colorPorDefecto = e.imagen != null ? e.imagen.color : Color.white;
            e.colorEtiquetaPorDefecto = e.etiqueta != null ? e.etiqueta.color : Color.white;

            elementos.Add(e);
        }
    }

    Transform BuscarPorNombre(string nombre)
    {
        foreach (Transform t in transform)
        {
            if (t.name == nombre) return t;
            Transform r = BuscarRecursivo(t, nombre);
            if (r != null) return r;
        }
        return null;
    }

    Transform BuscarRecursivo(Transform padre, string nombre)
    {
        foreach (Transform c in padre)
        {
            if (c.name == nombre) return c;
            Transform r = BuscarRecursivo(c, nombre);
            if (r != null) return r;
        }
        return null;
    }

    public Elemento Obtener(string id)
    {
        return elementos.Find(e => e.id == id);
    }

    public bool ExisteElemento(string id)
    {
        return Obtener(id) != null;
    }

    // ── Operaciones que el panel editor llama ──────────────────────────────

    public void Mover(string id, Vector2 delta)
    {
        Elemento e = Obtener(id);
        if (e == null || e.rt == null) return;
        e.rt.anchoredPosition += delta;
        Guardar();
    }

    public void SetColor(string id, Color color)
    {
        Elemento e = Obtener(id);
        if (e == null) return;
        if (e.imagen != null) e.imagen.color = color;
        if (e.etiqueta != null) e.etiqueta.color = ColorContraste(color);
        Debug.Log($"SetColor applied to {id} with color {color}");

        // Los grupos (Dpad/Botones) no tienen Image propio; se colorean sus
        // botones tactiles descendientes para que el color siempre se vea.
        if (e.rt != null)
            AplicarColorRecursivo(e.rt, color);

        Guardar();
    }

    void AplicarColorRecursivo(Transform t, Color color)
    {
        if (t.GetComponent<TouchButton>() != null)
        {
            Image img = t.GetComponent<Image>();
            if (img != null) img.color = color;

            Transform etq = t.Find("Etiqueta");
            if (etq != null)
            {
                TextMeshProUGUI tmp = etq.GetComponent<TextMeshProUGUI>();
                if (tmp != null) tmp.color = ColorContraste(color);
            }
        }

        foreach (Transform c in t)
            AplicarColorRecursivo(c, color);
    }

    public void RestablecerTodo()
    {
        foreach (Elemento e in elementos)
        {
            if (e.rt != null) e.rt.anchoredPosition = e.posicionPorDefecto;
            if (e.imagen != null) e.imagen.color = e.colorPorDefecto;
            if (e.etiqueta != null) e.etiqueta.color = e.colorEtiquetaPorDefecto;

            if (e.rt != null)
                AplicarColorRecursivo(e.rt, e.colorPorDefecto);
        }
        Guardar();
    }

    public void RestablecerElemento(string id)
    {
        Elemento e = Obtener(id);
        if (e == null) return;
        if (e.rt != null) e.rt.anchoredPosition = e.posicionPorDefecto;
        if (e.imagen != null) e.imagen.color = e.colorPorDefecto;
        if (e.etiqueta != null) e.etiqueta.color = e.colorEtiquetaPorDefecto;

        if (e.rt != null)
            AplicarColorRecursivo(e.rt, e.colorPorDefecto);

        Guardar();
    }

    // ── Persistencia ───────────────────────────────────────────────────────

    public void Guardar()
    {
        var datos = new HudElementoData[elementos.Count];
        for (int i = 0; i < elementos.Count; i++)
        {
            Elemento e = elementos[i];
            datos[i] = new HudElementoData
            {
                id = e.id,
                posicion = e.rt != null ? e.rt.anchoredPosition : Vector2.zero,
                color = e.imagen != null ? e.imagen.color : Color.white
            };
        }
        HudConfig.Guardar(datos);
    }

    public void AplicarGuardado()
    {
        HudElementoData[] datos = HudConfig.Cargar();
        if (datos == null) return;

        foreach (HudElementoData d in datos)
        {
            Elemento e = Obtener(d.id);
            if (e == null) continue;
            if (e.rt != null) e.rt.anchoredPosition = d.posicion;
            if (e.imagen != null)
            {
                e.imagen.color = d.color;
                if (e.etiqueta != null) e.etiqueta.color = ColorContraste(d.color);
            }
        }
    }

    // Texto legible sobre cualquier fondo.
    static Color ColorContraste(Color fondo)
    {
        float lum = 0.2126f * fondo.r + 0.7152f * fondo.g + 0.0722f * fondo.b;
        return lum > 0.5f ? new Color(0.08f, 0.08f, 0.08f, 1f) : Color.white;
    }
}