#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Garante que cada ExibitoController da cena tenha:
///   1. Um filho "Tela" (Cube branco + ExibitoView) — cria se ausente.
///   2. XRSimpleInteractable no Tela com hover/select conectados ao ExibitoController.
///
/// Roda automaticamente ao compilar (static constructor + delayCall).
/// Menu manual: MetaMuseu ▶ Adicionar XR Interactables nos Quadros
/// </summary>
[InitializeOnLoad]
public static class XRInteractableSetup
{
    private const string SESSION_KEY   = "XRInteractableSetup_Aplicado";
    private const string CANVAS_MAT    = "Assets/Materials/Museum/MuseumCanvas.mat";

    static XRInteractableSetup()
    {
        if (SessionState.GetBool(SESSION_KEY, false)) return;

        EditorApplication.delayCall += () =>
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                Aplicar(silencioso: true);

            SessionState.SetBool(SESSION_KEY, true);
        };
    }

    [MenuItem("MetaMuseu/► Adicionar XR Interactables nos Quadros", priority = 2)]
    public static void AplicarMenu() => Aplicar(silencioso: false);

    public static void Aplicar(bool silencioso = false)
    {
        var exibitos = Object.FindObjectsByType<ExibitoController>(FindObjectsSortMode.None);

        if (exibitos.Length == 0)
        {
            if (!silencioso)
                Debug.LogWarning("[XRInteractableSetup] Nenhum ExibitoController encontrado. " +
                                 "Execute MetaMuseu ▶ Construir Cena Completa primeiro.");
            return;
        }

        var canvasMat = AssetDatabase.LoadAssetAtPath<Material>(CANVAS_MAT);

        int criados    = 0;
        int adicionados = 0;
        int jaExistiam  = 0;

        foreach (var ex in exibitos)
        {
            var tela = ex.transform.Find("Tela");

            // ── Se "Tela" não existe, cria como filho do exibito ─────────────
            if (tela == null)
            {
                var moldura = ex.transform.Find("Moldura");
                if (moldura == null)
                {
                    Debug.LogWarning($"[XRInteractableSetup] '{ex.name}': estrutura inesperada " +
                                     "(sem Moldura nem Tela) — pulando.");
                    continue;
                }

                var telaGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                telaGO.name = "Tela";
                telaGO.transform.SetParent(ex.transform);
                telaGO.transform.localPosition = new Vector3(0f, 0f, 0.04f);
                telaGO.transform.localScale    = new Vector3(1.50f, 1.05f, 0.02f);
                telaGO.transform.localRotation = Quaternion.identity;

                if (canvasMat != null)
                    telaGO.GetComponent<Renderer>().sharedMaterial = canvasMat;

                telaGO.AddComponent<ExibitoView>();

                tela = telaGO.transform;
                EditorUtility.SetDirty(ex.gameObject);
                criados++;
            }

            // ── XRSimpleInteractable ─────────────────────────────────────────
            var go = tela.gameObject;

            if (go.GetComponent<XRSimpleInteractable>() != null)
            {
                jaExistiam++;
                continue;
            }

            var xri = go.AddComponent<XRSimpleInteractable>();

            UnityEventTools.AddVoidPersistentListener(xri.hoverEntered,  ex.AoEntrarHover);
            UnityEventTools.AddVoidPersistentListener(xri.hoverExited,   ex.AoSairHover);
            UnityEventTools.AddVoidPersistentListener(xri.selectEntered, ex.Ativar);

            EditorUtility.SetDirty(go);
            adicionados++;
        }

        if (criados > 0 || adicionados > 0)
        {
            EditorSceneManager.MarkAllScenesDirty();
            Debug.Log($"[XRInteractableSetup] ✅  " +
                      $"Tela criada: {criados} | " +
                      $"XRSimpleInteractable adicionado: {adicionados} | " +
                      $"Já existiam: {jaExistiam}. — Salve com Ctrl+S.");
        }
        else if (!silencioso)
        {
            Debug.Log($"[XRInteractableSetup] Todos os {jaExistiam} quadros já estão " +
                      "completos — nada a fazer.");
        }
    }
}
#endif
