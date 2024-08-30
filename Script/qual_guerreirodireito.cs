using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class qual_guerreirodireito : MonoBehaviour
{
    public delegate void atualizarultimoelemento(int indice);
    public static event atualizarultimoelemento ultimoelementoatualizado;
    public delegate void AtualizarClickDireito(int click_direito);
    public static event AtualizarClickDireito ValorAtualizou;
    public qual_guerreiro guerreiroesquerdo;
    public GameObject[] balao_dguerreiros, meus_baloes;
    public SpriteRenderer balao_branco_elementofinal, elemento_diferente;
    public Sprite white_balao, balaobrancoarmado, balaobrancodesarmado, balaoarqueiro;
    public Sprite[] balao_vermelho;
    public int ultimo_elemento, apertado_botao, guardar_n, clicado_primeiro, guardar_click, guardar_o, clicks, guardarbaloes, quantoscliks;
    public int[] valoresdeelementos;
    public bool click_x1, diferentedeum, aconteceu, elementoprimeirovalor, lenghtrecebe, elementomaismais;
    void Awake()
    {
        guerreiroesquerdo = Object.FindFirstObjectByType<qual_guerreiro>();
        GameObject jogador = GameObject.FindGameObjectWithTag("Player");
        var tamanho = 1;
        for(int contador = 0; contador < jogador.transform.childCount; contador++)
        {
            if(jogador.transform.GetChild(contador).name.Contains("tag"))
            {
                balao_dguerreiros[tamanho-1] = jogador.transform.GetChild(contador).gameObject;
                tamanho++;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ultimo_elemento = meus_baloes.Length - 1;
        click_x1 = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(guerreiroesquerdo.apertado_botao == 3 && guerreiroesquerdo.botaodireitoclicado == false)
        {
            quantoscliks = 0;
            ultimo_elemento = guerreiroesquerdo.balao_selecionado+1;
        }
        if(apertado_botao == 4)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    quantoscliks++;
                    clicado_primeiro++;
                    if(clicado_primeiro > guerreiroesquerdo.quantoscliks && click_x1 == true)
                    {
                        guerreiroesquerdo.balao_selecionado = ultimo_elemento;
                        click_x1 = false;
                        balao_branco_elementofinal = meus_baloes[ultimo_elemento].GetComponent<SpriteRenderer>();
                        for(int baloes = 0; baloes < balao_dguerreiros.Length; baloes++)
                        {
                            if(meus_baloes[ultimo_elemento].CompareTag("guerreiroescudoespada") && balao_dguerreiros[baloes].CompareTag("guerreiroescudoespada"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if(meus_baloes[ultimo_elemento].CompareTag("guerreirosniper") && balao_dguerreiros[baloes].CompareTag("guerreirosniper"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if(meus_baloes[ultimo_elemento].CompareTag("guerreirodesarmado") && balao_dguerreiros[baloes].CompareTag("guerreirodesarmado"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if(meus_baloes[ultimo_elemento].CompareTag("guerreiroarqueiro") && balao_dguerreiros[baloes].CompareTag("guerreiroarqueiro"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                        }
                    }
                    else
                    {
                        if(ultimo_elemento == meus_baloes.Length-1)
                        {
                            ultimo_elemento = guerreiroesquerdo.balao_selecionado;
                        }
                        if(ultimo_elemento+1 < meus_baloes.Length && ultimo_elemento != guerreiroesquerdo.balao_selecionado+1)
                        {
                            ultimo_elemento++;
                            //guerreiroesquerdo.balao_selecionado = ultimo_elemento-1;
                        }
                        if(quantoscliks == 2 && ultimo_elemento+1 < meus_baloes.Length)
                        {
                            quantoscliks = 1;
                            ultimo_elemento++;
                        }
                        click_x1 = false;
                        balao_branco_elementofinal = meus_baloes[ultimo_elemento].GetComponent<SpriteRenderer>();
                        for(int baloes = 0; baloes < balao_dguerreiros.Length; baloes++)
                        {
                            if(meus_baloes[ultimo_elemento].CompareTag("guerreiroescudoespada") && balao_dguerreiros[baloes].CompareTag("guerreiroescudoespada"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if(meus_baloes[ultimo_elemento].CompareTag("guerreirosniper") && balao_dguerreiros[baloes].CompareTag("guerreirosniper"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if(meus_baloes[ultimo_elemento].CompareTag("guerreirodesarmado") && balao_dguerreiros[baloes].CompareTag("guerreirodesarmado"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if(meus_baloes[ultimo_elemento].CompareTag("guerreiroarqueiro") && balao_dguerreiros[baloes].CompareTag("guerreiroarqueiro"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                        }
                    }
                    for(int o = 0; o < meus_baloes.Length; o++)
                    {
                        elemento_diferente = meus_baloes[o].GetComponent<SpriteRenderer>();
                        if(guardarbaloes != o)
                        {
                            if(elemento_diferente.sprite != white_balao && elemento_diferente.sprite != balaobrancoarmado &&
                            elemento_diferente.sprite != balaobrancodesarmado && elemento_diferente.sprite != balaoarqueiro)
                            {
                                if(meus_baloes[o].CompareTag("guerreiroescudoespada"))
                                {
                                    elemento_diferente.sprite = white_balao;
                                }
                                if(meus_baloes[o].CompareTag("guerreirosniper"))
                                {
                                    elemento_diferente.sprite = balaobrancoarmado;
                                }
                                if(meus_baloes[o].CompareTag("guerreirodesarmado"))
                                {
                                    elemento_diferente.sprite = balaobrancodesarmado;
                                }
                                if(meus_baloes[o].CompareTag("guerreiroarqueiro"))
                                {
                                    elemento_diferente.sprite = balaoarqueiro;
                                }
                            }
                        }
                        
                    }
                }
            }
        }
    }
    public void balao_atualizado(GameObject[] balao)
    {
        meus_baloes = balao;
    }    

    public void apertar_botao_direito(int pressionado)
    {
        apertado_botao = pressionado;
    }
}