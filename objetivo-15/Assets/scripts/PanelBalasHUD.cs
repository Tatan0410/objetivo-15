using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelBalasHUD : MonoBehaviour
{
    public KeyCode teclaAnterior = KeyCode.Q;
    public KeyCode teclaSiguiente = KeyCode.E;

    [Header("Contadores por tipo")]
    public ContadorHUD contadorComun;
    public ContadorHUD contadorEpica;
    public ContadorHUD contadorLegendaria;

    [Header("Tipo seleccionado")]
    public TMP_Text textoSeleccionado;
    public Image iconoSeleccionado;
    public Sprite spriteComun;
    public Sprite spriteEpica;
    public Sprite spriteLegendaria;

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
            MunicionManager.instancia?.EstablecerTipo(TipoBala.Comun);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            MunicionManager.instancia?.EstablecerTipo(TipoBala.Epica);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            MunicionManager.instancia?.EstablecerTipo(TipoBala.Legendaria);

        if (Input.GetKeyDown(teclaAnterior) || Input.GetButtonDown("CycleLeft"))
            CambiarAnterior();
        if (Input.GetKeyDown(teclaSiguiente) || Input.GetButtonDown("CycleRight"))
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
        if (contadorEpica != null) contadorEpica.Actualizar(m.balasEpicas);
        if (contadorLegendaria != null) contadorLegendaria.Actualizar(m.balasLegendarias);

        if (textoSeleccionado != null)
            textoSeleccionado.text = NombreTipo(m.tipoSeleccionado);

        if (iconoSeleccionado != null)
            iconoSeleccionado.sprite = SpriteDe(m.tipoSeleccionado);
    }

    string NombreTipo(TipoBala tipo)
    {
        switch (tipo)
        {
            case TipoBala.Epica: return "EPICA";
            case TipoBala.Legendaria: return "LEGENDARIA";
            default: return "COMUN";
        }
    }

    Sprite SpriteDe(TipoBala tipo)
    {
        switch (tipo)
        {
            case TipoBala.Epica: return spriteEpica;
            case TipoBala.Legendaria: return spriteLegendaria;
            default: return spriteComun;
        }
    }
}
