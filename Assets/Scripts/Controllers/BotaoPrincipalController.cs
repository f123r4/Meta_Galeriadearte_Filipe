// =============================================================
// Controllers/BotaoPrincipalController.cs
// Lógica do botão interativo na cena VR
//   - Conecta hover/select do XRSimpleInteractable
//   - Fallback OnMouse* para testes no Editor sem headset
// Padrão MVC — camada Controller
// =============================================================

using UnityEngine;
using MeuAmbienteVR.Views;

namespace MeuAmbienteVR.Controllers
{
    [RequireComponent(typeof(Collider))]
    public class BotaoPrincipalController : MonoBehaviour
    {
        [Header("Configurações")]
        [SerializeField] private string rotulo                  = "Botão Principal";
        [SerializeField] private float  intervaloEntreAtivacoes = 1f;

        // Outros scripts se inscrevem sem acoplamento direto
        public event System.Action OnAtivado;

        private BotaoPrincipalView view;
        private float              ultimaAtivacao = -999f;
        private bool               habilitado     = true;

        private void Awake()  => view = GetComponent<BotaoPrincipalView>();

        // ── Eventos XR (conectar no Inspector via XRSimpleInteractable) ──

        public void AoEntrarHover() => view?.MostrarEstadoHover();
        public void AoSairHover()   => view?.MostrarEstadoNormal();

        public void AoPressionar()
        {
            if (!habilitado) return;
            if (Time.time - ultimaAtivacao < intervaloEntreAtivacoes) return;

            ultimaAtivacao = Time.time;
            view?.MostrarEstadoPressionado();
            Invoke(nameof(RetornarNormal), 0.25f);

            Debug.Log($"[BotaoPrincipalController] '{rotulo}' ativado.");
            OnAtivado?.Invoke();
        }

        // ── Fallback mouse — testes no Editor ───────────────────

        private void OnMouseDown()  => AoPressionar();
        private void OnMouseEnter() => AoEntrarHover();
        private void OnMouseExit()  => AoSairHover();

        // ── API pública ──────────────────────────────────────────

        public void SetHabilitado(bool valor)
        {
            habilitado = valor;
            if (!valor) view?.MostrarEstadoNormal();
        }

        private void RetornarNormal() => view?.MostrarEstadoNormal();
    }
}
