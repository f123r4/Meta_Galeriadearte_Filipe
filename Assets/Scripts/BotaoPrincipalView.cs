using UnityEngine;

// Controla o feedback visual do botão (cor do material)
public class BotaoPrincipalView : MonoBehaviour
{
    private Renderer rend;

    private static readonly Color corNormal = Color.white;
    private static readonly Color corHover  = Color.yellow;
    private static readonly Color corAtivo  = Color.green;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Muda a cor conforme o estado de hover
    public void SetHover(bool hover)
    {
        if (rend != null)
            rend.material.color = hover ? corHover : corNormal;
    }

    // Muda a cor para indicar que o botão foi ativado
    public void SetAtivo()
    {
        if (rend != null)
            rend.material.color = corAtivo;
    }
}
