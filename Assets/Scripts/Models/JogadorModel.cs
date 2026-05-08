// =============================================================
// Models/JogadorModel.cs
// Dados e estado do jogador — sem dependência de MonoBehaviour
// Padrão MVC — camada Model
// =============================================================

using System;

namespace MeuAmbienteVR.Models
{
    [Serializable]
    public class JogadorModel
    {
        public string Nome       { get; set; }
        public int    Pontuacao  { get; private set; }
        public bool   EmMovimento{ get; private set; }

        public JogadorModel(string nome)
        {
            Nome       = nome;
            Pontuacao  = 0;
            EmMovimento= false;
        }

        public void AdicionarPontos(int pontos)
        {
            if (pontos > 0) Pontuacao += pontos;
        }

        public void SetMovimento(bool valor) => EmMovimento = valor;

        public void Reiniciar()
        {
            Pontuacao   = 0;
            EmMovimento = false;
        }
    }
}
