using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class gunbowatack : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float distancia1, distancia2, distancia3, tempo, tempo1, tempo2, tempo3, diferenca, tempoderecuperacao;
    public bool isPressed = false, canAttack = true; 
    public GameObject proibidoatirar;
    private Transform guerreirocinemachine, guerreiroPrincipal, miratransform;
    private Vector3 distancia_entreguerreiro_emira;
    public foco_function foco;
    public warrior_function warriorfunction;
    public life life;

    // Variável para armazenar a corrotina ativa
    private Coroutine corrotinaAtual;

    // Variáveis para armazenar o tempo de desativação e o tempo restante
    private float tempoDesativacao;
    private float tempoRestante;

    // Evento para notificar quando a corrotina chega ao ponto desejado
    public delegate void AtivarCorrotinaEvento();
    public event AtivarCorrotinaEvento OnCorrotinaIniciaTempo;

    void Awake()
    {
        foco = FindFirstObjectByType<foco_function>();
    }

    void Start()
    {
        miratransform = observarmira.mira.transform;
    }

    void Update()
    {
        warriorfunction = foco.warriorobject.GetComponent<warrior_function>();
        if (isPressed && canAttack)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                isPressed = false;

                // Certifique-se de que a corrotina atual esteja parada antes de iniciar uma nova
                if (corrotinaAtual != null)
                {
                    StopCoroutine(corrotinaAtual);
                }

                if(guerreiroPrincipal.CompareTag("arqueiro"))
                {
                    if(diferenca <= distancia1)
                    {
                        corrotinaAtual = StartCoroutine(primeiradistancia());
                    }
                    else if(diferenca <= distancia2)
                    {
                        corrotinaAtual = StartCoroutine(segundadistancia());
                    }
                    else if(diferenca <= distancia3)
                    {
                        corrotinaAtual = StartCoroutine(terceiradistancia());
                    }
                }
                else
                {
                    corrotinaAtual = StartCoroutine(com_armadefogo());
                }
            }
        }

        life = atirar.life;

        for (int i = 0; i < warriorfunction.guerreiros.Length; i++)
        {
            guerreirocinemachine = warriorfunction.guerreiros[i].transform;
            if (warrior_function.cinemachinecamera.Follow == guerreirocinemachine)
            {
                guerreiroPrincipal = warriorfunction.guerreiros[i].transform;
                distancia_entreguerreiro_emira = guerreiroPrincipal.position - miratransform.position;

                // Calcula a distância entre o guerreiro principal e a mira
                diferenca = distancia_entreguerreiro_emira.magnitude;

            }
        }

        // Se o botão está desativado, calcule o tempo restante
        if (!canAttack && Time.time < tempoDesativacao + tempoderecuperacao)
        {
            tempoRestante = (tempoDesativacao + tempoderecuperacao) - Time.time;
        }
        else
        {
            tempoRestante = 0;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    private IEnumerator primeiradistancia()
    {
        canAttack = false;
        tempoDesativacao = Time.time; // Armazena o tempo de desativação
        this.GetComponent<UnityEngine.UI.Button>().enabled = false;
        this.GetComponent<UnityEngine.UI.Image>().enabled = false;
        proibidoatirar.SetActive(true);

        // Notifica que a corrotina está prestes a iniciar a espera
        OnCorrotinaIniciaTempo?.Invoke();
        
        yield return new WaitForSeconds(tempo1);

        life.characterlife--;
        yield return new WaitForSeconds(tempoderecuperacao);
        proibidoatirar.SetActive(false);
        this.GetComponent<UnityEngine.UI.Button>().enabled = true;
        this.GetComponent<UnityEngine.UI.Image>().enabled = true;
        canAttack = true;
        corrotinaAtual = null; // Reseta a corrotina ativa
    }

    private IEnumerator segundadistancia()
    {
        canAttack = false;
        tempoDesativacao = Time.time; // Armazena o tempo de desativação
        this.GetComponent<UnityEngine.UI.Button>().enabled = false;
        this.GetComponent<UnityEngine.UI.Image>().enabled = false;
        proibidoatirar.SetActive(true);

        // Notifica que a corrotina está prestes a iniciar a espera
        OnCorrotinaIniciaTempo?.Invoke();
        
        yield return new WaitForSeconds(tempo2);

        life.characterlife--;
        yield return new WaitForSeconds(tempoderecuperacao);
        proibidoatirar.SetActive(false);
        this.GetComponent<UnityEngine.UI.Button>().enabled = true;
        this.GetComponent<UnityEngine.UI.Image>().enabled = true;
        canAttack = true;
        corrotinaAtual = null; // Reseta a corrotina ativa
    }

    private IEnumerator terceiradistancia()
    {
        canAttack = false;
        tempoDesativacao = Time.time; // Armazena o tempo de desativação
        this.GetComponent<UnityEngine.UI.Button>().enabled = false;
        this.GetComponent<UnityEngine.UI.Image>().enabled = false;
        proibidoatirar.SetActive(true);

        // Notifica que a corrotina está prestes a iniciar a espera
        OnCorrotinaIniciaTempo?.Invoke();
        
        yield return new WaitForSeconds(tempo3);

        life.characterlife--;
        yield return new WaitForSeconds(tempoderecuperacao);
        proibidoatirar.SetActive(false);
        this.GetComponent<UnityEngine.UI.Button>().enabled = true;
        this.GetComponent<UnityEngine.UI.Image>().enabled = true;
        canAttack = true;
        corrotinaAtual = null; // Reseta a corrotina ativa
    }

    private IEnumerator com_armadefogo()
    {
        canAttack = false;
        tempoDesativacao = Time.time; // Armazena o tempo de desativação
        this.GetComponent<UnityEngine.UI.Button>().enabled = false;
        this.GetComponent<UnityEngine.UI.Image>().enabled = false;
        proibidoatirar.SetActive(true);

        // Notifica que a corrotina está prestes a iniciar a espera
        OnCorrotinaIniciaTempo?.Invoke();
        
        yield return new WaitForSeconds(tempo);

        life.characterlife--;
        yield return new WaitForSeconds(tempoderecuperacao);
        proibidoatirar.SetActive(false);
        this.GetComponent<UnityEngine.UI.Button>().enabled = true;
        this.GetComponent<UnityEngine.UI.Image>().enabled = true;
        canAttack = true;
        corrotinaAtual = null; // Reseta a corrotina ativa
    }
}
