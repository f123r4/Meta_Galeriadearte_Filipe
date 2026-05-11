using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BotaoPrincipalController : MonoBehaviour
{
    private BotaoPrincipalView view;

    void Awake()
    {
        view = GetComponent<BotaoPrincipalView>();
    }

    public void AoEntrarHoverProximidade()
    {
        view?.SetHover(true);
        var gm = FindFirstObjectByType<GameManager>();
        gm?.hudView?.ExibirMensagem("Pressione E para ativar!");
        Debug.Log("[Botao] Hover enter");
    }

    public void AoSairHoverProximidade()
    {
        view?.SetHover(false);
        var gm = FindFirstObjectByType<GameManager>();
        gm?.hudView?.ExibirMensagem("Colete todos os objetos!");
        Debug.Log("[Botao] Hover exit");
    }

    public void AoPressionarProximidade()
    {
        view?.SetAtivo();
        var gm = FindFirstObjectByType<GameManager>();
        gm?.hudView?.ExibirMensagem("Botao ativado!");
        Debug.Log("[Botao] Pressionado!");
    }

    public void AoEntrarHover(HoverEnterEventArgs args) => AoEntrarHoverProximidade();
    public void AoSairHover(HoverExitEventArgs args)    => AoSairHoverProximidade();
    public void AoPressionar(SelectEnterEventArgs args) => AoPressionarProximidade();
}
