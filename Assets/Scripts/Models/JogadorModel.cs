// Estado do jogador durante a sessão VR
[System.Serializable]
public class JogadorModel
{
    public int pontuacao;
    public int exibitosVisitados;
    public float tempoSessao;

    public void AdicionarPontos(int valor) => pontuacao += valor;
    public void RegistrarVisita()          => exibitosVisitados++;
}
