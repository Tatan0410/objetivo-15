using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelBalasHUD : MonoBehaviour
{
    public KeyCode teclaAnterior = KeyCode.Q;
    public KeyCode teclaSiguiente = KeyCode.E;

    [Header("Contadores por tipo")]
    public ContadorHUD contadorComun;
    public ContadorHUD contadorRara;
    public ContadorHUD contadorEpica;

    [Header("Tipo seleccionado")]
    public TMP_Text textoSeleccionado;
    public Image iconoSeleccionado;
    public Sprite spriteComun;
    public Sprite spriteRara;
    public Sprite spriteEpica;

    [Header("Boton de cambio")]
    public Button botonCambiar;

    private bool suscrito = false;

    void Start()
    {
        Suscribirse();

        if (botonCambiar != null)
            botonCambiar.onClick.AddListener(CambiarSiguiente);
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaAnterior))
            CambiarAnterior();
        if (Input.GetKeyDown(teclaSiguiente))
            CambiarSiguiente();
    }

    void OnEnable()
    {
        Suscribirse();
    }

    void OnDisable()
    {
        if (MunicionManager.instancia != null && suscrito)
        {
            MunicionManager.instancia.OnCambio -= Refrescar;
            suscrito = false;
        }
    }

    void OnDestroy()
    {
        if (MunicionManager.instancia != null && suscrito)
        {
            MunicionManager.instancia.OnCambio -= Refrescar;
            suscrito = false;
        }
    }

    void Suscribirse()
    {
        if (suscrito || MunicionManager.instancia == null) return;
        MunicionManager.instancia.OnCambio += Refrescar;
        suscrito = true;
        Refrescar();
    }

    void CambiarAnterior()
    {
        Cambiar(-1);
    }

    void CambiarSiguiente()
    {
        Cambiar(1);
    }

    void Cambiar(int dir)
    {
        MunicionManager m = MunicionManager.instancia;
        if (m == null) return;

        int idx = ((int)m.tipoSeleccionado + dir + 3) % 3;
        m.EstablecerTipo((TipoBala)idx);
    }

    void Refrescar()
    {
        MunicionManager m = MunicionManager.instancia;
        if (m == null) return;

        if (contadorComun != null) contadorComun.Actualizar(m.balasComunes);
        if (contadorRara != null) contadorRara.Actualizar(m.balasRaras);
        if (contadorEpica != null) contadorEpica.Actualizar(m.balasEpicas);

        if (textoSeleccionado != null)
            textoSeleccionado.text = NombreTipo(m.tipoSeleccionado);

        if (iconoSeleccionado != null)
            iconoSeleccionado.sprite = SpriteDe(m.tipoSeleccionado);
    }

    string NombreTipo(TipoBala tipo)
    {
        switch (tipo)
        {
            case TipoBala.Rara: return "RARA";
            case TipoBala.Epica: return "EPICA";
            default: return "COMUN";
        }
    }

    Sprite SpriteDe(TipoBala tipo)
    {
        switch (tipo)
        {
            case TipoBala.Rara: return spriteRara;
            case TipoBala.Epica: return spriteEpica;
            default: return spriteComun;
        }
    }
}
