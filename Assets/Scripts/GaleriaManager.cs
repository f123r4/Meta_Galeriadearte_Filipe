using UnityEngine;

public class GaleriaManager : MonoBehaviour
{
    public static GaleriaManager Instance { get; private set; }

    public HUDView hudView;

    private JogadorModel jogador      = new JogadorModel();
    private int          totalExibitos = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        totalExibitos = Object.FindObjectsByType<ExibitoController>(FindObjectsSortMode.None).Length;
        AtualizarHUD("Valide seu ingresso para entrar na galeria.");
    }

    private void Update()
    {
        jogador.tempoSessao += Time.deltaTime;
    }

    public void RegistrarIngressoAprovado()
    {
        AtualizarHUD("Ingresso aprovado! Bem-vindo à Galeria de Arte!");
    }

    public void RegistrarInteracaoExibito(string tituloObra, int pontos)
    {
        jogador.AdicionarPontos(pontos);
        jogador.RegistrarVisita();

        string msg = jogador.exibitosVisitados >= totalExibitos
            ? "Parabéns! Você apreciou todas as obras!"
            : $"+{pontos} pts — {tituloObra}";

        AtualizarHUD(msg);
    }

    public void ExibirMensagem(string msg) => AtualizarHUD(msg);

    private void AtualizarHUD(string mensagem)
    {
        if (hudView == null) return;
        hudView.AtualizarPontuacao(jogador.pontuacao);
        hudView.ExibirMensagem(mensagem);
    }
}
