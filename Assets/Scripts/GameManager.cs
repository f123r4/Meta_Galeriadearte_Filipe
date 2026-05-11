using UnityEngine;

// Gerencia o estado global do jogo: pontuação e progresso de coleta
public class GameManager : MonoBehaviour
{
    [SerializeField] public HUDView hudView;

    private int pontuacao = 0;
    private int totalObjetos = 3;
    private int objetosColetados = 0;

    private void Start()
    {
        // Inicializa o HUD com valores zerados
        if (hudView != null)
        {
            hudView.AtualizarPontuacao(pontuacao);
            hudView.AtualizarObjetos(objetosColetados, totalObjetos);
            hudView.ExibirMensagem("Colete todos os objetos!");
        }
    }

    // Chamado por ObjetoColetavelController quando um objeto é coletado
    public void RegistrarColeta(int pontos)
    {
        pontuacao += pontos;
        objetosColetados++;

        if (hudView != null)
        {
            hudView.AtualizarPontuacao(pontuacao);
            hudView.AtualizarObjetos(objetosColetados, totalObjetos);

            if (objetosColetados >= totalObjetos)
                hudView.ExibirMensagem("Parabéns! Todos os objetos coletados!");
            else
                hudView.ExibirMensagem("Objeto coletado! +" + pontos + " pontos");
        }
    }
}
