using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Validador de ingresso da galeria — fica na ante-sala.
// Quando ativado (tecla E por proximidade ou XR Select), aprova o ingresso e abre a porta.
public class BotaoPrincipalController : MonoBehaviour
{
    [SerializeField] private PortaAnimadaController portaGaleria;

    private BotaoPrincipalView view;
    private bool ingressoAprovado = false;

    private void Awake() => view = GetComponent<BotaoPrincipalView>();

    public void AoEntrarHoverProximidade()
    {
        if (ingressoAprovado) return;
        view?.SetHover(true);
        GaleriaManager.Instance?.ExibirMensagem("Pressione E para validar seu ingresso!");
    }

    public void AoSairHoverProximidade()
    {
        if (ingressoAprovado) return;
        view?.SetHover(false);
        GaleriaManager.Instance?.ExibirMensagem("Valide seu ingresso para entrar na galeria.");
    }

    public void AoPressionarProximidade()
    {
        if (ingressoAprovado) return;
        ingressoAprovado = true;
        view?.SetAtivo();
        portaGaleria?.Alternar();
        GaleriaManager.Instance?.RegistrarIngressoAprovado();
    }

    // Eventos XR
    public void AoEntrarHover(HoverEnterEventArgs _)  => AoEntrarHoverProximidade();
    public void AoSairHover(HoverExitEventArgs _)     => AoSairHoverProximidade();
    public void AoPressionar(SelectEnterEventArgs _)  => AoPressionarProximidade();
}
