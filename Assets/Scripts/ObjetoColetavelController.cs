using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Controla a lógica de coleta do objeto coletável
public class ObjetoColetavelController : MonoBehaviour
{
    [SerializeField] public string nomeObjeto = "Objeto";
    [SerializeField] public int pontos = 10;

    private bool coletado = false;

    // Tenta coletar o objeto por proximidade (chamado pelo PlayerController)
    public void TentarColetar()
    {
        if (coletado) return;

        coletado = true;

        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
            gm.RegistrarColeta(pontos);

        ObjetoColetavelView view = GetComponent<ObjetoColetavelView>();
        if (view != null)
            view.Coletar();
        else
            gameObject.SetActive(false);
    }

    // Interação via XR (XRSimpleInteractable — SelectEnterEventArgs)
    public void OnInteracaoXR(SelectEnterEventArgs args)
    {
        TentarColetar();
    }
}
