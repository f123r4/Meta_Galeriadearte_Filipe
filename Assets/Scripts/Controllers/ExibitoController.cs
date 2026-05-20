using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Controla a lógica de interação de uma obra de arte no museu virtual.
// Suporta tanto XRSimpleInteractable quanto detecção por proximidade.
public class ExibitoController : MonoBehaviour
{
    [Header("Dados da Obra")]
    public string titulo    = "Obra Sem Título";
    public string artista   = "Artista Desconhecido";
    public int    ano       = 2025;
    [TextArea] public string descricao = "Descrição da obra de arte.";
    public int    pontosInteracao = 15;

    [Header("Referências")]
    [SerializeField] private PainelInfoController painelInfo;

    private ExibitoModel modelo;
    private ExibitoView   view;

    private void Awake()
    {
        view   = GetComponentInChildren<ExibitoView>(true);
        modelo = new ExibitoModel(titulo, descricao, artista, ano);
    }

    // Ativa o exibito — chame de qualquer origem (XR, proximidade, teclado)
    public void Ativar()
    {
        if (modelo.foiInteragido) return;
        modelo.foiInteragido = true;

        view?.AtivarDestaque();

        if (painelInfo != null)
            painelInfo.ExibirInfo(modelo.FormatarInfo());

        GaleriaManager.Instance?.RegistrarInteracaoExibito(titulo, pontosInteracao);
    }

    // Hover entrou — ativa pontos + painel na primeira visita, destaque visual sempre
    public void AoEntrarHover()
    {
        view?.SetHover(true);
        bool primeiraVez = !modelo.foiInteragido;
        Ativar(); // só executa de verdade na primeira vez (foiInteragido guarda o estado)
        string msg = primeiraVez
            ? $"Obra: {titulo} — +{pontosInteracao} pts!"
            : $"Obra: {titulo} — Já visitada";
        GaleriaManager.Instance?.ExibirMensagem(msg);
    }

    public void AoSairHover()
    {
        view?.SetHover(false);
        GaleriaManager.Instance?.ExibirMensagem("Explore o MetaMuseu!");
    }

    // ---- Eventos XR ----
    public void OnXRSelectEntered(SelectEnterEventArgs _) => Ativar();
    public void OnXRHoverEntered(HoverEnterEventArgs _)   => AoEntrarHover();
    public void OnXRHoverExited(HoverExitEventArgs _)     => AoSairHover();
}
