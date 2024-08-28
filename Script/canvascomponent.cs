using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class canvascomponent : NetworkBehaviour
{
    public GameObject ataque;
    private GameObject instanciaataque, jogadorlocal;
    private NetworkObject ataquenetwork;
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        if(IsServer)
        {
            instanciaataque = Instantiate(ataque, this.transform);
            ataquenetwork = instanciaataque.GetComponent<NetworkObject>();
            ataquenetwork.Spawn();
            ataquenetwork.transform.SetParent(this.transform, false);
        }
        var warrior = this.transform.Find("warrior").gameObject;
        var warriorfunction = warrior.GetComponent<warrior_function>();
        warriorfunction.ataque = instanciaataque;
        GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject jogador in jogadores)
        {
            if(jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                jogadorlocal = jogador;
                var macasradesenrolado = this.transform.Find("mascara para pergaminho desenrolado");
                var scrollbar = macasradesenrolado.transform.Find("Scrollbar");
                var slidingarea = scrollbar.transform.Find("Sliding Area");
                var handle = slidingarea.transform.Find("Handle");
                var mascarabotoesdesenrolado = handle.transform.Find("mascara");
                for(int contador = 0; contador < mascarabotoesdesenrolado.childCount; contador++)
                {
                    if(mascarabotoesdesenrolado.GetChild(contador).name.Contains("guerreiro"))
                    {
                        var dragcentralbutton = mascarabotoesdesenrolado.GetChild(contador).GetComponent<DragCentralButton>();
                        dragcentralbutton.jogador = jogadorlocal.GetComponent<NetworkObject>();
                    }
                }
                break;
            }
        }
        var charactercamera = jogadorlocal.transform.Find("character_camera");
        var charactercamerascript = charactercamera.GetComponent<foco_function>();
        charactercamerascript.ataque = instanciaataque;
        var gunbowscript = ataquenetwork.GetComponent<gunbow>();
        gunbowscript.scriptwarrior = warrior;
        ataquenetwork.gameObject.SetActive(false);
        var atirar = this.transform.Find("atirar").gameObject;
        charactercamerascript.atirar = atirar;
        var left = this.transform.Find("Select Left").gameObject;
        charactercamerascript.esquerdo = left;
        var right = this.transform.Find("Select Right").gameObject;
        charactercamerascript.direito = right;
        var selectwarrior = this.transform.Find("Select Warrior").gameObject;
        charactercamerascript.selectwarrior = selectwarrior;
        var foco = this.transform.Find("foco").gameObject;
        charactercamerascript.botaofoco = foco;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
