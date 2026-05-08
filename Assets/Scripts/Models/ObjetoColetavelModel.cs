// =============================================================
// Models/ObjetoColetavelModel.cs
// Dados de um objeto coletável — sem dependência de MonoBehaviour
// Padrão MVC — camada Model
// =============================================================

using System;

namespace MeuAmbienteVR.Models
{
    [Serializable]
    public class ObjetoColetavelModel
    {
        public string Id      { get; private set; }
        public string Nome    { get; set; }
        public int    Pontos  { get; set; }
        public bool   Coletado{ get; private set; }

        public ObjetoColetavelModel(string id, string nome, int pontos)
        {
            Id      = id;
            Nome    = nome;
            Pontos  = pontos;
            Coletado= false;
        }

        // Marca como coletado — irreversível durante a sessão
        public void Coletar()  => Coletado = true;

        // Reseta para testes ou reinício de cena
        public void Resetar()  => Coletado = false;
    }
}
