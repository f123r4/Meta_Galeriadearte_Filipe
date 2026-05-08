// =============================================================
// Views/HUDView.cs
// Exibe pontuação, contador de objetos e mensagens na tela
// Padrão MVC — camada View
// Dependência: TextMeshPro (incluso no Unity 2022 LTS)
// =============================================================

using System.Collections;
using UnityEngine;
using TMPro;

namespace MeuAmbienteVR.Views
{
    public class HUDView : MonoBehaviour
    {
        [Header("Textos")]
        [SerializeField] private TextMeshProUGUI textoPontuacao;
        [SerializeField] private TextMeshProUGUI textoObjetos;
        [SerializeField] private TextMeshProUGUI textoMensagem;

        [Header("Duração da mensagem temporária (s)")]
        [SerializeField] private float duracaoMensagem = 3f;

        private Coroutine rotinaTemporaria;

        private void Start()
        {
            AtualizarPontuacao(0);
            AtualizarContadorObjetos(0, 0);
            OcultarMensagem();
        }

        public void AtualizarPontuacao(int valor)
        {
            if (textoPontuacao) textoPontuacao.text = $"Pontuação: {valor}";
        }

        public void AtualizarContadorObjetos(int coletados, int total)
        {
            if (textoObjetos) textoObjetos.text = $"Objetos: {coletados}/{total}";
        }

        // Mensagem permanente (ex: tela de vitória)
        public void ExibirMensagem(string texto)
        {
            if (!textoMensagem) return;
            textoMensagem.text = texto;
            textoMensagem.gameObject.SetActive(true);
        }

        // Mensagem que desaparece após duracaoMensagem segundos
        public void ExibirMensagemTemporaria(string texto)
        {
            if (!textoMensagem) return;
            if (rotinaTemporaria != null) StopCoroutine(rotinaTemporaria);
            rotinaTemporaria = StartCoroutine(RotinaMensagem(texto));
        }

        private IEnumerator RotinaMensagem(string texto)
        {
            textoMensagem.text = texto;
            textoMensagem.gameObject.SetActive(true);
            yield return new WaitForSeconds(duracaoMensagem);
            OcultarMensagem();
        }

        private void OcultarMensagem()
        {
            if (textoMensagem) textoMensagem.gameObject.SetActive(false);
        }
    }
}
