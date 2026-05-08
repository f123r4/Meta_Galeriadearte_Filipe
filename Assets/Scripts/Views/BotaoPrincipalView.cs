// =============================================================
// Views/BotaoPrincipalView.cs
// Feedback visual e sonoro do botão interativo
// Padrão MVC — camada View
// =============================================================

using UnityEngine;

namespace MeuAmbienteVR.Views
{
    public class BotaoPrincipalView : MonoBehaviour
    {
        [Header("Materiais por Estado")]
        [SerializeField] private Material materialNormal;
        [SerializeField] private Material materialHover;
        [SerializeField] private Material materialPressionado;

        [Header("Escala ao Pressionar")]
        [SerializeField] private float fatorEscala = 0.88f;

        [Header("Áudio")]
        [SerializeField] private AudioClip somClicar;

        private Renderer    botaoRenderer;
        private AudioSource audioSource;
        private Vector3     escalaOriginal;

        private void Awake()
        {
            botaoRenderer = GetComponent<Renderer>();
            audioSource   = GetComponent<AudioSource>();
            escalaOriginal= transform.localScale;
        }

        private void Start() => Aplicar(materialNormal);

        public void MostrarEstadoNormal()
        {
            Aplicar(materialNormal);
            transform.localScale = escalaOriginal;
        }

        public void MostrarEstadoHover()       => Aplicar(materialHover);

        public void MostrarEstadoPressionado()
        {
            Aplicar(materialPressionado);
            transform.localScale = escalaOriginal * fatorEscala;
            if (audioSource && somClicar) audioSource.PlayOneShot(somClicar);
        }

        private void Aplicar(Material mat)
        {
            if (botaoRenderer && mat) botaoRenderer.material = mat;
        }
    }
}
