using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class addcanvas : NetworkBehaviour
{
    public GameObject canvas;
    [SerializeField] private GameObject instanciacanvas;
    private GameObject[] jogadores;
    private NetworkObject canvasnetwork;
    private static bool foiinstanciado;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsClient)
        {
            instanciarcanvasServerRpc();
        }
        else if(!foiinstanciado)
        {
            jogadores = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject jogadorlocal in jogadores)
            {
                if(jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    instanciacanvas = Instantiate(canvas, jogadorlocal.transform);
                    canvasnetwork = instanciacanvas.GetComponent<NetworkObject>();
                    canvasnetwork.Spawn();
                    canvasnetwork.transform.SetParent(jogadorlocal.transform, false);
                    foiinstanciado = true;
                    break;
                }
            }
        }
        jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject jogadorlocal in jogadores)
        {
            if(jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                if(IsClient)
                {
                    StartCoroutine(aguardandocanvas(jogadorlocal));
                    canvasnetwork = instanciacanvas.GetComponent<NetworkObject>();
                }
                var botaodesenho = canvasnetwork.transform.Find("botaodesenho").gameObject;
                var botaodesenhoscript = botaodesenho.GetComponent<botaodesenho>();
                var charactercamera = jogadorlocal.transform.Find("character_camera");
                var charactercamerascript = charactercamera.GetComponent<foco_function>();
                charactercamerascript.botaodesenho = botaodesenho;
                var warrior = canvasnetwork.transform.Find("warrior").gameObject;
                charactercamerascript.warriorobject = warrior;
                var left = canvasnetwork.transform.Find("Select Left").gameObject;
                var guerreiroesquerdo = left.GetComponent<qual_guerreiro>();
                charactercamerascript.guerreiroesquerdo = guerreiroesquerdo;
                botaodesenhoscript.foco = charactercamerascript;
                break;
            }
        }
    }

    private IEnumerator aguardandocanvas(GameObject jogadorlocal)
    {
        while(instanciacanvas == null)
        {
            instanciacanvas = jogadorlocal.transform.Find("Canvas(Clone)")?.gameObject;
            yield return null;
        }
    }

    [ServerRpc]
    private void instanciarcanvasServerRpc(ServerRpcParams serverrpcparams = default)
    {
        var clientId = serverrpcparams.Receive.SenderClientId;
        jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject jogadorlocal in jogadores)
        {
            if(jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
                instanciacanvas = Instantiate(canvas, jogadorlocal.transform);
                canvasnetwork = instanciacanvas.GetComponent<NetworkObject>();
                canvasnetwork.Spawn();
                canvasnetwork.transform.SetParent(jogadorlocal.transform, false);
                canvasnetwork.ChangeOwnership(clientId);
            }
        }
    }
}
