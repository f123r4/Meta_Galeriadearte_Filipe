using UnityEngine;
using TMPro;

public class HUDView : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI textoPontuacao;
    [SerializeField] public TextMeshProUGUI textoMensagem;

    [SerializeField] private float distancia    = 2f;
    [SerializeField] private float alturaOffset = 1.4f;
    [SerializeField] private float tempoMensagem = 4f;

    private float timerMensagem;

    private void Start()
    {
        AncorarAoJogador();
        AtualizarPontuacao(0);
        ExibirMensagem("Bem-vindo ao MetaMuseu!");
    }

    private void AncorarAoJogador()
    {
        GameObject ancora = GameObject.FindWithTag("Player");
        if (ancora == null)
        {
            var cam = Camera.main ?? FindFirstObjectByType<Camera>();
            if (cam != null) ancora = cam.gameObject;
        }
        if (ancora == null) return;

        Vector3 escala = transform.localScale;
        transform.SetParent(ancora.transform, false);
        transform.localScale    = escala;
        transform.localPosition = new Vector3(0f, alturaOffset, distancia);
        transform.localRotation = Quaternion.identity;
    }

    private void LateUpdate()
    {
        if (timerMensagem <= 0f) return;
        timerMensagem -= Time.deltaTime;
        if (timerMensagem <= 0f && textoMensagem != null)
            textoMensagem.text = "";
    }

    public void AtualizarPontuacao(int pontos)
    {
        if (textoPontuacao) textoPontuacao.text = $"Pontos: {pontos}";
    }

    public void ExibirMensagem(string msg)
    {
        if (textoMensagem) textoMensagem.text = msg;
        timerMensagem = tempoMensagem;
    }
}
