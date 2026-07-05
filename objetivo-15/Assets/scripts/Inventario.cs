using UnityEngine;

public class Inventario : MonoBehaviour
{
    public static Inventario instancia;

    [Header("Cantidades")]
    public int botellasPET = 0;
    public int bolsasPlasticas = 0;
    public int tarros = 0;
    public int tubosPVC = 0;

    [Header("UI - Contadores")]
    // TODO: reemplazar iconos placeholder con sprites reales
    public ContadorHUD contadorPET;
    public ContadorHUD contadorBolsa;
    public ContadorHUD contadorTarro;
    public ContadorHUD contadorTubo;

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ActualizarUI();
    }

    public void AgregarPlastico(TipoPlastico tipo)
    {
        Debug.Log("AGREGANDO: " + tipo);
        switch (tipo)
        {
            case TipoPlastico.BotellaPET:
                botellasPET++;
                break;
            case TipoPlastico.BolsaPlastica:
                bolsasPlasticas++;
                break;
            case TipoPlastico.Tarro:
                tarros++;
                break;
            case TipoPlastico.TuboPVC:
                tubosPVC++;
                break;
        }

        ActualizarUI();
        Debug.Log("Recogiste: " + tipo.ToString());
    }

    public bool TieneIngredientes(int pet, int bolsas, int t, int tubos)
    {
        return botellasPET >= pet &&
               bolsasPlasticas >= bolsas &&
               tarros >= t &&
               tubosPVC >= tubos;
    }

    public void GastarIngredientes(int pet, int bolsas, int t, int tubos)
    {
        botellasPET -= pet;
        bolsasPlasticas -= bolsas;
        tarros -= t;
        tubosPVC -= tubos;
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (contadorPET != null) contadorPET.Actualizar(botellasPET);
        if (contadorBolsa != null) contadorBolsa.Actualizar(bolsasPlasticas);
        if (contadorTarro != null) contadorTarro.Actualizar(tarros);
        if (contadorTubo != null) contadorTubo.Actualizar(tubosPVC);
    }
}
