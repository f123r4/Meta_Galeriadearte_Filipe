using UnityEngine;

// Feedback visual do validador de ingresso: azul (standby) → amarelo (hover) → verde (aprovado).
[RequireComponent(typeof(Renderer))]
public class BotaoPrincipalView : MonoBehaviour
{
    private Renderer rend;
    private Material _mat;

    private static readonly Color corNormal = new Color(0.15f, 0.55f, 0.90f); // azul standby
    private static readonly Color corHover  = new Color(1f,   0.85f, 0.10f);  // amarelo
    private static readonly Color corAtivo  = new Color(0.10f, 0.90f, 0.30f); // verde aprovado

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend)
        {
            _mat = rend.material;
            AplicarCor(corNormal);
        }
    }

    public void SetHover(bool hover) => AplicarCor(hover ? corHover : corNormal);

    public void SetAtivo() => AplicarCor(corAtivo);

    private void AplicarCor(Color c)
    {
        if (_mat == null) return;
        _mat.color = c;
        if (_mat.HasProperty("_BaseColor")) _mat.SetColor("_BaseColor", c);
        if (_mat.HasProperty("_Color"))     _mat.SetColor("_Color",     c);
    }
}
