#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Marca objetos estáticos do MetaMuseu com ContributeGI e configura o bake.
/// Menu: MetaMuseu ▶ Configurar e Iniciar Bake
/// </summary>
[InitializeOnLoad]
public static class LightingSetup
{
    private static readonly HashSet<string> RAIZES_DINAMICAS = new HashSet<string>
    {
        "Porta_Galeria_Pivot",
        "XROrigin",
        "[--- PLAYER ---]",
    };

    private static readonly HashSet<string> OBJETOS_EXCLUIDOS = new HashSet<string>
    {
        "Painel_Porta",
        "HUD_Canvas",
        "MainCamera",
        "CameraOffset",
        "LeftHand Controller",
        "RightHand Controller",
        "Locomotion System",
    };

    static LightingSetup() { }

    [MenuItem("MetaMuseu/► Configurar e Iniciar Bake", priority = 3)]
    public static void ConfigurarEBake()
    {
        MarcarStaticFlags();
        ConfigurarLightmapper();
        Debug.Log("[LightingSetup] Iniciando bake — aguarde a barra de progresso do Unity.");
        // Unity 6: BakeAsync sem callback; usa evento bakeCompleted
        Lightmapping.bakeCompleted -= OnBakeDone;   // evita duplicação
        Lightmapping.bakeCompleted += OnBakeDone;
        Lightmapping.BakeAsync();
    }

    [MenuItem("MetaMuseu/► Só Marcar Objetos como Static (sem bake)", priority = 4)]
    public static void ApenasMarcar()
    {
        int marcados = MarcarStaticFlags();
        Debug.Log($"[LightingSetup] {marcados} objetos marcados como Lightmap Static. " +
                  "Use Window ▶ Rendering ▶ Lighting ▶ Generate Lighting para bake manual.");
    }

    private static int MarcarStaticFlags()
    {
        int marcados  = 0;
        int ignorados = 0;

        var todosRenderers = Object.FindObjectsByType<MeshRenderer>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var mr in todosRenderers)
        {
            if (EhDinamico(mr.transform))
            {
                ignorados++;
                continue;
            }

            var flags = GameObjectUtility.GetStaticEditorFlags(mr.gameObject);
            var novas = flags
                | StaticEditorFlags.ContributeGI
                | StaticEditorFlags.BatchingStatic
                | StaticEditorFlags.OccludeeStatic
                | StaticEditorFlags.OccluderStatic;

            if (novas != flags)
            {
                GameObjectUtility.SetStaticEditorFlags(mr.gameObject, novas);
                EditorUtility.SetDirty(mr.gameObject);
                marcados++;
            }
        }

        EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"[LightingSetup] Flags estáticas: {marcados} marcados, {ignorados} ignorados.");
        return marcados;
    }

    private static bool EhDinamico(Transform t)
    {
        if (OBJETOS_EXCLUIDOS.Contains(t.name)) return true;
        var cursor = t;
        while (cursor != null)
        {
            if (RAIZES_DINAMICAS.Contains(cursor.name)) return true;
            cursor = cursor.parent;
        }
        return false;
    }

    private static void ConfigurarLightmapper()
    {
        if (!Lightmapping.TryGetLightingSettings(out var ls))
        {
            ls = new LightingSettings();
            Lightmapping.SetLightingSettingsForScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene(), ls);
        }

        ls.lightmapper         = LightingSettings.Lightmapper.ProgressiveCPU;
        ls.lightmapResolution  = 15f;
        ls.lightmapMaxSize     = 1024;
        ls.directSampleCount   = 32;
        ls.indirectSampleCount = 256;
        ls.maxBounces          = 2;
        ls.ao                  = true;
        ls.aoMaxDistance       = 1f;
        ls.autoGenerate        = false;   // bake só sob-demanda

        EditorUtility.SetDirty(ls);
        Debug.Log("[LightingSetup] LightingSettings: Progressive CPU, AO ligado, bake manual.");
    }

    private static void OnBakeDone()
    {
        Lightmapping.bakeCompleted -= OnBakeDone;
        Debug.Log("[LightingSetup] Bake concluído! Salve a cena com Ctrl+S.");
        EditorSceneManager.MarkAllScenesDirty();
    }
}
#endif
