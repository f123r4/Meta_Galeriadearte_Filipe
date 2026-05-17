// Dados puros de um exibito do museu — sem MonoBehaviour
[System.Serializable]
public class ExibitoModel
{
    public string titulo;
    public string descricao;
    public string artista;
    public int ano;
    public bool foiInteragido;

    public ExibitoModel(string titulo, string descricao, string artista, int ano)
    {
        this.titulo    = titulo;
        this.descricao = descricao;
        this.artista   = artista;
        this.ano       = ano;
        foiInteragido  = false;
    }

    public string FormatarInfo()
    {
        return $"{titulo}\n{artista}, {ano}\n\n{descricao}";
    }
}
