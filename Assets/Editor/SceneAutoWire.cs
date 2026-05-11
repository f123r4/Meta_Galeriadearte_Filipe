#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class SceneAutoWire : EditorWindow
{
    [MenuItem("Tools/Auto-Wire Scene References")]
    public static void ConectarReferencias()
    {
        var gm  = Object.FindFirstObjectByType<GameManager>();
        var hud = Object.FindFirstObjectByType<HUDView>();

        if (gm && hud)
        {
            gm.hudView = hud;
            EditorUtility.SetDirty(gm);
            Debug.Log("✅ GameManager → HUDView");
        }

        if (hud)
        {
            var tp = GameObject.Find("Texto_Pontuacao");
            var to = GameObject.Find("Texto_Objetos");
            var tm = GameObject.Find("Texto_Mensagem");
            if (tp) hud.textoPontuacao = tp.GetComponent<TextMeshProUGUI>();
            if (to) hud.textoObjetos   = to.GetComponent<TextMeshProUGUI>();
            if (tm) hud.textoMensagem  = tm.GetComponent<TextMeshProUGUI>();
            EditorUtility.SetDirty(hud);
            Debug.Log("✅ HUDView → textos");
        }

        var pc  = Object.FindFirstObjectByType<PlayerController>();
        var cam = Camera.main;
        if (pc && cam)
        {
            pc.referenciaCamera = cam;
            EditorUtility.SetDirty(pc);
            Debug.Log("✅ PlayerController → Main Camera");
        }

        string[] nomes = { "Chave", "Cristal", "Moeda" };
        int[]    pts   = { 10, 20, 30 };
        for (int i = 0; i < 3; i++)
        {
            var go = GameObject.Find($"Objeto_Coletavel_0{i+1}");
            if (go == null) continue;
            var ctrl = go.GetComponent<ObjetoColetavelController>();
            if (ctrl == null) continue;
            ctrl.nomeObjeto = nomes[i];
            ctrl.pontos     = pts[i];
            EditorUtility.SetDirty(go);
            Debug.Log($"✅ Objeto_Coletavel_0{i+1} → {nomes[i]}");
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("✅ Auto-Wire concluído! Salve com Ctrl+S.");
    }
}
#endif
