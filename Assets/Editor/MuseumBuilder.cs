#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// Constrói a cena Galeria de Arte do zero — apaga hierarquia anterior e recria tudo.
/// Menu: MetaMuseu ▶ Construir Cena Completa
/// Layout: ante-sala (botão de ingresso) → porta → salão principal (6 quadros nas paredes).
/// </summary>
public static class MuseumBuilder
{
    // ── Paleta ────────────────────────────────────────────────────────────────
    static readonly Color CorPiso    = new Color(0.88f, 0.84f, 0.76f);
    static readonly Color CorTeto    = new Color(0.97f, 0.97f, 0.97f);
    static readonly Color CorParede  = new Color(0.95f, 0.93f, 0.90f);
    static readonly Color CorMoldura = new Color(0.62f, 0.50f, 0.28f);
    static readonly Color CorSuporte = new Color(0.55f, 0.55f, 0.55f);
    static readonly Color CorBotao   = new Color(0.15f, 0.55f, 0.90f);
    static readonly Color CorTela    = Color.white;

    struct Mats { public Material piso, teto, parede, moldura, suporte, tela, botao; }

    // ── Dimensões ─────────────────────────────────────────────────────────────
    // Sala única: x [-10, +10], z [-14, +10], y [0, 5]
    // Parede divisória (ante-sala/salão) em z = -4, com vão de porta 3×3 m centrado em x=0.
    const float MEIO_W    = 10f;   // metade da largura (x: -10 a +10)
    const float Z_SUL     = -14f;  // parede sul exterior
    const float Z_DIVISOR = -4f;   // parede divisória
    const float Z_NORTE   = +10f;  // parede norte
    const float ALTURA    = 5f;    // pé-direito
    const float H_PORTA   = 3f;    // altura do vão da porta
    const float W_PORTA   = 3f;    // largura do vão da porta

    // =========================================================================
    [MenuItem("MetaMuseu/► Construir Cena Completa", priority = 1)]
    public static void ConstruirTudo()
    {
        LimparCena();
        var mats = CriarMateriais();
        CriarManagement();
        CriarPlayer();
        CriarAmbiente(mats);
        CriarExibitos(mats);
        CriarInteractables(mats);
        ConectarReferencias();
        XRInteractableSetup.Aplicar();
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("[MuseumBuilder] ✅ Galeria de Arte construída! Salve com Ctrl+S.");
    }

    // ── 1. Limpeza total ──────────────────────────────────────────────────────
    static void LimparCena()
    {
        foreach (var nome in new[]
        {
            "[--- MANAGEMENT ---]", "[--- PLAYER ---]",
            "[--- ENVIRONMENT ---]", "[--- EXIBITOS ---]", "[--- INTERACTABLES ---]"
        })
        {
            var go = GameObject.Find(nome);
            if (go != null) Object.DestroyImmediate(go);
        }
    }

    // ── 2. Materiais URP persistidos em Assets/Materials/Museum/ ─────────────
    static Mats CriarMateriais()
    {
        Directory.CreateDirectory("Assets/Materials/Museum");
        Shader s = ObterShader();
        return new Mats
        {
            piso    = Mat(s, "MuseumFloor",   CorPiso,    0.20f),
            teto    = Mat(s, "MuseumCeiling", CorTeto,    0.10f),
            parede  = Mat(s, "MuseumWall",    CorParede,  0.10f),
            moldura = Mat(s, "MuseumFrame",   CorMoldura, 0.45f),
            suporte = Mat(s, "MuseumSupport", CorSuporte, 0.20f),
            tela    = Mat(s, "MuseumCanvas",  CorTela,    0.05f),
            botao   = Mat(s, "MuseumButton",  CorBotao,   0.50f),
        };
    }

    static Shader ObterShader()
    {
        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s != null) return s;
        s = Shader.Find("Universal Render Pipeline/Simple Lit");
        if (s != null) return s;
        return Shader.Find("Standard");
    }

    static Material Mat(Shader s, string nome, Color cor, float smooth)
    {
        string path = $"Assets/Materials/Museum/{nome}.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null) { mat = new Material(s) { name = nome }; AssetDatabase.CreateAsset(mat, path); }
        mat.color = cor;
        if (mat.HasProperty("_BaseColor"))  mat.SetColor("_BaseColor",  cor);
        if (mat.HasProperty("_Color"))      mat.SetColor("_Color",      cor);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smooth);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smooth);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   0f);
        EditorUtility.SetDirty(mat);
        return mat;
    }

    // ── 3. Management: GaleriaManager + EventSystem + HUD Canvas ─────────────
    static void CriarManagement()
    {
        var raiz = new GameObject("[--- MANAGEMENT ---]");

        Filho(raiz, "GaleriaManager").AddComponent<GaleriaManager>();

        var es = Filho(raiz, "EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();

        var hudObj = Filho(raiz, "HUD_Canvas");
        var canvas = hudObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        hudObj.AddComponent<CanvasScaler>();
        hudObj.AddComponent<GraphicRaycaster>();
        hudObj.transform.localScale = Vector3.one * 0.005f;

        TMP(hudObj, "Texto_Pontuacao", new Vector2(0,  35), "Galeria de Arte", 40);
        TMP(hudObj, "Texto_Mensagem",  new Vector2(0, -35), "",                24);
        hudObj.AddComponent<HUDView>();
    }

    // ── 4. Player: XROrigin na ante-sala olhando norte ────────────────────────
    static void CriarPlayer()
    {
        var raiz   = new GameObject("[--- PLAYER ---]");
        var origin = Filho(raiz, "XROrigin");
        origin.transform.position = new Vector3(0f, 0f, -11f); // centro da ante-sala
        origin.tag = "Player";

        var cc = origin.AddComponent<CharacterController>();
        cc.height = 1.8f; cc.center = new Vector3(0f, 0.9f, 0f); cc.radius = 0.3f;
        origin.AddComponent<PlayerController>();

        var camOff = Filho(origin, "CameraOffset");
        camOff.transform.localPosition = new Vector3(0f, 1.7f, 0f);

        var camObj = Filho(camOff, "MainCamera");
        camObj.tag = "MainCamera";
        var cam = camObj.AddComponent<Camera>();
        cam.nearClipPlane = 0.01f;
        camObj.AddComponent<AudioListener>();
    }

    // ── 5. Ambiente: sala única com parede divisória e iluminação ────────────
    static void CriarAmbiente(Mats m)
    {
        var raiz = new GameObject("[--- ENVIRONMENT ---]");

        float largura  = MEIO_W * 2f;          // 20 m
        float profTotal = Z_NORTE - Z_SUL;      // 24 m
        float zCentro   = (Z_SUL + Z_NORTE) / 2f; // -2
        float meiaAltura = ALTURA / 2f;

        // Chão e Teto — Plane Unity 10×10 unidades; scale = (2, 1, 2.4) → 20×24 m
        Plane(raiz, "Chao",
              new Vector3(0f, 0f, zCentro),
              Quaternion.identity,
              new Vector3(largura / 10f, 1f, profTotal / 10f), m.piso);

        Plane(raiz, "Teto",
              new Vector3(0f, ALTURA, zCentro),
              Quaternion.Euler(180f, 0f, 0f),
              new Vector3(largura / 10f, 1f, profTotal / 10f), m.teto);

        // Paredes externas
        Parede(raiz, "Parede_Norte",
               new Vector3(0f,       meiaAltura, Z_NORTE),
               new Vector3(largura + 0.6f, ALTURA, 0.3f), m.parede);

        Parede(raiz, "Parede_Sul",
               new Vector3(0f,       meiaAltura, Z_SUL),
               new Vector3(largura + 0.6f, ALTURA, 0.3f), m.parede);

        Parede(raiz, "Parede_Leste",
               new Vector3( MEIO_W, meiaAltura, zCentro),
               new Vector3(0.3f, ALTURA, profTotal + 0.6f), m.parede);

        Parede(raiz, "Parede_Oeste",
               new Vector3(-MEIO_W, meiaAltura, zCentro),
               new Vector3(0.3f, ALTURA, profTotal + 0.6f), m.parede);

        // Parede divisória (ante-sala / salão) — vão de porta 3×3 m centrado em x=0
        // Lateral esq: x de -10 a -1.5 (centro x = -5.75, largura = 8.5 m)
        Parede(raiz, "Div_Esq",
               new Vector3(-5.75f, meiaAltura, Z_DIVISOR),
               new Vector3(8.5f, ALTURA, 0.3f), m.parede);

        // Lateral dir: x de +1.5 a +10 (centro x = +5.75, largura = 8.5 m)
        Parede(raiz, "Div_Dir",
               new Vector3(5.75f, meiaAltura, Z_DIVISOR),
               new Vector3(8.5f, ALTURA, 0.3f), m.parede);

        // Topo acima do vão (y de H_PORTA a ALTURA, altura = 2 m)
        float topoH   = ALTURA - H_PORTA;
        float topoCY  = H_PORTA + topoH / 2f;
        Parede(raiz, "Div_Topo",
               new Vector3(0f, topoCY, Z_DIVISOR),
               new Vector3(largura + 0.6f, topoH, 0.3f), m.parede);

        // Iluminação da ante-sala (ponto)
        var anteLuz = Filho(raiz, "Luz_AnteSala");
        anteLuz.transform.position = new Vector3(0f, 4.2f, -9f);
        var al = anteLuz.AddComponent<Light>();
        al.type = LightType.Point;
        al.color = new Color(1f, 0.95f, 0.88f);
        al.intensity = 1.5f; al.range = 12f;

        // Iluminação do salão (ponto)
        var salaLuz = Filho(raiz, "Luz_Salao");
        salaLuz.transform.position = new Vector3(0f, 4.5f, 3f);
        var sl = salaLuz.AddComponent<Light>();
        sl.type = LightType.Point;
        sl.color = new Color(1f, 0.97f, 0.92f);
        sl.intensity = 1.2f; sl.range = 20f;

        // Luz direcional suave (tom âmbar)
        var dirLuz = Filho(raiz, "Directional_Light");
        dirLuz.transform.rotation = Quaternion.Euler(40f, 30f, 0f);
        var dl = dirLuz.AddComponent<Light>();
        dl.type = LightType.Directional;
        dl.color = new Color(1f, 0.97f, 0.90f);
        dl.intensity = 0.65f;

        RenderSettings.ambientMode  = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.45f, 0.43f, 0.40f);
    }

    // ── 6. Exibitos: 6 quadros pendurados nas paredes do salão ───────────────
    static void CriarExibitos(Mats m)
    {
        var raiz = new GameObject("[--- EXIBITOS ---]");

        // Moldura dourada: carrega do PolyKebap se disponível
        var frameMat = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/PolyKebap/OilPaintings/Materials/PaintingFrame_Gold.mat") ?? m.moldura;

        // 6 obras distribuídas: 2 na parede norte, 2 na parede leste, 2 na parede oeste.
        // O salão ocupa z: -4 a +10, x: -10 a +10.
        // pos = centro do quadro; rot = a "frente" do quadro aponta para o interior do salão.
        var obras = new (string titulo, string artista, int ano, string desc,
                         Vector3 pos, Quaternion rot)[]
        {
            // Parede Norte (z ≈ +9.85) — frente aponta para sul → rot Y=180°
            ("Natureza e Código",    "A. Vitor",    2024,
             "Florestas digitais onde pixels brotam como folhas — fusão entre o orgânico e o virtual.",
             new Vector3(-4.5f, 2.2f,  9.85f), Quaternion.Euler(0f,  180f, 0f)),

            ("Horizonte Binário",    "M. Santos",   2025,
             "Pôr do sol fragmentado em zeros e uns — a fronteira entre o real e o simulado.",
             new Vector3( 4.5f, 2.2f,  9.85f), Quaternion.Euler(0f,  180f, 0f)),

            // Parede Leste (x ≈ +9.85) — frente aponta para oeste → rot Y=-90°
            ("O Sonho do Metaverso", "F. Lima",     2025,
             "Viagem onírica por mundos virtuais, explorando identidade e pertencimento digital.",
             new Vector3( 9.85f, 2.2f, 5.5f), Quaternion.Euler(0f, -90f, 0f)),

            ("Reflexos do Futuro",   "C. Ramos",    2024,
             "Espelhos digitais que fragmentam e reconstroem a percepção do observador.",
             new Vector3( 9.85f, 2.2f, 1.0f), Quaternion.Euler(0f, -90f, 0f)),

            // Parede Oeste (x ≈ -9.85) — frente aponta para leste → rot Y=+90°
            ("Código e Alma",        "L. Ferreira", 2025,
             "Linhas de código transcritas como pintura — o que é ser humano na era digital?",
             new Vector3(-9.85f, 2.2f, 5.5f), Quaternion.Euler(0f,  90f, 0f)),

            ("Geometria Sensível",   "R. Mendes",   2024,
             "Formas geométricas que pulsam ao ritmo de dados — arte generativa em estado puro.",
             new Vector3(-9.85f, 2.2f, 1.0f), Quaternion.Euler(0f,  90f, 0f)),
        };

        foreach (var o in obras)
        {
            var root = new GameObject($"Exibito_{o.titulo.Replace(" ", "_")}");
            root.transform.SetParent(raiz.transform);
            root.transform.position = o.pos;
            root.transform.rotation = o.rot;

            // Estrutura do quadro — tudo em coordenadas locais; local Z+ aponta para o interior do salão.
            // Moldura dourada encostada na parede (z local levemente negativo)
            Cubo(root, "Moldura",
                 new Vector3(0f, 0f, -0.01f),
                 new Vector3(1.72f, 1.27f, 0.05f), frameMat);

            // Tela (canvas branca pronta para receber imagem do asset do usuário)
            var tela = Cubo(root, "Tela",
                            new Vector3(0f, 0f, 0.04f),
                            new Vector3(1.50f, 1.05f, 0.02f), m.tela);
            tela.AddComponent<ExibitoView>();

            // Gancho/prego de parede (visual)
            Cubo(root, "Gancho",
                 new Vector3(0f, 0.68f, -0.02f),
                 new Vector3(0.05f, 0.05f, 0.07f), m.suporte);

            // Spot direcionado à tela (local: à frente e acima, apontando para z-)
            var lObj = Filho(root, "ExhibitLight");
            lObj.transform.localPosition = new Vector3(0f, 1.5f, 1.2f);
            lObj.transform.localRotation = Quaternion.Euler(45f, 180f, 0f);
            var spot = lObj.AddComponent<Light>();
            spot.type = LightType.Spot;
            spot.color = new Color(1f, 0.97f, 0.90f);
            spot.intensity = 1.8f; spot.range = 6f; spot.spotAngle = 35f;

            // Painel de informação (Canvas WorldSpace, inativo até hover)
            CriarPainelInfo(root, o.titulo);

            // Controladores
            var ctrl = root.AddComponent<ExibitoController>();
            var so   = new SerializedObject(ctrl);
            so.FindProperty("titulo").stringValue       = o.titulo;
            so.FindProperty("artista").stringValue      = o.artista;
            so.FindProperty("ano").intValue             = o.ano;
            so.FindProperty("descricao").stringValue    = o.desc;
            so.FindProperty("pontosInteracao").intValue = 10;
            so.ApplyModifiedPropertiesWithoutUndo();

            root.AddComponent<PainelInfoController>();
            EditorUtility.SetDirty(root);
        }
    }

    static void CriarPainelInfo(GameObject pai, string titulo)
    {
        var canvasObj = Filho(pai, "Painel_Info");
        canvasObj.transform.localPosition = new Vector3(0f, -0.85f, 0.12f);
        canvasObj.transform.localScale    = Vector3.one * 0.004f;

        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        var bg  = Filho(canvasObj, "Fundo");
        var img = bg.AddComponent<Image>();
        img.color = new Color(0.05f, 0.05f, 0.05f, 0.85f);
        img.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 200);

        var txtObj = Filho(canvasObj, "Texto_Info");
        var tmp    = txtObj.AddComponent<TextMeshProUGUI>();
        tmp.text = titulo; tmp.fontSize = 22;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 185);

        canvasObj.AddComponent<PainelInfoView>();
        canvasObj.SetActive(false);
    }

    // ── 7. Interactables: validador de ingresso na ante-sala + porta ─────────
    static void CriarInteractables(Mats m)
    {
        var raiz = new GameObject("[--- INTERACTABLES ---]");

        // ── Validador de Ingresso (kiosque na ante-sala) ──────────────────────
        var kiosque = Filho(raiz, "Validador_Ingresso");
        kiosque.transform.position = new Vector3(3f, 0f, -8.5f);

        // Coluna do suporte
        Cubo(kiosque, "Suporte_Coluna",
             new Vector3(0f, 0.5f, 0f), new Vector3(0.35f, 1.0f, 0.35f), m.suporte);

        // Plataforma no topo
        Cubo(kiosque, "Suporte_Topo",
             new Vector3(0f, 1.06f, 0f), new Vector3(0.44f, 0.07f, 0.44f), m.suporte);

        // Botão (esfera achatada — azul standby)
        var btnObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        btnObj.name = "Botao_Ingresso";
        btnObj.transform.SetParent(kiosque.transform);
        btnObj.transform.localPosition = new Vector3(0f, 1.21f, 0f);
        btnObj.transform.localScale    = new Vector3(0.22f, 0.10f, 0.22f);
        btnObj.GetComponent<Renderer>().sharedMaterial = m.botao;
        btnObj.AddComponent<BotaoPrincipalController>();
        btnObj.AddComponent<BotaoPrincipalView>();

        // Canvas com label "VALIDAR INGRESSO" acima do kiosque
        var labelCanvas = Filho(kiosque, "Label_Ingresso");
        labelCanvas.transform.localPosition = new Vector3(0f, 1.90f, 0f);
        labelCanvas.transform.localScale    = Vector3.one * 0.006f;
        var lc = labelCanvas.AddComponent<Canvas>();
        lc.renderMode = RenderMode.WorldSpace;
        labelCanvas.AddComponent<CanvasScaler>();
        labelCanvas.AddComponent<GraphicRaycaster>();
        var ltxt = Filho(labelCanvas, "Texto_Label").AddComponent<TextMeshProUGUI>();
        ltxt.text = "VALIDAR\nINGRESSO";
        ltxt.fontSize = 26;
        ltxt.color = new Color(0.9f, 0.85f, 0.5f);
        ltxt.alignment = TextAlignmentOptions.Center;
        ltxt.fontStyle = FontStyles.Bold;
        ltxt.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 130);

        // ── Porta entre ante-sala e salão ─────────────────────────────────────
        // Pivot na dobradiça esquerda do vão (x = -1.5, z = Z_DIVISOR).
        // O painel filho está no centro local (1.5, 1.5, 0) → cobre o vão 3×3 m exatamente.
        var pivot = Filho(raiz, "Porta_Galeria_Pivot");
        pivot.transform.position = new Vector3(-W_PORTA / 2f, 0f, Z_DIVISOR);

        var porta = pivot.AddComponent<PortaAnimadaController>();

        // Desabilita abertura automática por proximidade — só abre pelo botão
        var soPorta = new SerializedObject(porta);
        var propAuto = soPorta.FindProperty("autoAbrirProximidade");
        if (propAuto != null)
        {
            propAuto.boolValue = false;
            soPorta.ApplyModifiedPropertiesWithoutUndo();
        }

        // Painel da porta: localPos (1.5, 1.5, 0) = centro do vão em coordenadas mundiais
        Cubo(pivot, "Painel_Porta",
             new Vector3(W_PORTA / 2f, H_PORTA / 2f, 0f),
             new Vector3(W_PORTA, H_PORTA, 0.08f), m.parede);

        EditorUtility.SetDirty(raiz);
    }

    // ── 8. Auto-wire de todas as referências ─────────────────────────────────
    static void ConectarReferencias()
    {
        // GaleriaManager → HUDView
        var gm  = Object.FindFirstObjectByType<GaleriaManager>();
        var hud = Object.FindFirstObjectByType<HUDView>();
        if (gm && hud) { gm.hudView = hud; EditorUtility.SetDirty(gm); }

        // HUDView → TMP texts
        if (hud)
        {
            var tp = GameObject.Find("Texto_Pontuacao");
            var tm = GameObject.Find("Texto_Mensagem");
            if (tp) hud.textoPontuacao = tp.GetComponent<TextMeshProUGUI>();
            if (tm) hud.textoMensagem  = tm.GetComponent<TextMeshProUGUI>();
            EditorUtility.SetDirty(hud);
        }

        // PlayerController → MainCamera
        var pc  = Object.FindFirstObjectByType<PlayerController>();
        var cam = Camera.main;
        if (pc && cam) { pc.referenciaCamera = cam; EditorUtility.SetDirty(pc); }

        // BotaoPrincipalController → PortaAnimadaController
        var btn  = Object.FindFirstObjectByType<BotaoPrincipalController>();
        var door = Object.FindFirstObjectByType<PortaAnimadaController>();
        if (btn && door)
        {
            var so = new SerializedObject(btn);
            so.FindProperty("portaGaleria").objectReferenceValue = door;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(btn.gameObject);
        }

        // Wiring de cada ExibitoController
        foreach (var ex in Object.FindObjectsByType<ExibitoController>(FindObjectsSortMode.None))
        {
            var pic = ex.GetComponent<PainelInfoController>();
            if (pic == null) continue;

            var soEx = new SerializedObject(ex);
            soEx.FindProperty("painelInfo").objectReferenceValue = pic;
            soEx.ApplyModifiedPropertiesWithoutUndo();

            var piv = ex.GetComponentInChildren<PainelInfoView>(true);
            if (piv != null)
            {
                var soPic = new SerializedObject(pic);
                soPic.FindProperty("view").objectReferenceValue = piv;
                soPic.ApplyModifiedPropertiesWithoutUndo();

                var tmp = piv.GetComponentInChildren<TextMeshProUGUI>(true);
                if (tmp != null)
                {
                    var soPiv = new SerializedObject(piv);
                    soPiv.FindProperty("textoInfo").objectReferenceValue = tmp;
                    soPiv.ApplyModifiedPropertiesWithoutUndo();
                }
                EditorUtility.SetDirty(piv.gameObject);
            }
            EditorUtility.SetDirty(ex.gameObject);
        }

        Debug.Log("[MuseumBuilder] Referências conectadas automaticamente.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static GameObject Filho(GameObject pai, string nome)
    {
        var g = new GameObject(nome);
        g.transform.SetParent(pai.transform, false);
        return g;
    }

    static GameObject Filho(Transform pai, string nome)
    {
        var g = new GameObject(nome);
        g.transform.SetParent(pai, false);
        return g;
    }

    static void Plane(GameObject pai, string nome, Vector3 pos, Quaternion rot, Vector3 scale, Material mat)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Plane);
        g.name = nome;
        g.transform.SetParent(pai.transform);
        g.transform.position   = pos;
        g.transform.rotation   = rot;
        g.transform.localScale = scale;
        g.GetComponent<Renderer>().sharedMaterial = mat;
    }

    static void Parede(GameObject pai, string nome, Vector3 pos, Vector3 scale, Material mat)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = nome;
        g.transform.SetParent(pai.transform);
        g.transform.position   = pos;
        g.transform.localScale = scale;
        g.GetComponent<Renderer>().sharedMaterial = mat;
    }

    static GameObject Cubo(GameObject pai, string nome, Vector3 lpos, Vector3 lscl)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = nome;
        g.transform.SetParent(pai.transform);
        g.transform.localPosition = lpos;
        g.transform.localScale    = lscl;
        return g;
    }

    static GameObject Cubo(GameObject pai, string nome, Vector3 lpos, Vector3 lscl, Material mat)
    {
        var g = Cubo(pai, nome, lpos, lscl);
        g.GetComponent<Renderer>().sharedMaterial = mat;
        return g;
    }

    static GameObject TMP(GameObject pai, string nome, Vector2 anchoredPos, string texto, float size)
    {
        var obj = new GameObject(nome);
        obj.transform.SetParent(pai.transform, false);
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = texto; tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        var rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = new Vector2(500, 60);
        return obj;
    }
}
#endif
