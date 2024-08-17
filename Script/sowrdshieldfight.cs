using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class sowrdshieldfight : NetworkBehaviour
{
    public GameObject ataquebutton;
    public atackbutton atackbutton;
    private Coroutine executando_ounao;

    void Awake()
    {
    }

    void Start()
    {
        atackbutton = ataquebutton.GetComponent<atackbutton>();
        atackbutton.OnCorrotinaIniciaTempo += executarcoroutinetempodescongelado;
    }

    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy"))
        {
            atackbutton.proibidoatacar.SetActive(false);
            if(executando_ounao != null && atackbutton.canAttack == false)
            {
                atackbutton.proibidoatacar.SetActive(true);
            }
            ataquebutton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("enemy"))
        {
            ataquebutton.SetActive(false);
            atackbutton.proibidoatacar.SetActive(false);

        }
    }

    private void executarcoroutinetempodescongelado()
    {
        executando_ounao = StartCoroutine(tempodescongelado());
    }

    private IEnumerator tempodescongelado()
    {
        yield return new WaitForSeconds(atackbutton.tempoderecuperacao);
        atackbutton.proibidoatacar.SetActive(false); // Espera o tempo de cooldown
        atackbutton.GetComponent<Image>().enabled = true;
        atackbutton.GetComponent<Button>().enabled = true;
        atackbutton.canAttack = true; // Ativa o ataque novamente
    }
}
