// =============================================================
// Views/ObjetoColetavelView.cs
// Animação de idle (flutuação + rotação) e efeito de coleta
// Padrão MVC — camada View
// =============================================================

using UnityEngine;

namespace MeuAmbienteVR.Views
{
    public class ObjetoColetavelView : MonoBehaviour
    {
        [Header("Animação Idle")]
        [SerializeField] private float velocidadeRotacao = 90f;
        [SerializeField] private float amplitudeFlutua   = 0.15f;
        [SerializeField] private float velocidadeFlutua  = 1.5f;

        [Header("Efeito de Coleta")]
        [SerializeField] private AudioClip  somColeta;
        [SerializeField] private GameObject particulasColeta; // prefab opcional

        private AudioSource audioSource;
        private Vector3     posicaoInicial;
        private bool        animando = true;

        private void Awake()
        {
            audioSource   = GetComponent<AudioSource>();
            posicaoInicial= transform.position;
        }

        private void Update()
        {
            if (!animando) return;

            // Rotação no eixo Y — indica que o objeto é interativo
            transform.Rotate(Vector3.up, velocidadeRotacao * Time.deltaTime);

            // Flutuação senoidal
            float y = posicaoInicial.y + Mathf.Sin(Time.time * velocidadeFlutua) * amplitudeFlutua;
            transform.position = new Vector3(posicaoInicial.x, y, posicaoInicial.z);
        }

        // Acionado pelo Controller ao confirmar a coleta
        public void ExecutarAnimacaoColeta()
        {
            animando = false;

            if (particulasColeta != null)
                Instantiate(particulasColeta, transform.position, Quaternion.identity);

            float atraso = 0f;
            if (audioSource != null && somColeta != null)
            {
                audioSource.PlayOneShot(somColeta);
                atraso = somColeta.length;
            }

            Invoke(nameof(Desativar), atraso);
        }

        private void Desativar() => gameObject.SetActive(false);
    }
}
