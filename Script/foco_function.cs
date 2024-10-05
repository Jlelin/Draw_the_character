using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class foco_function : NetworkBehaviour
{
    public delegate void assinantecorrigirfocofunction();
    public event assinantecorrigirfocofunction corrigirfocofunction;
    public int apertado_botao;

    public static NetworkObject guerreiroativo;
    public NetworkButtonManager networkbutton_manager;
    public qual_guerreiro guerreiroesquerdo;
    public warrior_function warrior;
    public Transform maincamera_position;
    public Camera maincamera;
    public SpriteRenderer terreno;
    public GameObject warriorobject, botaofoco, ataque, esquerdo, direito, selectwarrior, botaodesenho, atirar;
    public GameObject mira, charactercamera;
    private int tamanho_vetor, ativoucoroutine; // Mova a declaração para o escopo da classe
    public float largura, altura;

    void Awake()
    {
        
        warrior = Object.FindFirstObjectByType<warrior_function>();
        networkbutton_manager = NetworkButtonManager.canvasHostClient.GetComponent<NetworkButtonManager>();
        corrigirfocofunction += corrigirfocofunctionn;

    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EsperarBotaofoco());
    }
    // Update is called once per frame
    public void Update()
    {
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
                                GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
                                foreach(GameObject jogador in jogadores)
                                {
                                    if(jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                                    {
                                        charactercamera = jogador.transform.Find("character_camera").gameObject;
                                        var canvas = jogador.transform.Find("Canvas(Clone)");
                                        warrior = canvas.transform.Find("warrior").gameObject.GetComponent<warrior_function>();
                                    }
                                    else
                                    {
                                        var warriorfather = jogador.transform.Find("warrior's father(Clone)");
                                        for(int contador=0; contador < warriorfather.childCount;contador++)
                                        {
                                            if((warriorfather.GetChild(contador).name.Contains("guerreiro") || warriorfather.GetChild(contador).name.Contains("arqueiro"))
                                            && !warriorfather.GetChild(contador).name.Contains("balao"))
                                            {
                                                warriorfather.GetChild(contador).gameObject.SetActive(false);
                                            }
                                        }
                                    }
                                }
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
                    charactercamera.SetActive(false);
                    botaofoco.SetActive(false);
                    apertado_botao = 0;
                    warriorobject.SetActive(true);
                }
            }
        }
        if(botaofoco != null)
        {
            if(botaofoco.activeSelf && guerreiroativo != null && ativoucoroutine < 1)
            {
                if(guerreiroativo.gameObject.activeSelf)
                {
                    StartCoroutine(enquantoestaativo());
                    ativoucoroutine++;
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

    public void atribuirguerreiroesquerdoatamanhovetor()
    {
        tamanho_vetor = guerreiroesquerdo.balao.Length;
    }

    public void focofunctioninitialessentials()
    {
        corrigirfocofunction?.Invoke();
        tamanho_vetor = guerreiroesquerdo.balao.Length;
        maincamera = networkbutton_manager.maincamera;
        maincamera_position = networkbutton_manager.cameratransform;
        terreno = networkbutton_manager.terreno;
        altura = terreno.bounds.size.y;
        altura = altura * 100;
        largura = altura / 100f;
        largura = largura / 2f;
        maincamera_position.position = new Vector3(0, 0, -1);
        maincamera.orthographicSize = largura;
        if(guerreiroesquerdo.balao[0] != null)
        {
            for (int i = 0; i <= tamanho_vetor; i++) { // Corrija a condição aqui
                guerreiroesquerdo.balao[i].SetActive(true);
            }
        }
    }

    private IEnumerator enquantoestaativo()
    {
        while(guerreiroativo.gameObject.activeSelf)
        {
            yield return null;
            GameObject[] todosobjetos = FindObjectsOfType<GameObject>(true);
            foreach(GameObject objeto in todosobjetos)
            {
                if(botaofoco.activeSelf)
                {
                    if((objeto.name.Contains("guerreiro") || objeto.name.Contains("arqueiro")) && !objeto.name.Contains("balao") && !objeto.name.Contains("tag"))
                    {
                        if(!objeto.activeSelf)
                        {
                            objeto.SetActive(true);
                        }
                    }
                }
            }
        }
        ativoucoroutine = 0;
    }

    IEnumerator EsperarBotaofoco()
    {
        // Aguarda até que botaofoco seja atribuído
        while (botaofoco == null)
        {
            yield return null; // Espera até o próximo frame para verificar novamente
        }

        // Selecione o EventTrigger do botaofoco
        EventTrigger trigger = botaofoco.GetComponent<EventTrigger>();

        if (trigger != null) // Verifica se o EventTrigger existe no botaofoco
        {
            // Se já existir o evento PointerDown, adicione a função apertar_botao(1)
            foreach (var entry in trigger.triggers)
            {
                if (entry.eventID == EventTriggerType.PointerDown)
                {
                    entry.callback.AddListener((eventData) => { apertar_botao(1); });
                }
                else if (entry.eventID == EventTriggerType.PointerUp)
                {
                    entry.callback.AddListener((eventData) => { apertar_botao(0); });
                }
            }
        }
        else
        {
            Debug.LogWarning("EventTrigger não encontrado no objeto botaofoco.");
        }
    }
}
