using UnityEngine;

// Controla o painel de informações que aparece ao interagir com uma obra.
// Pode ser ativado diretamente pelo ExibitoController ou por trigger de proximidade.
public class PainelInfoController : MonoBehaviour
{
    [SerializeField] private PainelInfoView view;
    [SerializeField] private float tempoAutoFechar = 6f;

    private float temporizador = 0f;
    private bool  ativo        = false;

    private void Awake()
    {
        if (view == null) view = GetComponentInChildren<PainelInfoView>(true);
        view?.Ocultar();
    }

    private void Update()
    {
        if (!ativo) return;

        temporizador += Time.deltaTime;
        if (temporizador >= tempoAutoFechar)
            Ocultar();
    }

    public void ExibirInfo(string texto)
    {
        view?.Exibir(texto);
        ativo        = true;
        temporizador = 0f;
    }

    public void Ocultar()
    {
        view?.Ocultar();
        ativo = false;
    }

    // Trigger de proximidade: ativa quando jogador entra na zona
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var exibito = GetComponentInParent<ExibitoController>();
            if (exibito != null) exibito.AoEntrarHover();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var exibito = GetComponentInParent<ExibitoController>();
            if (exibito != null) exibito.AoSairHover();
        }
    }
}
