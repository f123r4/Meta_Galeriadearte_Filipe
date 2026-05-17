using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Controla a animação de abertura/fechamento da porta via Quaternion.Lerp.
// autoAbrirProximidade = false → porta só abre por chamada explícita (ex.: botão de ingresso).
public class PortaAnimadaController : MonoBehaviour
{
    [SerializeField] private Transform pivoPiso;
    [SerializeField] private float anguloAberta           = 90f;
    [SerializeField] private float velocidade             = 2f;
    [SerializeField] private bool  autoAbrirProximidade   = true;

    private bool  aberta         = false;
    private bool  emMovimento    = false;
    private float progressoAlvo  = 0f;
    private float progressoAtual = 0f;

    private Quaternion rotacaoFechada;
    private Quaternion rotacaoAberta;

    private void Awake()
    {
        bool temTrigger = false;
        foreach (var bc in GetComponents<BoxCollider>())
            if (bc.isTrigger) { temTrigger = true; break; }

        if (!temTrigger)
        {
            var t = gameObject.AddComponent<BoxCollider>();
            t.isTrigger = true;
            t.center    = new Vector3(0.5f, 1.1f, 0f);
            t.size      = new Vector3(3f,   2.5f, 4f);
        }
    }

    private void Start()
    {
        if (pivoPiso == null) pivoPiso = transform;
        rotacaoFechada = pivoPiso.localRotation;
        rotacaoAberta  = rotacaoFechada * Quaternion.Euler(0f, anguloAberta, 0f);
    }

    private void Update()
    {
        if (!emMovimento) return;

        progressoAtual = Mathf.MoveTowards(progressoAtual, progressoAlvo, velocidade * Time.deltaTime);
        pivoPiso.localRotation = Quaternion.Lerp(rotacaoFechada, rotacaoAberta, progressoAtual);

        if (Mathf.Approximately(progressoAtual, progressoAlvo))
            emMovimento = false;
    }

    public void Alternar()
    {
        aberta        = !aberta;
        progressoAlvo = aberta ? 1f : 0f;
        emMovimento   = true;

        string estado = aberta ? "aberta" : "fechada";
        GaleriaManager.Instance?.ExibirMensagem($"Porta {estado}!");
    }

    // ── Eventos XR ────────────────────────────────────────────────────────────
    public void OnXRSelectEntered(SelectEnterEventArgs _) => Alternar();

    // ── Proximidade automática (só quando habilitada) ─────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!autoAbrirProximidade) return;
        if (other.CompareTag("Player") && !aberta)
            Alternar();
    }
}
