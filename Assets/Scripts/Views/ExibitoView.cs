using UnityEngine;

// Feedback visual de uma obra de arte: hover leve e destaque ao ser interagida.
// Usa emissão do material URP para o brilho sem alocar novos materiais.
[RequireComponent(typeof(Renderer))]
public class ExibitoView : MonoBehaviour
{
    [SerializeField] private Color corHover   = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private Color corAtivada = new Color(0.2f, 1f,   0.5f);
    [SerializeField] private float intensidade = 0.4f;
    [SerializeField] private float velocidadeTransicao = 3f;

    private Renderer rend;
    private Material _mat;          // instância única criada em Awake — sem leak
    private Color    corEmissaoAlvo;
    private Color    corEmissaoAtual;
    private bool     pulsando = false;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        _mat = rend.material;       // cria a instância uma única vez; Unity rastreia e destrói ao descarregar a cena
        _mat.EnableKeyword("_EMISSION");
        corEmissaoAtual = Color.black;
        corEmissaoAlvo  = Color.black;
    }

    private void Update()
    {
        if (pulsando)
        {
            float pulse = (Mathf.Sin(Time.time * 2f) + 1f) * 0.5f;
            corEmissaoAtual = Color.Lerp(corAtivada * 0.3f, corAtivada * intensidade, pulse);
        }
        else
        {
            corEmissaoAtual = Color.Lerp(corEmissaoAtual, corEmissaoAlvo,
                                         velocidadeTransicao * Time.deltaTime);
        }

        _mat.SetColor(EmissionColor, corEmissaoAtual);
    }

    public void SetHover(bool hover)
    {
        corEmissaoAlvo = hover ? corHover * intensidade : Color.black;
    }

    // Ativa destaque permanente (obra já visitada)
    public void AtivarDestaque()
    {
        pulsando = true;
    }
}
