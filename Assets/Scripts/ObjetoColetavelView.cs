using UnityEngine;

// Controla a aparência visual do objeto coletável (rotação e desativação)
public class ObjetoColetavelView : MonoBehaviour
{
    [SerializeField] private float velocidadeRotacao = 90f;

    private void Update()
    {
        // Rotação contínua para indicar que o objeto é interativo
        transform.Rotate(Vector3.up, velocidadeRotacao * Time.deltaTime, Space.World);
    }

    // Desativa o objeto ao ser coletado
    public void Coletar()
    {
        gameObject.SetActive(false);
    }
}
