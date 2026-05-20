# MetaMuseu — Galeria de Arte Virtual
**Web 3.0 | Residência em TIC 29 — Unidade 1 / Capítulo 3 (Projeto Avançado)**  
Aluno: Filipe Mazon | Prof.: Ana Beatriz

---

## Apresentando o Projeto

O **MetaMuseu** é uma galeria de arte virtual imersiva desenvolvida com Unity 6 e Meta XR SDK, inserida no contexto do Metaverso. O visitante começa na ante-sala, onde interage com o **Validador de Ingresso** — um totem com botão que libera o acesso ao salão principal. No salão, 6 obras de arte são expostas nas paredes para contemplação. A única interação disponível é o acionamento do totem na entrada.

---

## Contexto e Objetivos no Metaverso

O projeto representa um **ambiente cultural e educacional** no Metaverso: democratizar o acesso à arte permitindo que qualquer pessoa visite um museu virtual de alta qualidade sem barreiras geográficas ou financeiras. O ambiente resolve o problema do desengajamento em exposições tradicionais ao tornar cada obra interativa e contextualizada.

---

## Processo de Criação e Dificuldades

O projeto evoluiu a partir do ambiente básico (Trabalho 1), onde existia apenas uma casa com objetos coletáveis. Para o avançado, o ambiente foi completamente retemáticado como museu e a arquitetura MVC foi expandida com novos tipos de objetos: `ExibitoController`, `PortaAnimadaController` e `PainelInfoController`.

**Maiores desafios:**
- **Emissão URP em runtime**: materiais em URP precisam ter o keyword `_EMISSION` habilitado explicitamente via script; sem isso, `SetColor("_EmissionColor")` não tem efeito visual.
- **Fade-in do painel**: foi necessário usar `CanvasGroup.alpha` com `Mathf.MoveTowards` pois `Color.Lerp` em um `Image` não suporta transparência sem `CanvasGroup`.
- **GetComponent vs GetComponentInChildren**: ExibitoView está no filho `Quadro`, mas ExibitoController está no root do exibito. Corrigido usando `GetComponentInChildren<ExibitoView>(true)`.
- **Ausência de headset**: todos os testes foram feitos com movimentação por teclado (WASD) e XR Device Simulator — a escala dos objetos foi calibrada visualmente no Editor.

---

## Estrutura do Projeto

```
MeuProjeto/
├── Assets/
│   ├── Scripts/
│   │   ├── GaleriaManager.cs            ← Singleton central (MVC)
│   │   ├── PlayerController.cs          ← Movimentação WASD + detecção por proximidade
│   │   ├── Models/
│   │   │   ├── ExibitoModel.cs          ← Dados de cada obra de arte
│   │   │   ├── JogadorModel.cs          ← Pontuação, obras visitadas, tempo de sessão
│   │   │   └── AmbienteModel.cs         ← Estado global do museu
│   │   ├── Views/
│   │   │   ├── HUDView.cs               ← HUD World Space (auto-limpa mensagens)
│   │   │   ├── ExibitoView.cs           ← Emissão URP + pulso verde ao ativar obra
│   │   │   ├── PainelInfoView.cs        ← Canvas 3D com fade-in (CanvasGroup)
│   │   │   ├── ObjetoColetavelView.cs   ← Rotação + encolhimento ao coletar
│   │   │   └── BotaoPrincipalView.cs    ← Troca de cor (normal / hover / ativo)
│   │   └── Controllers/
│   │       ├── ExibitoController.cs     ← Interação com obras (XR + proximidade)
│   │       ├── PainelInfoController.cs  ← Exibe painel, auto-fecha em 6s
│   │       ├── PortaAnimadaController.cs← Animação Quaternion.Lerp, toggle
│   │       ├── ObjetoColetavelController.cs
│   │       └── BotaoPrincipalController.cs
│   ├── Editor/
│   │   └── MuseumBuilder.cs             ← Menu MetaMuseu/► Construir Cena Completa
│   ├── Scenes/                          ← scene1.unity (galeria principal)
│   └── Materials/
│       └── Museum/                      ← Materiais URP gerados pelo MuseumBuilder
├── Packages/manifest.json               ← Meta XR SDK v201 + XR Interaction Toolkit 3.0.7
├── ProjectSettings/
├── .gitignore
├── Relatorio_Tecnico_FilipeMazon_Avancado.txt
└── README.md
```

---

## Arquitetura MVC

| Camada | Arquivos | Responsabilidade |
|--------|----------|-----------------|
| **Model** | `Models/*.cs` | Dados puros, sem MonoBehaviour |
| **View** | `Views/*.cs` | Visual (emissão, fade, cor) — zero lógica |
| **Controller** | `Controllers/*.cs`, `GaleriaManager`, `PlayerController` | Input, lógica, coordenação |

```
Proximidade detectada por PlayerController (Physics.OverlapSphere)
       ↓
  ExibitoController.AoEntrarHover()
       ↓
  ExibitoController.Ativar()  (apenas na primeira visita)
       ├──view.AtivarDestaque()──────────▶ ExibitoView   (pulso de emissão verde)
       ├──painelInfo.ExibirInfo()────────▶ PainelInfoController → PainelInfoView (fade-in)
       └──GaleriaManager.Registrar()────▶ JogadorModel → HUDView (pontuação + mensagem)
```

---

## Hierarquia da Cena

```
[--- MANAGEMENT ---]
  └─ GaleriaManager          (GaleriaManager.cs — Singleton)
  └─ EventSystem             (EventSystem + StandaloneInputModule)
  └─ HUD_Canvas              (Canvas World Space + HUDView.cs)
        ├─ Texto_Pontuacao   (TextMeshProUGUI)
        └─ Texto_Mensagem    (TextMeshProUGUI)

[--- PLAYER ---]
  └─ XROrigin                (CharacterController + PlayerController | tag: Player)
        └─ CameraOffset      (localPos 0, 1.7, 0)
              └─ MainCamera  (Camera + AudioListener | tag: MainCamera)

[--- ENVIRONMENT ---]
  └─ Chao                    (Plane 20×20 m)
  └─ Teto                    (Plane invertido, y=5)
  └─ Parede_Norte            (Cube 20.6×5×0.3)
  └─ Parede_Leste / Oeste    (Cube 0.3×5×20)
  └─ Parede_Sul_Esq/Dir/Topo (3 cubos formando parede com vão de entrada)
  └─ Directional_Light       (luz direcional âmbar)

[--- EXIBITOS ---]
  └─ Exibito_*               (ExibitoController + PainelInfoController)
        ├─ Moldura           (Cube 1.72×1.27×0.05 — borda dourada da obra)
        ├─ Tela              (Cube 1.50×1.05×0.02 — ExibitoView + XRSimpleInteractable)
        ├─ Gancho            (Cylinder — suporte visual na parede)
        ├─ ExhibitLight      (Light Spot — ilumina a obra)
        └─ Painel_Info       (Canvas World Space + PainelInfoView — inativo até hover)
              ├─ Fundo       (Image semitransparente)
              └─ Texto_Info  (TextMeshProUGUI)

[--- INTERACTABLES ---]
  └─ Porta_Galeria_Pivot     (PortaAnimadaController — pivô da dobradiça, abre via botão)
        └─ Painel_Porta      (Cube — folha da porta animada)
  └─ Validador_Ingresso      (kiosque na ante-sala)
        ├─ Suporte_Coluna    (Cylinder)
        ├─ Suporte_Topo      (Cube)
        ├─ Botao_Ingresso    (Sphere azul + BotaoPrincipalController + BotaoPrincipalView)
        └─ Label             (Canvas — "Validar Ingresso")
```

---

## Requisitos Técnicos

- **Unity 6** (versão: 6000.3.14f1)
- **Meta XR SDK Core / Interaction / OVR** v201.0.0
- **XR Interaction Toolkit** v3.0.7
- **TextMeshPro** v3.0.6 (incluso no Unity 6)
- **Universal Render Pipeline** v17.0.3
- **Android Build Support** + Android SDK/NDK (para build no Quest)

---

## Setup Passo a Passo

### 1. Abrir o Projeto
1. Abra o **Unity Hub** com o editor **6000.3.14f1**
2. **Open** → selecione a pasta `MeuProjeto/`
3. Aguarde a importação (~5 min na 1ª abertura)

### 2. Construir a Cena
No Unity Editor: **MetaMuseu → ► Construir Cena Completa**

O `MuseumBuilder.cs` cria toda a hierarquia automaticamente e conecta todas as referências. **Salve com Ctrl+S** após executar.

### 3. Configurar XR Plugin Management
1. **Edit → Project Settings → XR Plugin Management**
2. Aba **PC**: ✅ OpenXR + Meta Quest Feature Group
3. Aba **Android**: ✅ OpenXR + Meta Quest Feature Group

### 4. Build Settings para Android (Meta Quest)
1. **File → Build Settings → Android → Switch Platform**
2. **Player Settings → Other Settings:**
   - Minimum API: **Android 10 (API 29)**
   - Target API: **Android 12 (API 31)**
   - Texture Compression: **ASTC**
   - Scripting Backend: **IL2CPP**
   - Target Architectures: ✅ **ARM64**

### 5. (Opcional) XRSimpleInteractable para uso com headset
Para cada `Exibito_*` → no `Quadro` filho:
1. **Add Component → XRSimpleInteractable**
2. **Hover Entered → ExibitoController / OnXRHoverEntered**
3. **Hover Exited → ExibitoController / OnXRHoverExited**

---

## Teste no Editor (sem headset)

| Ação | Controle |
|------|---------|
| Mover pelo museu | WASD ou setas |
| Interagir com obra (hover automático) | Aproximar do quadro |
| Ativar modo XR Simulator | Window → XR → OpenXR → Meta XR Simulator → Enable |

---


*Web 3.0 | Residência em TIC 29 — 2026 | Filipe Mazon*
