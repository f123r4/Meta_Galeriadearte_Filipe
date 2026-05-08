# Meu Ambiente VR
**Web 3.0 | Residência em TIC 29 — Unidade 1 / Capítulo 3**  
Aluno: Filipe Mazon | Prof.: Ana Beatriz

Experiência em Realidade Virtual desenvolvida com Unity 2022 LTS e Meta XR SDK.  
O jogador navega por um ambiente 3D, coleta objetos interativos e acumula pontuação no HUD.

---

## Estrutura do Projeto

```
MeuProjeto/
├── Assets/
│   ├── Scripts/
│   │   ├── GameManager.cs              ← Singleton central (MVC Controller)
│   │   ├── PlayerController.cs         ← Movimentação XR + teclado
│   │   ├── Models/
│   │   │   ├── AmbienteModel.cs        ← Estado da sessão VR
│   │   │   ├── JogadorModel.cs         ← Pontuação e movimento
│   │   │   └── ObjetoColetavelModel.cs ← Dados de cada objeto
│   │   ├── Views/
│   │   │   ├── HUDView.cs              ← Interface (pontuação, mensagens)
│   │   │   ├── ObjetoColetavelView.cs  ← Animação e som dos objetos
│   │   │   └── BotaoPrincipalView.cs   ← Feedback visual do botão
│   │   └── Controllers/
│   │       ├── ObjetoColetavelController.cs ← Detecção de coleta
│   │       └── BotaoPrincipalController.cs  ← Lógica do botão VR
│   ├── Scenes/          ← Cena principal AmbienteVR.unity
│   ├── Prefabs/         ← Prefabs reutilizáveis
│   └── Materials/       ← Materiais dos objetos
├── Packages/
│   └── manifest.json    ← Meta XR SDK + dependências
├── ProjectSettings/
│   ├── ProjectVersion.txt
│   ├── ProjectSettings.asset
│   └── XRSettings.asset
├── .gitignore
└── README.md
```

---

## Arquitetura MVC

| Camada | Arquivo | Responsabilidade |
|--------|---------|-----------------|
| **Model** | `Models/*.cs` | Dados puros sem MonoBehaviour |
| **View** | `Views/*.cs` | Apenas visual e áudio |
| **Controller** | `GameManager.cs`, `PlayerController.cs`, `Controllers/*.cs` | Input, lógica, coordenação |

**Fluxo de dados:**
```
Input (XR / Teclado)
       ↓
  Controller  ──atualiza──▶  Model
       │
       └──aciona──▶  View  (reflete o novo estado)
       │
       └──notifica──▶  GameManager  (pontuação, fim de jogo)
```

---

## Requisitos

- **Unity 6** (versão: 6000.3.14f1)
- **Meta XR All-in-One SDK** v60+
- **TextMeshPro** (incluso no Unity 2022)
- **XR Plugin Management** (incluso no Unity)
- **Android Build Support** + Android SDK/NDK (para build no Quest)

---

## Setup Passo a Passo

### 1. Abrir o Projeto
1. Abra o **Unity Hub** e certifique-se de ter o editor **6000.3.14f1** instalado
2. Clique em **Open** → selecione a pasta `MeuProjeto/`
3. Aguarde a importação dos pacotes (pode demorar na 1ª abertura)

### 2. Instalar o Meta XR SDK
> O `manifest.json` já inclui a dependência. Se não importar automaticamente:
1. **Window → Package Manager → "+" → Add package by name**
2. Nome: `com.meta.xr.sdk.all`

### 3. Configurar XR Plugin Management
1. **Edit → Project Settings → XR Plugin Management**
2. Aba **PC**: marque ✅ **Oculus** (teste no Editor)
3. Aba **Android**: marque ✅ **Oculus** (build para Quest)

### 4. Build Settings para Android (Meta Quest)
1. **File → Build Settings → Android → Switch Platform**
2. **Player Settings → Other Settings:**
   - Minimum API Level: **Android 10 (API 29)**
   - Target API Level: **Android 12 (API 31)**
   - Texture Compression: **ASTC**
   - Scripting Backend: **IL2CPP**
   - Target Architectures: ✅ **ARM64**

### 5. Hierarquia da Cena

Crie os GameObjects com estes nomes exatos:

```
[--- MANAGEMENT ---]
  └─ GameManager          → componente: GameManager.cs
  └─ EventSystem          → componentes: EventSystem + StandaloneInputModule
  └─ HUD_Canvas           → Canvas (World Space) + HUDView.cs
        ├─ Texto_Pontuacao   → TextMeshProUGUI
        ├─ Texto_Objetos     → TextMeshProUGUI
        └─ Texto_Mensagem    → TextMeshProUGUI

[--- PLAYER ---]
  └─ XROrigin             → prefab Meta XR SDK + PlayerController.cs
                             tag: "Player"

[--- ENVIRONMENT ---]
  └─ Plane_Chao           → 3D Object → Plane
  └─ Directional Light
  └─ Casa
        ├─ Paredes
        └─ Porta
              └─ Macaneta

[--- INTERACTABLES ---]
  └─ Objeto_Coletavel_01  → mesh + Collider + ObjetoColetavelView + ObjetoColetavelController
  └─ Objeto_Coletavel_02
  └─ Objeto_Coletavel_03
  └─ Botao_Principal      → mesh + Collider + BotaoPrincipalView + BotaoPrincipalController
```

### 6. Conectar Referências no Inspector

| GameObject | Campo | Arrastar |
|------------|-------|---------|
| `GameManager` | Hud View | `HUD_Canvas` |
| `PlayerController` | Referencia Camera | `Main Camera` |
| `HUDView` | Texto Pontuacao | `Texto_Pontuacao` |
| `HUDView` | Texto Objetos | `Texto_Objetos` |
| `HUDView` | Texto Mensagem | `Texto_Mensagem` |
| `ObjetoColetavelController` | Nome / Pontos | preencher manualmente |

### 7. Conectar XRSimpleInteractable (opcional — interação com controle)

Para cada `Objeto_Coletavel_XX`:
1. Adicione o componente **XRSimpleInteractable**
2. **Select Entered → "+" → ObjetoColetavelController / OnInteracaoXR**

Para `Botao_Principal`:
1. Adicione **XRSimpleInteractable**
2. **HoverEntered → BotaoPrincipalController / AoEntrarHover**
3. **HoverExited → BotaoPrincipalController / AoSairHover**
4. **SelectEntered → BotaoPrincipalController / AoPressionar**

---

## Teste no Editor (sem headset)

| Ação | Controle |
|------|---------|
| Mover jogador | WASD ou setas |
| Coletar objeto | Aproximar o player (trigger por proximidade) |
| Ativar botão | Clique do mouse sobre o objeto |

Para usar o **XR Meta Simulator** (emula controles):  
**Window → XR → OpenXR → Meta XR Simulator → Enable**

---

## Build para Meta Quest

```
1. Conecte o Quest via USB e ative o Modo Desenvolvedor
2. File → Build Settings → Build and Run
3. Aguarde a instalação automática no headset
```

---

## Critérios de Avaliação (referência)

| Critério | Peso |
|----------|------|
| Configuração Técnica (SDK, build, XR Plugin) | 40% |
| Criatividade e Organização da cena | 30% |
| Documentação (README, comentários, relatório) | 20% |
| Funcionamento Geral (sem erros críticos) | 10% |

---

*Web 3.0 | Residência em TIC 29 — 2025*
