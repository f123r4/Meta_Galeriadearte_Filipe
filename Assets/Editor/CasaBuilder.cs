#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CasaBuilder : EditorWindow
{
    [MenuItem("Tools/Build Casa")]
    public static void BuildCasa()
    {
        var casa = GameObject.Find("Casa");
        if (casa == null) { Debug.LogError("❌ GameObject 'Casa' não encontrado!"); return; }

        var paredes = casa.transform.Find("Paredes")?.gameObject
                      ?? new GameObject("Paredes") { transform = { parent = casa.transform } };

        var portaGO = casa.transform.Find("Porta")?.gameObject
                      ?? new GameObject("Porta") { transform = { parent = casa.transform } };

        // Posição base da casa
        Vector3 c = casa.transform.position;
        float largura = 6f, profundidade = 6f, altura = 3f, espessura = 0.2f;

        // Parede Fundo
        CriarParede(paredes, "Parede_Fundo",
            c + new Vector3(0, altura/2, profundidade/2),
            new Vector3(largura, altura, espessura));

        // Parede Esquerda
        CriarParede(paredes, "Parede_Esquerda",
            c + new Vector3(-largura/2, altura/2, 0),
            new Vector3(espessura, altura, profundidade));

        // Parede Direita
        CriarParede(paredes, "Parede_Direita",
            c + new Vector3(largura/2, altura/2, 0),
            new Vector3(espessura, altura, profundidade));

        // Parede Frente Esquerda (ao lado da porta)
        CriarParede(paredes, "Parede_Frente_Esq",
            c + new Vector3(-2f, altura/2, -profundidade/2),
            new Vector3(2f, altura, espessura));

        // Parede Frente Direita (ao lado da porta)
        CriarParede(paredes, "Parede_Frente_Dir",
            c + new Vector3(2f, altura/2, -profundidade/2),
            new Vector3(2f, altura, espessura));

        // Frente Superior (acima da porta)
        CriarParede(paredes, "Parede_Frente_Topo",
            c + new Vector3(0, altura - 0.5f, -profundidade/2),
            new Vector3(2f, 1f, espessura));

        // Teto
        CriarParede(paredes, "Teto",
            c + new Vector3(0, altura, 0),
            new Vector3(largura, espessura, profundidade));

        // Porta (visual)
        portaGO.transform.position = c + new Vector3(0, 1f, -profundidade/2);
        var portaMesh = portaGO.GetComponent<MeshFilter>() != null
            ? portaGO : CriarCubo(portaGO.transform.parent?.gameObject ?? casa, "Porta_Mesh",
                c + new Vector3(0, 1f, -profundidade/2),
                new Vector3(2f, 2f, espessura));
        portaMesh.transform.SetParent(portaGO.transform);
        portaMesh.name = "Porta_Mesh";
        AplicarCor(portaMesh, new Color(0.55f, 0.27f, 0.07f)); // marrom

        // Maçaneta
        var macaneta = portaGO.transform.Find("Macaneta")?.gameObject;
        if (macaneta == null) macaneta = new GameObject("Macaneta");
        macaneta.transform.SetParent(portaGO.transform);
        var mac = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mac.name = "Macaneta_Mesh";
        mac.transform.SetParent(macaneta.transform);
        mac.transform.position = c + new Vector3(0.7f, 1f, -profundidade/2 - 0.15f);
        mac.transform.localScale = Vector3.one * 0.15f;
        AplicarCor(mac, new Color(1f, 0.85f, 0f)); // dourado

        // Chão interno
        CriarParede(casa, "Chao_Interno",
            c + new Vector3(0, 0.01f, 0),
            new Vector3(largura - espessura, 0.02f, profundidade - espessura),
            new Color(0.6f, 0.5f, 0.4f));

        EditorUtility.SetDirty(casa);
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("✅ Casa construída! Salve com Ctrl+S.");
    }

    static GameObject CriarParede(GameObject pai, string nome, Vector3 pos, Vector3 scale, Color? cor = null)
    {
        // Reusa se já existe
        var existing = pai.transform.Find(nome);
        if (existing != null) return existing.gameObject;

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = nome;
        go.transform.SetParent(pai.transform);
        go.transform.position = pos;
        go.transform.localScale = scale;
        AplicarCor(go, cor ?? new Color(0.85f, 0.82f, 0.75f));
        return go;
    }

    static GameObject CriarCubo(GameObject pai, string nome, Vector3 pos, Vector3 scale)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = nome;
        go.transform.SetParent(pai.transform);
        go.transform.position = pos;
        go.transform.localScale = scale;
        return go;
    }

    static void AplicarCor(GameObject go, Color cor)
    {
        var rend = go.GetComponent<Renderer>();
        if (rend == null) return;
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat.shader.name == "Hidden/InternalErrorShader")
            mat = new Material(Shader.Find("Standard"));
        mat.color = cor;
        rend.material = mat;
    }
}
#endif
