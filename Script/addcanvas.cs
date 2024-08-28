using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class addcanvas : NetworkBehaviour
{
    public GameObject canvas;
    private GameObject instanciacanvas;
    private NetworkObject canvasnetwork;
    // Start is called before the first frame update
    void Start()
    {
        if(IsServer)
        {
            instanciacanvas = Instantiate(canvas, this.transform);
            canvasnetwork = instanciacanvas.GetComponent<NetworkObject>();
            canvasnetwork.Spawn();
        }
        canvasnetwork.transform.SetParent(this.transform, false);
        var botaodesenho = canvasnetwork.transform.Find("botaodesenho").gameObject;
        var botaodesenhoscript = botaodesenho.GetComponent<botaodesenho>();
        var charactercamera = this.transform.Find("character_camera");
        var charactercamerascript = charactercamera.GetComponent<foco_function>();
        charactercamerascript.botaodesenho = botaodesenho;
        var warrior = canvasnetwork.transform.Find("warrior").gameObject;
        charactercamerascript.warriorobject = warrior;
        var left = canvasnetwork.transform.Find("Select Left").gameObject;
        var guerreiroesquerdo = left.GetComponent<qual_guerreiro>();
        charactercamerascript.guerreiroesquerdo = guerreiroesquerdo;
        botaodesenhoscript.foco = charactercamerascript;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
