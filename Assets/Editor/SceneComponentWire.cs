#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class SceneComponentWire : EditorWindow
{
    [MenuItem("Tools/Add Missing Components")]
    public static void AddComponents()
    {
        // ── GameManager ──────────────────────────────────────────
        var gmGO = GameObject.Find("GameManager");
        if (gmGO != null)
        {
            AddIfMissing<GameManager>(gmGO);
            Debug.Log("✅ GameManager: componente adicionado");
        }

        // ── EventSystem ──────────────────────────────────────────
        var esGO = GameObject.Find("EventSystem");
        if (esGO != null)
        {
            AddIfMissing<EventSystem>(esGO);
            AddIfMissing<StandaloneInputModule>(esGO);
            Debug.Log("✅ EventSystem: componentes adicionados");
        }

        // ── HUD_Canvas ───────────────────────────────────────────
        var hudGO = GameObject.Find("HUD_Canvas");
        if (hudGO != null)
        {
            var canvas = AddIfMissing<Canvas>(hudGO);
            canvas.renderMode = RenderMode.WorldSpace;
            AddIfMissing<CanvasScaler>(hudGO);
            AddIfMissing<GraphicRaycaster>(hudGO);
            AddIfMissing<HUDView>(hudGO);

            // Posicionar HUD na frente do player
            hudGO.transform.position    = new Vector3(0f, 1.6f, 2f);
            hudGO.transform.localScale  = new Vector3(0.002f, 0.002f, 0.002f);

            // Textos TMP
            SetupTMP("Texto_Pontuacao", "Pontos: 0");
            SetupTMP("Texto_Objetos",   "Objetos: 0/3");
            SetupTMP("Texto_Mensagem",  "Colete todos os objetos!");

            EditorUtility.SetDirty(hudGO);
            Debug.Log("✅ HUD_Canvas: componentes e posição configurados");
        }

        // ── XROrigin / PlayerController ──────────────────────────
        var xrGO = GameObject.Find("XROrigin");
        if (xrGO != null)
        {
            AddIfMissing<PlayerController>(xrGO);
            xrGO.tag = "Player";
            EditorUtility.SetDirty(xrGO);
            Debug.Log("✅ XROrigin: PlayerController e tag Player configurados");
        }

        // ── Objetos Coletáveis ───────────────────────────────────
        string[] nomes = { "Chave", "Cristal", "Moeda" };
        int[]    pts   = { 10, 20, 30 };

        for (int i = 1; i <= 3; i++)
        {
            var go = GameObject.Find($"Objeto_Coletavel_0{i}");
            if (go == null) continue;

            AddIfMissing<BoxCollider>(go);
            AddIfMissing<ObjetoColetavelView>(go);
            var ctrl = AddIfMissing<ObjetoColetavelController>(go);
            ctrl.nomeObjeto = nomes[i - 1];
            ctrl.pontos     = pts[i - 1];

            // XRSimpleInteractable
            var xrInt = AddIfMissing<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>(go);
            // Conectar OnInteracaoXR via UnityEvent seria manual,
            // mas registramos o listener via código:
            xrInt.selectEntered.AddListener(ctrl.OnInteracaoXR);

            EditorUtility.SetDirty(go);
            Debug.Log($"✅ Objeto_Coletavel_0{i} ({nomes[i-1]}): componentes adicionados");
        }

        // ── Botao_Principal ──────────────────────────────────────
        var botaoGO = GameObject.Find("Botao_Principal");
        if (botaoGO != null)
        {
            AddIfMissing<CapsuleCollider>(botaoGO);
            AddIfMissing<BotaoPrincipalView>(botaoGO);
            var bCtrl = AddIfMissing<BotaoPrincipalController>(botaoGO);

            var xrInt = AddIfMissing<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>(botaoGO);
            xrInt.hoverEntered.AddListener(bCtrl.AoEntrarHover);
            xrInt.hoverExited.AddListener(bCtrl.AoSairHover);
            xrInt.selectEntered.AddListener(bCtrl.AoPressionar);

            EditorUtility.SetDirty(botaoGO);
            Debug.Log("✅ Botao_Principal: componentes e eventos conectados");
        }

        // ── Auto-Wire referências ─────────────────────────────────
        SceneAutoWire.ConectarReferencias();

        // ── Salvar cena ───────────────────────────────────────────
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("✅ Tudo pronto! Salve com Ctrl+S.");
    }

    static T AddIfMissing<T>(GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null) comp = go.AddComponent<T>();
        return comp;
    }

    static void SetupTMP(string goName, string textoInicial)
    {
        var go = GameObject.Find(goName);
        if (go == null) return;
        var tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = textoInicial;
        tmp.fontSize  = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        var rect = go.GetComponent<RectTransform>();
        if (rect) rect.sizeDelta = new Vector2(300f, 60f);
        EditorUtility.SetDirty(go);
    }
}
#endif
