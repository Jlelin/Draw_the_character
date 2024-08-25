using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using UnityEngine.EventSystems;
using System.Linq;
using System;


public class warrior_function : NetworkBehaviour
{
    public qual_guerreiro guerreiroesquerdo;
    public qual_guerreirodireito guerreirodireito;
    public GameObject cinemachine, botaofoco, esquerdo, direito, selectwarrior, atirar, proibidoatirar, proibidoatacar;
    public CinemachineVirtualCamera cinemachinecamera;
    public GameObject[] guerreiros, jogadores;
    public GameObject botaodesenho, ataque, mira;
    public Transform[] warriorschild, filhosdewarriorfatherfilhos;
    public Transform filhosdewarriorfather;
    public Rigidbody2D constraints;
    public SpriteRenderer balaorenderer, balaoatual;
    public static NetworkObject instanciaguerreiro;
    public static NetworkVariable<ulong> guerreirosID = new NetworkVariable<ulong>(value: 0, writePerm: NetworkVariableWritePermission.Owner);
    public int orderinlayer, indices = 1;
    public int apertado_botao, guardar_o, guardar_p;
    public bool selecionarguerreiro;
    private int tamanho_vetor; // Mova a declaração para o escopo da classe
    private bool proximo;

    void Awake()
    {
        guerreirodireito = UnityEngine.Object.FindFirstObjectByType<qual_guerreirodireito>();
        guerreiroesquerdo = UnityEngine.Object.FindFirstObjectByType<qual_guerreiro>();
        botaofoco.SetActive(false);
        ataque.SetActive(false);
        atirar.SetActive(false);
        mira.SetActive(false);
        proibidoatacar.SetActive(false);
        proibidoatirar.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        proximo = false;
        tamanho_vetor = guerreiroesquerdo.balao.Length - 1; // Inicialize aqui
        selecionarguerreiro = false;
    }

    // Update is called once per frame
    void Update()
    {
        tamanho_vetor = guerreiroesquerdo.balao.Length - 1;
       if(apertado_botao == 2)
       {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    selecionarguerreiro = true;
                    esquerdo.SetActive(false);
                    direito.SetActive(false);
                    selectwarrior.SetActive(false);
                    botaofoco.SetActive(true);
                    botaodesenho.SetActive(false);
                }
            }
       }
        if(guerreiroesquerdo.apertado_botao == 3)
       {
            balaoatual = guerreiroesquerdo.balao[guerreiroesquerdo.balao_selecionado].GetComponent<SpriteRenderer>();
            for(int k = 0; k < guerreiroesquerdo.balao_diferentesguerreiros.Length; k++)
            {
                if(balaoatual.sprite == guerreiroesquerdo.balao_diferentesguerreiros[k])
                {
                    guardar_p = k;
                }
            }
       }
        if(guerreirodireito.apertado_botao == 4)
        {
            for(int f = 0; f < guerreirodireito.balao_vermelho.Length; f++)
            {
                if(guerreirodireito.balao_branco_elementofinal.sprite == guerreirodireito.balao_vermelho[f])
                {
                    guardar_p = f;
                }
            }
        }
        if(selecionarguerreiro == true)
        {
            jogadores = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject jogadorlocal in jogadores)
            {
                var jogadornetwork = jogadorlocal.GetComponent<NetworkObject>();
                if(jogadornetwork.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    cinemachine.SetActive(true);
                    for(int o = 0; o < guerreiroesquerdo.balao.Length; o++)
                    {
                        guerreiros[o].SetActive(true);
                        instanciaguerreiro = guerreiros[o].GetComponent<NetworkObject>();
                        instanciaguerreiro.enabled = true;
                        if(instanciaguerreiro.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                        {
                            if((instanciaguerreiro.CompareTag("guerreiroarqueiro") && instanciaguerreiro.IsSpawned == true) || (instanciaguerreiro.CompareTag("guerreirosniper") && instanciaguerreiro.IsSpawned == true))
                            {
                                ataque.SetActive(true);
                            }
                            indices = 1;
                            filhosdewarriorfather = jogadorlocal.transform.Find("warrior's father(Clone)");
                            if(filhosdewarriorfather != null)
                            {
                                var indicesfilhos = 1;
                                for(int j=0; j<filhosdewarriorfather.transform.childCount; j++)
                                {
                                    var sprite = filhosdewarriorfather.transform.GetChild(j).GetComponent<SpriteRenderer>();
                                    if(sprite.sprite != null)
                                    {
                                        if(!sprite.sprite.name.Contains("balao"))
                                        {
                                            Array.Resize(ref filhosdewarriorfatherfilhos, indicesfilhos);
                                            filhosdewarriorfatherfilhos[indicesfilhos-1] = filhosdewarriorfather.GetChild(j);
                                            indicesfilhos++;
                                        }
                                    }
                                }
                            }
                        }
                        balaorenderer = guerreiroesquerdo.balao[o].GetComponent<SpriteRenderer>();
                        for(int p=0; p < guerreiroesquerdo.balao_diferentesguerreiros_vetor.Length; p++)
                        {
                            if(balaorenderer.sprite == guerreiroesquerdo.balao_diferentesguerreiros[p] || balaorenderer.sprite == guerreirodireito.balao_vermelho[p])
                            {

                                instanciaguerreiro.GetComponent<movimentar>().enabled = true;
                                if(IsClient)
                                {
                                    notificarservidormovimentartrueServerRpc(instanciaguerreiro.NetworkObjectId);
                                }
                                if(instanciaguerreiro.CompareTag("guerreiroarqueiro") || instanciaguerreiro.CompareTag("guerreirosniper"))
                                {
                                    ataque.GetComponent<atackbutton>().enabled = false;
                                }
                                else
                                {
                                    ataque.GetComponent<gunbow>().enabled = false;
                                }
                                cinemachinecamera.Follow = instanciaguerreiro.transform;
                                balaorenderer.sortingOrder = orderinlayer;
                                constraints = instanciaguerreiro.GetComponent<Rigidbody2D>();
                                constraints.constraints &= ~(RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY);
                                guardar_p = p;
                                guardar_o = o;
                                break;
                            }
                            else
                            {
                                constraints = instanciaguerreiro.GetComponent<Rigidbody2D>();
                                constraints.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                            }
                        }
                    }        
                }
                else
                {
                    for(int o = 0; o < guerreiroesquerdo.balao.Length; o++)
                    {
                        var canvas = jogadorlocal.transform.Find("Canvas");
                        var warrior = canvas.transform.Find("warrior");
                        var scriptwarrior = warrior.GetComponent<warrior_function>();
                        if(scriptwarrior.guerreiros.Length > 0)
                        {
                            scriptwarrior.guerreiros[o].SetActive(true);
                            instanciaguerreiro = scriptwarrior.guerreiros[o].GetComponent<NetworkObject>();
                            instanciaguerreiro.enabled = true;
                            if(instanciaguerreiro.OwnerClientId != NetworkManager.Singleton.LocalClientId)
                            {
                                indices = 1;
                                filhosdewarriorfather = jogadorlocal.transform.Find("warrior's father(Clone)");
                                if(filhosdewarriorfather != null)
                                {
                                    var indicesfilhos = 1;
                                    for(int j=0; j<filhosdewarriorfather.transform.childCount; j++)
                                    {
                                        var sprite = filhosdewarriorfather.transform.GetChild(j).GetComponent<SpriteRenderer>();
                                        if(sprite.sprite != null)
                                        {
                                            if(!sprite.sprite.name.Contains("balao"))
                                            {
                                                Array.Resize(ref filhosdewarriorfatherfilhos, indicesfilhos);
                                                filhosdewarriorfatherfilhos[indicesfilhos-1] = filhosdewarriorfather.GetChild(j);
                                                indicesfilhos++;
                                            }
                                        }
                                    }
                                }
                            }
                            constraints = instanciaguerreiro.GetComponent<Rigidbody2D>();
                            constraints.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                        }
                    }
                }
            }
            for (int i = 0; i <= tamanho_vetor ; i++)
            {
                guerreiroesquerdo.balao[i].SetActive(false);
            }
            selecionarguerreiro = false;
       }
       if(IsServer)
       {
        if(guerreiroesquerdo.balao[DragCentralButton.indices] != null)
        {
            for(int m = 0; m < guerreiroesquerdo.balao.Length; m++)
            {
                balaorenderer = guerreiroesquerdo.balao[m].GetComponent<SpriteRenderer>();
                if(m != guardar_o)
                {
                    if(balaorenderer.sprite != guerreiroesquerdo.balao_diferentesguerreiros[guardar_p] || balaorenderer.sprite != guerreirodireito.balao_vermelho[guardar_p])
                    {
                        balaorenderer.sortingOrder = 3;
                    }
                    if(balaorenderer.sprite == guerreiroesquerdo.balao_diferentesguerreiros[guardar_p] || balaorenderer.sprite == guerreirodireito.balao_vermelho[guardar_p] && proximo == true)
                    {
                        balaorenderer.sortingOrder = orderinlayer;
                        if(proximo == true)
                        {
                            if(guerreiroesquerdo.apertado_botao == 3)
                            {
                                guardar_o--;
                            }
                            if(guerreirodireito.apertado_botao == 4)
                            {
                                guardar_o++;
                            }
                        }
                    }
                    proximo = true;
                }
            }       
        }
       }
       else
       {
        if(guerreiroesquerdo.balao.Length == DragCentralButton.indicescliente+1)
        {
            if(guerreiroesquerdo.balao[DragCentralButton.indicescliente] != null)
            {
                for(int m = 0; m < guerreiroesquerdo.balao.Length; m++)
                {
                    balaorenderer = guerreiroesquerdo.balao[m].GetComponent<SpriteRenderer>();
                    if(m != guardar_o)
                    {
                        if(balaorenderer.sprite != guerreiroesquerdo.balao_diferentesguerreiros[guardar_p] || balaorenderer.sprite != guerreirodireito.balao_vermelho[guardar_p])
                        {
                            balaorenderer.sortingOrder = 3;
                        }
                        if(balaorenderer.sprite == guerreiroesquerdo.balao_diferentesguerreiros[guardar_p] || balaorenderer.sprite == guerreirodireito.balao_vermelho[guardar_p] && proximo == true)
                        {
                            balaorenderer.sortingOrder = orderinlayer;
                            if(proximo == true)
                            {
                                if(guerreiroesquerdo.apertado_botao == 3)
                                {
                                    guardar_o--;
                                }
                                if(guerreirodireito.apertado_botao == 4)
                                {
                                    guardar_o++;
                                }
                            }
                        }
                        proximo = true;
                    }
                }       
            }
        }

       }
       if(apertado_botao == 2)
       {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    apertado_botao = 0;
                    gameObject.SetActive(false);
                }
            }
       }
    }

    [ServerRpc]
    private void notificarservidormovimentartrueServerRpc(ulong guerreiroID)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(guerreiroID, out var guerreironetworkobject))
        {
            guerreironetworkobject.gameObject.SetActive(true);
            guerreironetworkobject.enabled = true;
            guerreironetworkobject.GetComponent<movimentar>().enabled = true;
        }
    }

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }
}
