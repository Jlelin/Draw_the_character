using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class atirar : MonoBehaviour
{
    private Coroutine executando_ounao;
    public gunbowatack gunbowatack;
    public GameObject atirarr, ataque, proibidoatirar;
    public static life life;

    // Start is called before the first frame update
    void Start()
    {
        gunbowatack = atirarr.GetComponent<gunbowatack>();

        // Inscreva-se no evento para iniciar a corrotina no tempo exato
        gunbowatack.OnCorrotinaIniciaTempo += ExecutarCorrotinaNoPonto;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.CompareTag("enemy"))
        {
            if(atirarr.GetComponent<Button>().enabled == false && atirarr.GetComponent<Image>().enabled == false)
            {
                gunbowatack = atirarr.GetComponent<gunbowatack>();
            }
            if(executando_ounao != null && gunbowatack.canAttack == false)
            {
                proibidoatirar.SetActive(true);
            }
            atirarr.SetActive(true);
            ataque.SetActive(false);
            life = other.GetComponent<life>();
        }*/
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        /*if (other.CompareTag("enemy"))
        {
            atirarr.SetActive(false);
            proibidoatirar.SetActive(false);
            ataque.SetActive(true);
        }*/
    }

    // Método chamado pelo evento
    private void ExecutarCorrotinaNoPonto()
    {
        executando_ounao = StartCoroutine(CorrotinaNoPonto());
    }

    private IEnumerator CorrotinaNoPonto()
    {
        // Adicione o código que você deseja executar aqui
        yield return new WaitForSeconds(gunbowatack.tempoderecuperacao);
        proibidoatirar.SetActive(false);
        gunbowatack.GetComponent<UnityEngine.UI.Button>().enabled = true;
        gunbowatack.GetComponent<UnityEngine.UI.Image>().enabled = true;
        gunbowatack.canAttack = true;
    }
}
