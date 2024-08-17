using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class movimentar : NetworkBehaviour
{
    public NetworkVariable<ulong> miraId = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> movimentoId = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> moverid = new NetworkVariable<ulong>();
    private movimentar movimento;  // Referência local ao script 'movimentar'
    public Joystick mover; // Atualize para Joystick
    public Rigidbody2D corpo_rigido;
    public float velocidade, velocidadeRotacao;
    public float graus; // Variável para armazenar o valor de rotação

    private void Start()
    {
        corpo_rigido = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        // Verifique se o NetworkManager está ativo e se o instanciaguerreiro está definido
        if (NetworkManager.Singleton == null || DragCentralButton.instanciaguerreiro == null) return;

        // Verifique se o instanciaguerreiro tem um NetworkObject antes de acessar seu OwnerClientId
        NetworkObject guerreiroNetworkObject = DragCentralButton.instanciaguerreiro.GetComponent<NetworkObject>();

        if (guerreiroNetworkObject == null) return;

        // Continue apenas se o LocalClientId for igual ao OwnerClientId do guerreiro
        if (NetworkManager.Singleton.LocalClientId != guerreiroNetworkObject.OwnerClientId) return;

        // Verifique se a mira também está definida e tem um NetworkObject antes de continuar
        if (DragCentralButton.instanciamira != null)
        {
            NetworkObject miraNetworkObject = DragCentralButton.instanciamira.GetComponent<NetworkObject>();

            if (miraNetworkObject != null)
            {
                if (NetworkManager.Singleton.LocalClientId != miraNetworkObject.OwnerClientId) return;
            }
        }

        // Agora execute a movimentação e rotação, uma vez que todas as verificações foram feitas
        if (DragCentralButton.instanciaguerreiro.GetComponent<movimentar>().enabled == true)
        {
            mover_character();
            RotacionarPersonagem(); // Rotacionar o personagem seguindo o joystick
        }
        else
        {
            movercamera();
        }
    }


    private void mover_character()
    {
        float moverH = mover.Horizontal;
        float moverV = mover.Vertical;
        Vector2 direcao = new Vector2(moverH, moverV).normalized;

        corpo_rigido.velocity = direcao * velocidade;
    }

    private void movercamera()
    {
        mover_character();
    }

    private GameObject GetMiraObject()
    {
        if (miraId.Value != 0 && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(miraId.Value, out NetworkObject miraObject))
        {
            return miraObject.gameObject;
        }
        return null;
    }

    // Método para rotacionar o personagem suavemente em direção ao joystick
private void RotacionarPersonagem()
{
    // Verifica se o joystick está sendo movido
    if (mover.input != Vector2.zero)
    {
        // Calcula o ângulo desejado com base na direção do joystick
        float angle = Mathf.Atan2(mover.input.y, mover.input.x) * Mathf.Rad2Deg;

        // Corrige o ângulo para alinhar corretamente com a direção do joystick
        angle -= 90f; // Ajuste para corrigir a orientação

        // Atualiza a variável graus com o valor calculado
        graus = angle;

        // Aplica a rotação ao personagem diretamente
        transform.eulerAngles = new Vector3(0, 0, graus);

        // Debug para verificar os valores
        Debug.Log($"Input: {mover.input}, Ângulo: {angle}, Graus: {graus}");
    }
}


}
