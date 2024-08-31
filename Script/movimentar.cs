using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class movimentar : NetworkBehaviour
{
    public NetworkVariable<ulong> miraId = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> movimentoId = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> moverid = new NetworkVariable<ulong>();
    private movimentar movimento;  // Referência local ao script 'movimentar'
    public Joystick mover; // Atualize para Joystick
    public GameObject[] instanciadeguerreiros, jogadores;
    public Transform warriorfathertransform;
    public NetworkRigidbody2D corpoRigido;
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
        StartCoroutine(aguardandomover());
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
        }
    }

    private IEnumerator aguardandomover()
    {
        while(mover == null)
        {
            yield return null;
        }
        jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject jogador in jogadores)
        {
            if(jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                int indice = 1;
                warriorfathertransform = jogador.transform.Find("warrior's father(Clone)");
                if(warriorfathertransform != null)
                {
                    for(int contador=0; contador<warriorfathertransform.childCount;contador++)
                    {
                        if(!warriorfathertransform.GetChild(contador).name.Contains("balao"))
                        {
                            Array.Resize(ref instanciadeguerreiros, indice);
                            instanciadeguerreiros[indice-1] = warriorfathertransform.GetChild(contador).gameObject;
                            indice++;
                        }
                    }
                }
            }
            foreach(GameObject instanciadeguerreiro in instanciadeguerreiros)
            {
                corpoRigido = instanciadeguerreiro.GetComponent<NetworkRigidbody2D>();
                // Agora execute a movimentação e rotação, uma vez que todas as verificações foram feitas
                if (instanciadeguerreiro.GetComponent<movimentar>().enabled == true)
                {
                    mover_character();
                    RotacionarPersonagem(); // Rotacionar o personagem seguindo o joystick
                }
                else
                {
                    movercamera();
                }
            }
        }
    }
}