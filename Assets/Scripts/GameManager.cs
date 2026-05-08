// =============================================================
// GameManager.cs
// Controlador central do jogo — padrão Singleton
// Coordena Models, aciona Views e expõe API para os Controllers
// Padrão MVC — ponto de entrada da lógica de jogo
// =============================================================

using UnityEngine;
using MeuAmbienteVR.Models;
using MeuAmbienteVR.Views;

namespace MeuAmbienteVR
{
    public class GameManager : MonoBehaviour
    {
        // Acesso global — uma única instância por sessão
        public static GameManager Instancia { get; private set; }

        [Header("Referências de View")]
        [SerializeField] private HUDView hudView;

        // Eventos C# — outros scripts se inscrevem sem depender do GameManager
        public event System.Action<int> OnPontuacaoAtualizada;
        public event System.Action      OnJogoFinalizado;

        // Models gerenciados centralmente
        private AmbienteModel ambienteModel;
        private JogadorModel  jogadorModel;

        // ── Ciclo de vida ────────────────────────────────────────

        private void Awake()
        {
            // Destrói duplicata gerada em reload de cena
            if (Instancia != null && Instancia != this)
            {
                Destroy(gameObject);
                return;
            }
            Instancia = this;
            DontDestroyOnLoad(gameObject);

            InicializarModels();
        }

        private void Start()
        {
            ambienteModel.IniciarJogo();
            AtualizarHUD();
            Debug.Log("[GameManager] Sessão iniciada: " + ambienteModel.NomeAmbiente);
        }

        // ── Inicialização ────────────────────────────────────────

        private void InicializarModels()
        {
            ambienteModel = new AmbienteModel("Meu Ambiente VR");
            jogadorModel  = new JogadorModel("Jogador");
        }

        // ── API pública para Controllers ─────────────────────────

        // Registra um objeto coletável recém-criado na cena
        public void RegistrarObjetoColetavel(ObjetoColetavelModel modelo)
        {
            ambienteModel.AdicionarObjeto(modelo);
            AtualizarHUD();
        }

        // Processa a coleta de um objeto e atualiza o estado do jogo
        public void RegistrarColeta(ObjetoColetavelModel objeto)
        {
            if (objeto == null || objeto.Coletado) return;

            objeto.Coletar();
            jogadorModel.AdicionarPontos(objeto.Pontos);

            hudView?.ExibirMensagemTemporaria($"+{objeto.Pontos} pts — {objeto.Nome}!");
            AtualizarHUD();
            OnPontuacaoAtualizada?.Invoke(jogadorModel.Pontuacao);

            if (ambienteModel.TodosColetados())
                EncerrarJogo();
        }

        // Leitura somente — evita que outros scripts modifiquem os Models diretamente
        public JogadorModel  ObterJogador()   => jogadorModel;
        public AmbienteModel ObterAmbiente()  => ambienteModel;

        // ── Lógica interna ───────────────────────────────────────

        private void AtualizarHUD()
        {
            if (hudView == null) return;
            hudView.AtualizarPontuacao(jogadorModel.Pontuacao);
            hudView.AtualizarContadorObjetos(
                ambienteModel.TotalColetados(),
                ambienteModel.ObjetosColetaveis.Count
            );
        }

        private void EncerrarJogo()
        {
            ambienteModel.FinalizarJogo();
            hudView?.ExibirMensagem("Parabéns! Todos os objetos coletados!");
            OnJogoFinalizado?.Invoke();
            Debug.Log($"[GameManager] Jogo encerrado. Pontuação: {jogadorModel.Pontuacao}");
        }
    }
}
