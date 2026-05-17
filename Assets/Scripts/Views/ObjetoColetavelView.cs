using UnityEngine;

// Visual do artefato coletável: rotação contínua e escala ao coletar.
public class ObjetoColetavelView : MonoBehaviour
{
    [SerializeField] private float velocidadeRotacao = 90f;
    [SerializeField] private float velocidadeEncolher = 5f;

    private bool coletando = false;

    private void Update()
    {
        transform.Rotate(Vector3.up, velocidadeRotacao * Time.deltaTime, Space.World);

        if (coletando)
        {
            transform.localScale = Vector3.MoveTowards(
                transform.localScale, Vector3.zero, velocidadeEncolher * Time.deltaTime);

            if (transform.localScale.sqrMagnitude < 0.001f)
                gameObject.SetActive(false);
        }
    }

    public void Coletar()
    {
        coletando = true;
    }
}
