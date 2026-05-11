=====================================================
  MEU PRIMEIRO AMBIENTE VR
  Web 3.0 | Residência em TIC 29 — Unidade 1 / Capítulo 3
  Aluno: Filipe Mazon | Prof.: Ana Beatriz
=====================================================

DESCRIÇÃO
---------
Ambiente VR interativo criado em Unity 6 com Meta XR SDK.
O jogador navega por um cenário externo com uma casa, coleta
objetos e interage com um botão, tudo controlado pelo teclado
no Editor (sem necessidade de headset para testar).

REPOSITÓRIO GITHUB
------------------
https://github.com/f123r4/MeuAmvienteVR

REQUISITOS PARA RODAR
---------------------
- Unity 6000.3.14f1 (ou superior)
- Meta XR SDK (instalado via manifest.json)
- Plataforma: Android (Meta Quest) ou PC (Editor)

COMO ABRIR O PROJETO
--------------------
1. Clone o repositório:
   git clone https://github.com/f123r4/MeuAmvienteVR

2. Abra o Unity Hub
3. Clique em "Add" e selecione a pasta do projeto
4. Abra a cena: Assets/Scenes/scene1.unity
5. Pressione Play para testar no Editor

CONTROLES (TECLADO/MOUSE)
--------------------------
W / Seta Cima    → Mover para frente
S / Seta Baixo   → Mover para trás
A / Seta Esq     → Mover para esquerda
D / Seta Dir     → Mover para direita
E ou Clique      → Ativar botão (ao se aproximar)

MECÂNICAS
---------
- Aproximar dos cubos coloridos → coletar (+pontos)
- Aproximar do botão vermelho   → hover (amarelo)
- Pressionar E perto do botão   → ativar (verde)
- HUD mostra pontuação e mensagens em tempo real

HIERARQUIA DA CENA
------------------
[--- MANAGEMENT ---]
  GameManager       → controle de pontuação
  EventSystem       → sistema de eventos UI
  HUD_Canvas        → interface do jogador (World Space)
    Texto_Pontuacao
    Texto_Objetos
    Texto_Mensagem

[--- PLAYER ---]
  XROrigin (OVRCameraRig) → câmera e movimentação VR/PC

[--- ENVIRONMENT ---]
  Plane_Chao        → terreno navegável
  Directional Light → iluminação principal
  Casa              → ambiente principal
    Paredes
    Porta
      Macaneta

[--- INTERACTABLES ---]
  Objeto_Coletavel_01 (Chave   — 10 pts)
  Objeto_Coletavel_02 (Cristal — 20 pts)
  Objeto_Coletavel_03 (Moeda   — 30 pts)
  Botao_Principal    → interação por proximidade

ESTRUTURA DE PASTAS
-------------------
Assets/
  Scripts/    → GameManager, PlayerController, HUDView,
                ObjetoColetavelController/View,
                BotaoPrincipalController/View
  Editor/     → Scripts utilitários de construção da cena
  Materials/  → Materiais coloridos dos objetos
  Scenes/     → scene1.unity
  Prefabs/    → Prefabs do Meta XR SDK
ProjectSettings/
Packages/

CONFIGURAÇÃO DE BUILD
---------------------
- Platform: Android
- Minimum API Level: Android 10 (Level 29)
- Texture Compression: ASTC
- XR Plugin: OVR (Meta/Oculus)

=====================================================
