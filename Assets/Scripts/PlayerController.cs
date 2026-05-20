using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera referenciaCamera;
    [SerializeField] private float velocidade        = 3f;
    [SerializeField] private float velocidadeRotacao = 120f;
    [SerializeField] private float raioInteracao     = 1.8f;

    private static readonly int                  TodасAsLayers = ~0;
    private static readonly QueryTriggerInteraction ComTriggers = QueryTriggerInteraction.Collide;

    private CharacterController        _cc;
    private float                      _velocidadeY;
    private ExibitoController          _exibitoAtual;
    private BotaoPrincipalController   _botaoAtual;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Mover();
        VerificarProximidadeExibito();
        VerificarProximidadeBotao();
    }

    private void Mover()
    {
        if (_cc.isGrounded) _velocidadeY = -0.5f;
        else _velocidadeY += Physics.gravity.y * Time.deltaTime;

        float move   = Input.GetAxis("Vertical");
        float rotate = Input.GetAxis("Horizontal");

        transform.Rotate(0f, rotate * velocidadeRotacao * Time.deltaTime, 0f, Space.Self);

        Vector3 dir = transform.forward * move * velocidade;
        _cc.Move((dir + Vector3.up * _velocidadeY) * Time.deltaTime);
    }

    private void VerificarProximidadeExibito()
    {
        ExibitoController maisProximo = null;
        float menorDist = float.MaxValue;

        foreach (var col in Physics.OverlapSphere(transform.position, raioInteracao, TodасAsLayers, ComTriggers))
        {
            var ex = col.GetComponentInParent<ExibitoController>();
            if (ex == null) continue;
            float d = Vector3.Distance(transform.position, ex.transform.position);
            if (d < menorDist) { menorDist = d; maisProximo = ex; }
        }

        if (maisProximo == _exibitoAtual) return;
        _exibitoAtual?.AoSairHover();
        _exibitoAtual = maisProximo;
        _exibitoAtual?.AoEntrarHover();
    }

    private void VerificarProximidadeBotao()
    {
        BotaoPrincipalController maisProximo = null;
        float menorDist = float.MaxValue;

        foreach (var col in Physics.OverlapSphere(transform.position, raioInteracao, TodасAsLayers, ComTriggers))
        {
            var btn = col.GetComponentInParent<BotaoPrincipalController>();
            if (btn == null) continue;
            float d = Vector3.Distance(transform.position, btn.transform.position);
            if (d < menorDist) { menorDist = d; maisProximo = btn; }
        }

        if (maisProximo != _botaoAtual)
        {
            _botaoAtual?.AoSairHoverProximidade();
            _botaoAtual = maisProximo;
            _botaoAtual?.AoEntrarHoverProximidade();
        }

        if (_botaoAtual != null && Input.GetKeyDown(KeyCode.E))
            _botaoAtual.AoPressionarProximidade();
    }

}
