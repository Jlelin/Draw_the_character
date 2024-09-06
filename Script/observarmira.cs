using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class observarmira : NetworkBehaviour
{
    private static Transform guerreirocinemachine;
    public static GameObject mira;
    private static Transform guerreiroPrincipal;
    public Vector3 posicaodamira; // Referência ao transform do guerreiro principal
    public static warrior_function warriorfunction;
    public static DragCentralButton arrastarbotaocentral;

    private float initialAngle = 90f; // Ângulo inicial do guerreiro principal (olhando para a direita)

    void Awake()
    {
        mira_ativada.bancoposicaomira += guardarposicaomira;
        arrastarbotaocentral = FindFirstObjectByType<DragCentralButton>();
    }

    void Start()
    {

    }

    void Update()
    {
        StartCoroutine(aguardandomiralocal());
        StartCoroutine(aguardandomira());
    }

    public void guardarposicaomira()
    {
        mira.transform.position = gunbow.miras;
        posicaodamira = mira.transform.position;
    }

    public static void atribuirvalorawarriorfunctionviadragcentralbutton()
    {
        warriorfunction = arrastarbotaocentral.warrior.GetComponent<warrior_function>();
        // Define a rotação inicial do guerreiro principal para olhar para a direita
        guerreirocinemachine = warrior_function.instanciaguerreiro.transform;
        if (warrior_function.cinemachinecamera.Follow == guerreirocinemachine)
        {
            guerreiroPrincipal = guerreirocinemachine;
            // Define a rotação inicial para olhar para a direita
            guerreiroPrincipal.rotation = Quaternion.Euler(new Vector3(0, 0, -90)); // Olhando para a direita
        }
    }

    private IEnumerator aguardandomira()
    {
        while(mira == null)
        {
            yield return null;
        }
        // Verifica se a mira está ativada
        if (mira.activeSelf)
        {
            // Verifica se o guerreiro principal foi configurado
            if (guerreiroPrincipal != null)
            {
                
                // Calcula a direção da mira em relação ao guerreiro principal
                Vector3 direction = mira.transform.position - guerreiroPrincipal.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Ajusta o ângulo calculado considerando a rotação inicial
                float adjustedAngle = angle - initialAngle;
                if(posicaodamira != mira.transform.position)
                {
                    // Define a rotação do guerreiro para olhar na direção da mira
                    guerreiroPrincipal.rotation = Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
                }
            }
        }
    }
    
    private IEnumerator aguardandomiralocal()
    {
        if(mira != null)
        {
            if(mira.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId)
            {
                mira = null;
            }
        }
        while(mira == null)
        {
            GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject jogador in jogadores)
            {
                if(jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    mira = jogador.transform.Find("mira_0(Clone)")?.gameObject;
                }
            }
            yield return null;
        }
    }
}
