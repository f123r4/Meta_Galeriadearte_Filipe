// =============================================================
// Controllers/ObjetoColetavelController.cs
// Detecta interação e delega a lógica de coleta ao GameManager
// Interação por proximidade (trigger) + XRSimpleInteractable
// Padrão MVC — camada Controller
// =============================================================

using UnityEngine;
using MeuAmbienteVR.Models;
using MeuAmbienteVR.Views;

namespace MeuAmbienteVR.Controllers
{
    [RequireComponent(typeof(Collider))]
    public class ObjetoColetavelController : MonoBehaviour
    {
        [Header("Dados do Objeto")]
        [SerializeField] private string nomeObjeto = "Objeto Coletável";
        [SerializeField] private int    pontos     = 10;

        private ObjetoColetavelModel modelo;
        private ObjetoColetavelView  view;

        private void Awake()
        {
            view   = GetComponent<ObjetoColetavelView>();
            modelo = new ObjetoColetavelModel(gameObject.name, nomeObjeto, pontos);
            GetComponent<Collider>().isTrigger = true;
        }

        private void Start()
        {
            // Registra este objeto na contagem do GameManager
            GameManager.Instancia?.RegistrarObjetoColetavel(modelo);
        }

        // Coleta por proximidade — jogador caminha até o objeto
        private void OnTriggerEnter(Collider outro)
        {
            if (!outro.CompareTag("Player")) return;
            TentarColetar();
        }

        // Conectar ao evento "Select Entered" do XRSimpleInteractable no Inspector
        public void OnInteracaoXR() => TentarColetar();

        private void TentarColetar()
        {
            if (modelo.Coletado) return;
            GameManager.Instancia?.RegistrarColeta(modelo);
            view?.ExecutarAnimacaoColeta();
            Debug.Log($"[ObjetoColetavelController] '{nomeObjeto}' coletado | +{pontos} pts");
        }
    }
}
