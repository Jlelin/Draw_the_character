using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foco_function : MonoBehaviour
{
    public delegate void assinantecorrigirfocofunction();
    public event assinantecorrigirfocofunction corrigirfocofunction;
    public int apertado_botao;
    public NetworkButtonManager networkbutton_manager;
    public qual_guerreiro guerreiroesquerdo;
    public warrior_function warrior;
    public Transform maincamera_position;
    public Camera maincamera;
    public SpriteRenderer terreno;
    public GameObject warriorobject, botaofoco, ataque, esquerdo, direito, selectwarrior, botaodesenho, atirar;
    public GameObject mira;
    private int tamanho_vetor; // Mova a declaração para o escopo da classe
    private float largura, altura;

    void Awake()
    {
        
        warrior = Object.FindFirstObjectByType<warrior_function>();
        guerreiroesquerdo = FindFirstObjectByType<qual_guerreiro>();
        networkbutton_manager = NetworkButtonManager.canvasHostClient.GetComponent<NetworkButtonManager>();
        corrigirfocofunction += corrigirfocofunctionn;

    }
    // Start is called before the first frame update
    void Start()
    {
        corrigirfocofunction?.Invoke();
        botaodesenho.SetActive(true);
        warriorobject.SetActive(true);
        tamanho_vetor = guerreiroesquerdo.balao.Length;
        maincamera = networkbutton_manager.maincamera;
        maincamera_position = networkbutton_manager.cameratransform;
        terreno = networkbutton_manager.terreno;
        altura = terreno.bounds.size.y;
        altura = altura * 100;
        largura = altura / 100f;
        largura = largura / 2f;
        gameObject.SetActive(false);
        maincamera_position.position = new Vector3(0, 0, -1);
        maincamera.orthographicSize = largura;
        if(guerreiroesquerdo.balao[0] != null)
        {
            for (int i = 0; i <= tamanho_vetor; i++) { // Corrija a condição aqui
                guerreiroesquerdo.balao[i].SetActive(true);
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {
        tamanho_vetor = guerreiroesquerdo.balao.Length;
        if (apertado_botao == 1)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    maincamera_position.position = new Vector3(0, 0, -1);
                    maincamera.orthographicSize = largura;
                    ataque.SetActive(false);
                    atirar.SetActive(false);
                    mira.SetActive(false);
                    if( gunbow.contador > 1)
                    {
                        gunbow.contador = 0;
                    }
                    if(guerreiroesquerdo.balao[0] != null)
                    {
                        for (int i = 0; i <= tamanho_vetor; i++) 
                        { // Corrija a condição aqui
                            if(i < guerreiroesquerdo.balao.Length)
                            {
                                guerreiroesquerdo.balao[i].SetActive(true);
                                warrior.guerreiros[i].SetActive(false);
                            }
                        }
                    }
                    if(ataque.GetComponent<atackbutton>().enabled == true)
                    {
                        ataque.GetComponent<gunbow>().enabled = true;
                    }
                    else
                    {
                        ataque.GetComponent<atackbutton>().enabled = true;
                    }
                    esquerdo.SetActive(true);
                    direito.SetActive(true);
                    selectwarrior.SetActive(true);
                    botaodesenho.SetActive(true);
                    botaofoco.SetActive(false);
                    apertado_botao = 0;
                    warriorobject.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }

    }

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }

    public void corrigirfocofunctionn()
    {
        networkbutton_manager.ConfigurePlayerCharacter();
    }
}
