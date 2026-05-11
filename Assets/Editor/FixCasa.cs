#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class FixCasa : EditorWindow
{
    [MenuItem("Tools/Fix Casa")]
    public static void Fix()
    {
        // Pegar shader do pipeline ativo
        Shader shader = null;
        var rp = GraphicsSettings.defaultRenderPipeline;
        if (rp != null)
        {
            // URP — pegar via nome exato que o Unity usa internamente
            shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
        }
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) { Debug.LogError("❌ Nenhum shader encontrado!"); return; }
        Debug.Log("✅ Shader: " + shader.name);

        System.IO.Directory.CreateDirectory("Assets/Materials");

        Material CriarMat(string nome, Color cor)
        {
            string path = $"Assets/Materials/{nome}.mat";
            var mat = new Material(shader);
            mat.name = nome;
            mat.color = cor;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", cor);
            if (mat.HasProperty("_Color"))     mat.SetColor("_Color",     cor);
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        // Deletar materiais antigos primeiro
        foreach (var guid in AssetDatabase.FindAssets("t:Material", new[]{"Assets/Materials"}))
        {
            var p = AssetDatabase.GUIDToAssetPath(guid);
            AssetDatabase.DeleteAsset(p);
        }

        var matParede   = CriarMat("Mat_Parede",   new Color(0.85f, 0.82f, 0.75f));
        var matTeto     = CriarMat("Mat_Teto",     new Color(0.70f, 0.68f, 0.65f));
        var matPorta    = CriarMat("Mat_Porta",    new Color(0.45f, 0.25f, 0.10f));
        var matManeta   = CriarMat("Mat_Macaneta", new Color(1.00f, 0.85f, 0.10f));
        var matInterno  = CriarMat("Mat_Interno",  new Color(0.55f, 0.45f, 0.35f));
        var matChao     = CriarMat("Mat_Chao",     new Color(0.30f, 0.60f, 0.30f));
        var matChave    = CriarMat("Mat_Chave",    new Color(1.00f, 0.80f, 0.00f));
        var matCristal  = CriarMat("Mat_Cristal",  new Color(0.00f, 0.80f, 1.00f));
        var matMoeda    = CriarMat("Mat_Moeda",    new Color(0.70f, 0.00f, 0.80f));
        var matBotao    = CriarMat("Mat_Botao",    new Color(1.00f, 0.30f, 0.10f));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        void AplicarMat(Renderer r, Material m)
        {
            if (r && m) { r.sharedMaterial = m; EditorUtility.SetDirty(r.gameObject); }
        }

        // ── Casa ─────────────────────────────────────────────────
        var casa = GameObject.Find("Casa");
        if (casa != null)
        {
            casa.transform.position   = new Vector3(0f, 0f, 8f);
            casa.transform.localScale = Vector3.one;
            foreach (var rend in casa.GetComponentsInChildren<Renderer>())
            {
                string n = rend.gameObject.name;
                if      (n.Contains("Parede"))     AplicarMat(rend, matParede);
                else if (n.Contains("Teto"))       AplicarMat(rend, matTeto);
                else if (n.Contains("Porta_Mesh")) AplicarMat(rend, matPorta);
                else if (n.Contains("Macaneta"))   AplicarMat(rend, matManeta);
                else if (n.Contains("Chao"))       AplicarMat(rend, matInterno);
            }
            EditorUtility.SetDirty(casa);
            Debug.Log("✅ Casa corrigida");
        }

        // ── Plane_Chao ───────────────────────────────────────────
        var chao = GameObject.Find("Plane_Chao");
        if (chao != null)
        {
            chao.transform.position   = Vector3.zero;
            chao.transform.localScale = new Vector3(3f, 1f, 3f);
            AplicarMat(chao.GetComponent<Renderer>(), matChao);
            EditorUtility.SetDirty(chao);
            Debug.Log("✅ Plane_Chao corrigido");
        }

        // ── Coletáveis ───────────────────────────────────────────
        Material[] matsCol = { matChave, matCristal, matMoeda };
        Vector3[] posCol = {
            new Vector3(-2f, 0.5f, 3f),
            new Vector3( 0f, 0.5f, 3f),
            new Vector3( 2f, 0.5f, 3f),
        };
        for (int i = 1; i <= 3; i++)
        {
            var go = GameObject.Find($"Objeto_Coletavel_0{i}");
            if (go == null) continue;
            go.transform.position   = posCol[i-1];
            go.transform.localScale = Vector3.one * 0.3f;
            AplicarMat(go.GetComponent<Renderer>(), matsCol[i-1]);
            EditorUtility.SetDirty(go);
            Debug.Log($"✅ Objeto_Coletavel_0{i} corrigido");
        }

        // ── Botao_Principal ──────────────────────────────────────
        var botao = GameObject.Find("Botao_Principal");
        if (botao != null)
        {
            botao.transform.position   = new Vector3(0f, 0.9f, 6f);
            botao.transform.localScale = new Vector3(0.4f, 0.15f, 0.4f);
            AplicarMat(botao.GetComponent<Renderer>(), matBotao);
            EditorUtility.SetDirty(botao);
            Debug.Log("✅ Botao_Principal corrigido");
        }

        // ── HUD_Canvas ───────────────────────────────────────────
        var hud = GameObject.Find("HUD_Canvas");
        if (hud != null)
        {
            hud.transform.position   = new Vector3(0f, 1.8f, 3f);
            hud.transform.rotation   = Quaternion.identity;
            hud.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
            EditorUtility.SetDirty(hud);
            Debug.Log("✅ HUD_Canvas corrigido");
        }

        // ── XROrigin ─────────────────────────────────────────────
        var xr = GameObject.Find("XROrigin");
        if (xr != null) { xr.transform.position = Vector3.zero; EditorUtility.SetDirty(xr); }

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"✅ Fix completo! Shader usado: {shader.name} — Salve com Ctrl+S.");
    }
}
#endif
