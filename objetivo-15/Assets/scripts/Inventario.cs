using UnityEngine;

public class Inventario : MonoBehaviour
{
    public static Inventario instancia;

    [Header("Cantidades")]
    public int botellasPET = 0;
    public int bolsasPlasticas = 0;
    public int icopor = 0;
    public int bananas = 0;
    public int manzanas = 0;

    [Header("UI - Contadores")]
    public ContadorHUD contadorPET;
    public ContadorHUD contadorBolsa;
    public ContadorHUD contadorIcopor;
    public ContadorHUD contadorBanana;
    public ContadorHUD contadorManzana;

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
            case TipoPlastico.Icopor:
                icopor++;
                break;
            case TipoPlastico.Banana:
                bananas++;
                break;
            case TipoPlastico.Manzana:
                manzanas++;
                break;
        }

        ActualizarUI();
        Debug.Log("Recogiste: " + tipo.ToString());

        if (EstadisticasManager.instancia != null)
            EstadisticasManager.instancia.RegistrarPlastico();
    }

    public bool TieneIngredientes(int pet, int bolsas, int ico)
    {
        return botellasPET >= pet &&
               bolsasPlasticas >= bolsas &&
               icopor >= ico;
    }

    public void GastarIngredientes(int pet, int bolsas, int ico)
    {
        botellasPET -= pet;
        bolsasPlasticas -= bolsas;
        icopor -= ico;
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (contadorPET != null) contadorPET.Actualizar(botellasPET);
        if (contadorBolsa != null) contadorBolsa.Actualizar(bolsasPlasticas);
        if (contadorIcopor != null) contadorIcopor.Actualizar(icopor);
        if (contadorBanana != null) contadorBanana.Actualizar(bananas);
        if (contadorManzana != null) contadorManzana.Actualizar(manzanas);
    }
}
