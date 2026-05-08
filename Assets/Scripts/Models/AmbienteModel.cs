// =============================================================
// Models/AmbienteModel.cs
// Estado geral do ambiente VR — lista de objetos e fase do jogo
// Padrão MVC — camada Model
// =============================================================

using System;
using System.Collections.Generic;

namespace MeuAmbienteVR.Models
{
    [Serializable]
    public class AmbienteModel
    {
        public string NomeAmbiente  { get; set; }
        public bool   JogoAtivo     { get; private set; }
        public bool   JogoFinalizado{ get; private set; }

        public List<ObjetoColetavelModel> ObjetosColetaveis { get; private set; }

        public AmbienteModel(string nome)
        {
            NomeAmbiente       = nome;
            ObjetosColetaveis  = new List<ObjetoColetavelModel>();
            JogoAtivo          = false;
            JogoFinalizado     = false;
        }

        public void AdicionarObjeto(ObjetoColetavelModel obj)
        {
            if (obj != null && !ObjetosColetaveis.Contains(obj))
                ObjetosColetaveis.Add(obj);
        }

        public void IniciarJogo()
        {
            JogoAtivo      = true;
            JogoFinalizado = false;
        }

        public void FinalizarJogo()
        {
            JogoAtivo      = false;
            JogoFinalizado = true;
        }

        public int TotalColetados()
        {
            int n = 0;
            foreach (var obj in ObjetosColetaveis)
                if (obj.Coletado) n++;
            return n;
        }

        public bool TodosColetados() =>
            ObjetosColetaveis.Count > 0 && TotalColetados() == ObjetosColetaveis.Count;
    }
}
