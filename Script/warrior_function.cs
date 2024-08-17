using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class warrior_function : NetworkBehaviour
{
    public qual_guerreiro guerreiroesquerdo;
    public qual_guerreirodireito guerreirodireito;
    public GameObject cinemachine, botaofoco, esquerdo, direito, selectwarrior, atirar, proibidoatirar, proibidoatacar;
    public CinemachineVirtualCamera cinemachinecamera;
    public GameObject[] guerreiros;
    public static GameObject[] guerreiro;
    public GameObject botaodesenho, ataque, mira;
    public Rigidbody2D constraints;
    public SpriteRenderer balaorenderer, balaoatual;
    public static NetworkObject instanciaguerreiro;
    public static NetworkVariable<ulong> guerreirosID = new NetworkVariable<ulong>();
    public int orderinlayer;
    public int apertado_botao, guardar_o, guardar_p;
    public bool selecionarguerreiro;
    private int tamanho_vetor; // Mova a declaração para o escopo da classe
    private bool proximo;

    void Awake()
    {
        guerreirodireito = Object.FindFirstObjectByType<qual_guerreirodireito>();
        guerreiroesquerdo = Object.FindFirstObjectByType<qual_guerreiro>();
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
            cinemachine.SetActive(true);
            for(int o = 0; o < guerreiroesquerdo.balao.Length; o++)
            {
                guerreiros[o].SetActive(true);
                instanciaguerreiro = guerreiro[o].GetComponent<NetworkObject>();
                instanciaguerreiro.enabled = true;
                guerreirosID.Value = instanciaguerreiro.NetworkObjectId;
                if((instanciaguerreiro.CompareTag("arqueiro") && instanciaguerreiro.IsSpawned == true) || (instanciaguerreiro.CompareTag("armafogo") && instanciaguerreiro.IsSpawned == true))
                {
                    ataque.SetActive(true);
                }
                balaorenderer = guerreiroesquerdo.balao[o].GetComponent<SpriteRenderer>();
                for(int p=0; p < guerreiroesquerdo.balao_diferentesguerreiros_vetor.Length; p++)
                {
                    if(balaorenderer.sprite == guerreiroesquerdo.balao_diferentesguerreiros[p] || balaorenderer.sprite == guerreirodireito.balao_vermelho[p])
                    {

                        instanciaguerreiro.GetComponent<movimentar>().enabled = true;
                        if(instanciaguerreiro.CompareTag("arqueiro") || instanciaguerreiro.CompareTag("armafogo"))
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
                        constraints.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;;
                        instanciaguerreiro.GetComponent<movimentar>().enabled = false;
                    }
                }
            }
            for (int i = 0; i <= tamanho_vetor ; i++)
            {
                guerreiroesquerdo.balao[i].SetActive(false);
            }
            selecionarguerreiro = false;
       }
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

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }
}
