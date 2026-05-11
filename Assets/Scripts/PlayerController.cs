using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public Camera referenciaCamera;
    [SerializeField] float velocidade = 3f;
    [SerializeField] float raioColeta = 1.5f;
    [SerializeField] float raioBotao  = 1.2f;

    private BotaoPrincipalController botaoAtual = null;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(h, 0, v) * velocidade * Time.deltaTime, Space.Self);

        // Coleta por proximidade
        foreach (var col in Physics.OverlapSphere(transform.position, raioColeta))
        {
            var ctrl = col.GetComponent<ObjetoColetavelController>();
            if (ctrl != null) ctrl.TentarColetar();
        }

        // Hover do botao por proximidade
        BotaoPrincipalController botaoEncontrado = null;
        foreach (var col in Physics.OverlapSphere(transform.position, raioBotao))
        {
            var b = col.GetComponent<BotaoPrincipalController>();
            if (b != null) { botaoEncontrado = b; break; }
        }

        if (botaoEncontrado != null && botaoAtual == null)
        {
            botaoAtual = botaoEncontrado;
            botaoAtual.AoEntrarHoverProximidade();
        }
        else if (botaoEncontrado == null && botaoAtual != null)
        {
            botaoAtual.AoSairHoverProximidade();
            botaoAtual = null;
        }

        if (botaoAtual != null && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
            botaoAtual.AoPressionarProximidade();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioColeta);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioBotao);
    }
}
