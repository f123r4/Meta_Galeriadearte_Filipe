// =============================================================
// PlayerController.cs
// Movimentação do jogador no ambiente VR
//   - Joystick esquerdo do Meta Quest (XR Input)
//   - Teclado WASD como fallback para testes no Editor
// Componente obrigatório: CharacterController
// =============================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using MeuAmbienteVR.Models;

namespace MeuAmbienteVR
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movimento")]
        [SerializeField] private float velocidade     = 3f;
        [SerializeField] private float gravidade      = -9.81f;

        [Header("Câmera / XR Origin")]
        // Arraste o XROrigin (ou Main Camera) para orientar o movimento
        [SerializeField] private Transform referenciaCamera;

        private CharacterController cc;
        private JogadorModel        jogadorModel;
        private float               velocidadeVertical;

        // ── Ciclo de vida ────────────────────────────────────────

        private void Awake()
        {
            cc = GetComponent<CharacterController>();
        }

        private void Start()
        {
            // Obtém o Model do jogador registrado no GameManager
            jogadorModel = GameManager.Instancia?.ObterJogador();

            // Usa Main Camera se nenhuma referência foi atribuída
            if (referenciaCamera == null && Camera.main != null)
                referenciaCamera = Camera.main.transform;
        }

        private void Update()
        {
            Mover();
        }

        // ── Lógica de movimento ──────────────────────────────────

        private void Mover()
        {
            Vector2 eixo = LerEixo();

            // Aplica gravidade enquanto o jogador está no ar
            velocidadeVertical = cc.isGrounded ? -0.5f
                                               : velocidadeVertical + gravidade * Time.deltaTime;

            Vector3 direcaoHorizontal = ProjetarNaCamera(eixo);
            Vector3 deslocamento      = direcaoHorizontal * velocidade;
            deslocamento.y            = velocidadeVertical;

            cc.Move(deslocamento * Time.deltaTime);

            // Informa ao Model se o jogador está em movimento
            jogadorModel?.SetMovimento(eixo.magnitude > 0.05f);
        }

        // Lê o joystick XR; se indisponível usa o teclado
        private Vector2 LerEixo()
        {
            var dispositivos = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, dispositivos);

            if (dispositivos.Count > 0)
            {
                Vector2 joystick;
                if (dispositivos[0].TryGetFeatureValue(CommonUsages.primary2DAxis, out joystick)
                    && joystick.magnitude > 0.05f)
                    return joystick;
            }

            // Fallback: WASD / setas do teclado
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        // Transforma o input 2D na direção 3D relativa à câmera (sem componente vertical)
        private Vector3 ProjetarNaCamera(Vector2 eixo)
        {
            if (referenciaCamera == null)
                return new Vector3(eixo.x, 0f, eixo.y);

            Vector3 frente  = referenciaCamera.forward; frente.y  = 0f; frente.Normalize();
            Vector3 direita = referenciaCamera.right;   direita.y = 0f; direita.Normalize();

            return frente * eixo.y + direita * eixo.x;
        }
    }
}
