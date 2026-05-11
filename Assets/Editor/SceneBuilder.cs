#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Cria toda a hierarquia da cena via menu Tools/Build Scene Structure
public class SceneBuilder : EditorWindow
{
    [MenuItem("Tools/Build Scene Structure")]
    public static void ConstruirCena()
    {
        CriarManagement();
        CriarPlayer();
        CriarEnvironment();
        CriarInteractables();

        Debug.Log("[SceneBuilder] Hierarquia da cena criada com sucesso!");
    }

    // ── [--- MANAGEMENT ---] ────────────────────────────────────

    private static void CriarManagement()
    {
        GameObject raiz = new GameObject("[--- MANAGEMENT ---]");

        // GameManager
        GameObject gmObj = new GameObject("GameManager");
        gmObj.transform.SetParent(raiz.transform);
        gmObj.AddComponent<GameManager>();

        // EventSystem
        GameObject esObj = new GameObject("EventSystem");
        esObj.transform.SetParent(raiz.transform);
        esObj.AddComponent<EventSystem>();
        esObj.AddComponent<StandaloneInputModule>();

        // HUD_Canvas (World Space)
        GameObject canvasObj = new GameObject("HUD_Canvas");
        canvasObj.transform.SetParent(raiz.transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Reposiciona o canvas à frente do jogador
        canvasObj.transform.localPosition = new Vector3(0f, 1.5f, 2f);
        canvasObj.transform.localScale    = Vector3.one * 0.01f;

        // Três filhos TextMeshProUGUI
        CriarTextoTMP(canvasObj, "Texto_Pontuacao", new Vector2(0,  60), "Pontuação: 0");
        CriarTextoTMP(canvasObj, "Texto_Objetos",   new Vector2(0,   0), "Objetos: 0/3");
        CriarTextoTMP(canvasObj, "Texto_Mensagem",  new Vector2(0, -60), "Bem-vindo!");

        // HUDView no canvas
        canvasObj.AddComponent<HUDView>();
    }

    // Cria um filho TextMeshProUGUI dentro de um pai
    private static GameObject CriarTextoTMP(GameObject pai, string nome, Vector2 posicao, string conteudo)
    {
        GameObject obj = new GameObject(nome);
        obj.transform.SetParent(pai.transform, false);

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text      = conteudo;
        tmp.fontSize  = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = Color.white;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = posicao;
        rt.sizeDelta        = new Vector2(400, 50);

        return obj;
    }

    // ── [--- PLAYER ---] ────────────────────────────────────────

    private static void CriarPlayer()
    {
        GameObject raiz = new GameObject("[--- PLAYER ---]");

        GameObject xrOrigin = new GameObject("XROrigin");
        xrOrigin.transform.SetParent(raiz.transform);
        xrOrigin.tag = "Player";
        xrOrigin.AddComponent<PlayerController>();
    }

    // ── [--- ENVIRONMENT ---] ────────────────────────────────────

    private static void CriarEnvironment()
    {
        GameObject raiz = new GameObject("[--- ENVIRONMENT ---]");

        // Chão
        GameObject chao = GameObject.CreatePrimitive(PrimitiveType.Plane);
        chao.name = "Plane_Chao";
        chao.transform.SetParent(raiz.transform);
        chao.transform.localScale = new Vector3(5f, 1f, 5f);

        // Luz direcional
        GameObject luz = new GameObject("Directional Light");
        luz.transform.SetParent(raiz.transform);
        Light comp = luz.AddComponent<Light>();
        comp.type      = LightType.Directional;
        comp.intensity = 1f;
        luz.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Casa
        GameObject casa = new GameObject("Casa");
        casa.transform.SetParent(raiz.transform);

        // Paredes (filho de Casa)
        GameObject paredes = new GameObject("Paredes");
        paredes.transform.SetParent(casa.transform);

        // Porta (filho de Casa)
        GameObject porta = new GameObject("Porta");
        porta.transform.SetParent(casa.transform);

        // Maçaneta (filha de Porta)
        GameObject macaneta = new GameObject("Macaneta");
        macaneta.transform.SetParent(porta.transform);
    }

    // ── [--- INTERACTABLES ---] ──────────────────────────────────

    private static void CriarInteractables()
    {
        GameObject raiz = new GameObject("[--- INTERACTABLES ---]");

        // Três objetos coletáveis (cubos com collider incluído)
        for (int i = 1; i <= 3; i++)
        {
            GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubo.name = "Objeto_Coletavel_0" + i;
            cubo.transform.SetParent(raiz.transform);
            cubo.transform.position    = new Vector3(i * 2f - 4f, 0.5f, 2f);
            cubo.transform.localScale  = Vector3.one * 0.3f;
            cubo.AddComponent<ObjetoColetavelView>();
            cubo.AddComponent<ObjetoColetavelController>();
        }

        // Botão principal (cilindro com collider incluído)
        GameObject botao = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        botao.name = "Botao_Principal";
        botao.transform.SetParent(raiz.transform);
        botao.transform.position   = new Vector3(0f, 0.5f, -2f);
        botao.transform.localScale = new Vector3(0.5f, 0.25f, 0.5f);
        botao.AddComponent<BotaoPrincipalView>();
        botao.AddComponent<BotaoPrincipalController>();
    }
}
#endif
