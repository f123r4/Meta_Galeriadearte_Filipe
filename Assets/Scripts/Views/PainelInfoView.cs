using UnityEngine;
using TMPro;

// Exibe e oculta o painel de informações de uma obra.
// O painel é um World Space Canvas filho do exibito.
public class PainelInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoInfo;
    [SerializeField] private GameObject     painelRaiz;
    [SerializeField] private float          velocidadeFade = 4f;

    private CanvasGroup grupo;

    private void Awake()
    {
        if (painelRaiz == null) painelRaiz = gameObject;
        grupo = painelRaiz.GetComponent<CanvasGroup>();
        if (grupo == null) grupo = painelRaiz.AddComponent<CanvasGroup>();
        grupo.alpha = 0f;
        painelRaiz.SetActive(false);
    }

    private void Update()
    {
        // Fade in suave quando ativo
        if (painelRaiz.activeSelf && grupo.alpha < 1f)
            grupo.alpha = Mathf.MoveTowards(grupo.alpha, 1f, velocidadeFade * Time.deltaTime);
    }

    public void Exibir(string texto)
    {
        if (textoInfo != null) textoInfo.text = texto;
        grupo.alpha = 0f;
        painelRaiz.SetActive(true);

        // Vira o painel para a câmera principal
        var cam = Camera.main;
        if (cam != null)
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                             cam.transform.rotation * Vector3.up);
    }

    public void Ocultar()
    {
        painelRaiz.SetActive(false);
        grupo.alpha = 0f;
    }
}
