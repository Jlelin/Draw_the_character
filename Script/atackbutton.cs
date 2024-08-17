using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Necessário para interfaces IPointerDownHandler e IPointerUpHandler
using UnityEngine.UI;
public class atackbutton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public delegate void AtivarCorrotinaEvento();
    public event AtivarCorrotinaEvento OnCorrotinaIniciaTempo;
    public GameObject proibidoatacar;
    public static GameObject naoatacar;
    public int apertado_botao;
    public float tempoderecuperacao;
    public bool canAttack = true;

    void Awake()
    {
        naoatacar = proibidoatacar;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Inicializações, se necessárias
    }

    // Update is called once per frame
    void Update()
    {
        if (apertado_botao == 1 && canAttack)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                alvoparaespada.life.characterlife--;
                StartCoroutine(Cooldown()); // Iniciar o cooldown após o ataque
            }
        }
    }

    // Método chamado quando o ponteiro é pressionado sobre o GameObject
    public void OnPointerDown(PointerEventData eventData)
    {
        apertar_botao(1); // Define apertado_botao como 1 quando o ponteiro é pressionado
    }

    // Método chamado quando o ponteiro é liberado do GameObject
    public void OnPointerUp(PointerEventData eventData)
    {
        apertar_botao(0); // Define apertado_botao como 0 quando o ponteiro é liberado
    }

    // Método para atualizar o estado do botão
    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }

    private IEnumerator Cooldown()
    {
        canAttack = false; // Desativa o ataque
        proibidoatacar.SetActive(true);
        this.GetComponent<Button>().enabled = false;
        this.GetComponent<Image>().enabled = false;
        OnCorrotinaIniciaTempo?.Invoke();
        yield return new WaitForSeconds(tempoderecuperacao);
        proibidoatacar.SetActive(false); // Espera o tempo de cooldown
        this.GetComponent<Image>().enabled = true;
        this.GetComponent<Button>().enabled = true;
        canAttack = true; // Ativa o ataque novamente
    }
}
