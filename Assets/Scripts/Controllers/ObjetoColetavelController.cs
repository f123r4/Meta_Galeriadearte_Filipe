using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Controla a lógica de coleta de artefatos no museu virtual.
// Integrado com GaleriaManager (versão avançada).
public class ObjetoColetavelController : MonoBehaviour
{
    [SerializeField] public string nomeObjeto = "Artefato";
    [SerializeField] public int    pontos     = 10;

    private bool coletado = false;

    public void TentarColetar()
    {
        if (coletado) return;
        coletado = true;

        GaleriaManager.Instance?.RegistrarInteracaoExibito(nomeObjeto, pontos);

        var view = GetComponent<ObjetoColetavelView>();
        if (view != null) view.Coletar();
        else gameObject.SetActive(false);
    }

    public void OnInteracaoXR(SelectEnterEventArgs _) => TentarColetar();
}
