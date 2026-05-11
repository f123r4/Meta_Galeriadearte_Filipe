using UnityEngine;
using TMPro;

public class HUDView : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI textoPontuacao;
    [SerializeField] public TextMeshProUGUI textoObjetos;
    [SerializeField] public TextMeshProUGUI textoMensagem;

    private Transform camTransform;
    [SerializeField] float distancia = 2f;
    [SerializeField] float alturaOffset = -0.2f;

    void Start()
    {
        // Pegar a câmera central do OVRCameraRig
        var cam = Camera.main;
        if (cam == null) cam = FindFirstObjectByType<Camera>();
        if (cam != null) camTransform = cam.transform;
    }

    void LateUpdate()
    {
        if (camTransform == null) return;

        // Seguir a câmera
        Vector3 forward = camTransform.forward;
        forward.y = 0;
        forward.Normalize();

        transform.position = camTransform.position
            + forward * distancia
            + Vector3.up * alturaOffset;

        transform.rotation = Quaternion.LookRotation(forward);
    }

    public void AtualizarPontuacao(int pontos)
    {
        if (textoPontuacao) textoPontuacao.text = $"Pontos: {pontos}";
    }

    public void AtualizarObjetos(int coletados, int total)
    {
        if (textoObjetos) textoObjetos.text = $"Objetos: {coletados}/{total}";
    }

    public void ExibirMensagem(string msg)
    {
        if (textoMensagem) textoMensagem.text = msg;
    }
}
