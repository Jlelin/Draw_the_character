using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class qual_guerreiro : MonoBehaviour
{
    public delegate void atualizarbalaoselecionado(int balaoselecionado);
    public static event atualizarbalaoselecionado atualizoubalaoselecionado;
    public qual_guerreirodireito guerreirodireito;
    public GameObject[] balao, balao_diferentesguerreiros_vetor;
    public SpriteRenderer receberbalao_vermelho, quero_balaobranco, elementocomparativo;
    public Sprite balao_branco, balaobrancoarmado, balaobrancodesarmado, balaoarqueiro;
    public Sprite[] balao_diferentesguerreiros;
    public int apertado_botao, balao_selecionado, guardarbaloes, umclickdireito, quantoscliks, contarclick;
    public bool um_click, botaodireitoclicado;

    void Awake()
    {
       
        guerreirodireito = Object.FindFirstObjectByType<qual_guerreirodireito>();
        balao_atualizar();
    }    

    // Start is called before the first frame update
    void Start()
    {
        um_click = false;
        balao_selecionado = 0;
        botaodireitoclicado = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(guerreirodireito.apertado_botao == 4 && guerreirodireito.click_x1 == false)
        {
           contarclick = 0;
           balao_selecionado = guerreirodireito.ultimo_elemento-1;
        }
        if (apertado_botao == 3 && balao_selecionado < balao.Length)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    contarclick++;
                    quantoscliks++;
                    if(quantoscliks > guerreirodireito.clicado_primeiro && botaodireitoclicado == true)
                    {
                        guerreirodireito.ultimo_elemento = balao_selecionado;
                        botaodireitoclicado = false;
                        receberbalao_vermelho = balao[balao_selecionado].GetComponent<SpriteRenderer>();
                        for(int baloes = 0; baloes < balao_diferentesguerreiros.Length; baloes++)
                        {
                            if(balao[balao_selecionado].CompareTag("escudo_espada") && balao_diferentesguerreiros_vetor[baloes].CompareTag("escudo_espada"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if(balao[balao_selecionado].CompareTag("armafogo") && balao_diferentesguerreiros_vetor[baloes].CompareTag("armafogo"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if(balao[balao_selecionado].CompareTag("desarmado") && balao_diferentesguerreiros_vetor[baloes].CompareTag("desarmado"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if(balao[balao_selecionado].CompareTag("arqueiro") && balao_diferentesguerreiros_vetor[baloes].CompareTag("arqueiro"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                        }
                    }
                    else
                    {
                        if(balao_selecionado == 0)
                        {
                            balao_selecionado = guerreirodireito.ultimo_elemento;
                        }
                        if(balao_selecionado-1 >= 0  && balao_selecionado != guerreirodireito.ultimo_elemento-1)
                        {
                            balao_selecionado--;
                        }
                        if(contarclick == 2 && balao_selecionado-1 >=0)
                        {
                            contarclick = 1;
                            balao_selecionado--;
                        }
                        botaodireitoclicado = false;
                        receberbalao_vermelho = balao[balao_selecionado].GetComponent<SpriteRenderer>();
                        for(int baloes = 0; baloes < balao_diferentesguerreiros.Length; baloes++)
                        {
                            if(balao[balao_selecionado].CompareTag("escudo_espada") && balao_diferentesguerreiros_vetor[baloes].CompareTag("escudo_espada"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if(balao[balao_selecionado].CompareTag("armafogo") && balao_diferentesguerreiros_vetor[baloes].CompareTag("armafogo"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if(balao[balao_selecionado].CompareTag("desarmado") && balao_diferentesguerreiros_vetor[baloes].CompareTag("desarmado"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if(balao[balao_selecionado].CompareTag("arqueiro") && balao_diferentesguerreiros_vetor[baloes].CompareTag("arqueiro"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                        }
                    }
                    for(int o = 0; o < balao.Length; o++)
                    {
                        if(balao[o] != null)
                        {
                            quero_balaobranco = balao[o].GetComponent<SpriteRenderer>();
                            if(guardarbaloes != o)
                            {
                                if(quero_balaobranco.sprite != balao_branco && quero_balaobranco.sprite != balaobrancoarmado &&
                                quero_balaobranco.sprite != balaobrancodesarmado && quero_balaobranco.sprite != balaoarqueiro)
                                {
                                    if(balao[o].CompareTag("escudo_espada"))
                                    {
                                        quero_balaobranco.sprite = balao_branco;
                                    }
                                    if(balao[o].CompareTag("armafogo"))
                                    {
                                        quero_balaobranco.sprite = balaobrancoarmado;
                                    }
                                    if(balao[o].CompareTag("desarmado"))
                                    {
                                        quero_balaobranco.sprite = balaobrancodesarmado;
                                    }
                                    if(balao[o].CompareTag("arqueiro"))
                                    {
                                        quero_balaobranco.sprite = balaoarqueiro;
                                    }
                                }
                            }
                        }
                        else
                        {
                            
                        }
                        
                    }
                }
            }
        }
    }
    public void atualizar_clickdireito(int click_direito)
    {
        umclickdireito = click_direito;
    }

    public void balao_atualizar()
    {
        guerreirodireito.balao_atualizado(balao);
    }

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }
}
