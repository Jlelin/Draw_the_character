using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class gunbow : NetworkBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public NetworkVariable<ulong> avodID = new NetworkVariable<ulong>();
    private int apertado_botao;
    public static int contador;
    public GameObject mira, scriptwarrior, atirar, ataque, jogadorobject;
    public NetworkObject jogador;
    private PolygonCollider2D polygoncolliderenemy;
    private CircleCollider2D circlecolliderenemy;
    public static Vector3 miras, guerreiro;
    private Transform guerreirocinemachine;
    public movimentar movimento;
    public warrior_function warriorfunction;

    public NetworkObject guerreirodowarriorfunction;

    // Start is called before the first frame update
    void Start()
    {
        if(mira == null)
        {
            mira = DragCentralButton.mirainstance;
            if(mira != null)
            {
                miras = mira.transform.position;
            }
        }
        jogador = GetComponentInParent<NetworkObject>();
        if(IsServer)
        {
            avodID.Value = jogador.NetworkObjectId;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (apertado_botao == 1)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                contador++;
                // Define a posição da mira e ativa ela após um frame
                for (int i = 0; i < warriorfunction.guerreiros.Length; i++)
                {
                    if(IsServer)
                    {
                        warrior_function.guerreirosID.Value = warriorfunction.guerreiros[i].GetComponent<NetworkObject>().NetworkObjectId;
                    }
                    guerreirodowarriorfunction = NetworkManager.Singleton.SpawnManager.SpawnedObjects[warrior_function.guerreirosID.Value];
                    movimento = guerreirodowarriorfunction.GetComponent<movimentar>();
                    if (movimento.enabled == true)
                    {
                        movimento.enabled = false;
                        guerreiro = warriorfunction.guerreiros[i].transform.position;
                        miras = guerreiro;
                        mira.transform.position = miras; // Define a posição da mira
                        
                        // Força a atualização da cena
                        StartCoroutine(ActivateMiraAfterUpdate());
                        break;
                    }
                }
            }
        }
        else if (apertado_botao == 2)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                contador = 0;
                mira.SetActive(false);
                for (int i = 0; i < warriorfunction.guerreiros.Length; i++)
                {
                    guerreirocinemachine = warriorfunction.guerreiros[i].transform;
                    if(IsClient)
                    {
                        atribuirvaloraguerreiroIDServerRpc(warriorfunction.guerreiros[i].GetComponent<NetworkObject>().NetworkObjectId);
                    }
                    else
                    {
                        warrior_function.guerreirosID.Value = warriorfunction.guerreiros[i].GetComponent<NetworkObject>().NetworkObjectId;
                    }
                    guerreirodowarriorfunction = NetworkManager.Singleton.SpawnManager.SpawnedObjects[warrior_function.guerreirosID.Value];
                    if (warriorfunction.cinemachinecamera.Follow == guerreirocinemachine)
                    {
                        guerreirodowarriorfunction.GetComponent<movimentar>().enabled = true;
                    }
                }
            }
        }
        if(mira != null)
        {
            if (mira.activeSelf == true)
            {
                /*for (int i = 0; i < enemy.Length; i++)
                {
                    polygoncolliderenemy = enemy[i].GetComponent<PolygonCollider2D>();
                    polygoncolliderenemy.enabled = false;
                    circlecolliderenemy = GetNonTriggerCircleCollider(enemy[i]);
                    circlecolliderenemy.enabled = true;
                }*/
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (contador == 0)
        {
            apertar_botao(1); // Define apertado_botao como 1 quando o ponteiro é pressionado
        }
        else
        {
            apertar_botao(2);
        }
    }

    // Método chamado quando o ponteiro é liberado do GameObject
    public void OnPointerUp(PointerEventData eventData)
    {
        apertar_botao(0); // Define apertado_botao como 0 quando o ponteiro é liberado
    }

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }

    private CircleCollider2D GetNonTriggerCircleCollider(GameObject obj)
    {
        CircleCollider2D[] colliders = obj.GetComponents<CircleCollider2D>();
        CircleCollider2D colliderfinal = null;
        foreach (var collider in colliders)
        {
            if (collider.isTrigger)
            {
                collider.enabled = false;
            }
            else
            {
                colliderfinal = collider;
            }
        }
        return colliderfinal;
    }

    public void receberscriptwarrior()
    {
        warriorfunction = scriptwarrior.GetComponent<warrior_function>();
    }

    private IEnumerator ActivateMiraAfterUpdate()
    {
        yield return null; // Aguarda um frame para garantir que a posição foi atualizada
        mira.SetActive(true); // Ativa a mira após um frame
    }

    [ServerRpc]
    private void atribuirvaloraguerreiroIDServerRpc(ulong guerreiroID)
    {
        warrior_function.guerreirosID.Value = guerreiroID;
    }
}
