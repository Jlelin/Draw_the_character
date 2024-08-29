using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class addcanvas : NetworkBehaviour
{
    public delegate void assinaturapararodarmetodofocofunctioninicial();
    public event assinaturapararodarmetodofocofunctioninicial valoresatribuidosafocofunction;
    public delegate void assinaturaparaavisarsobreguerreiroesquerdo();
    public event assinaturaparaavisarsobreguerreiroesquerdo guerreiroesquerdopronto;
    public foco_function focofunction;
    public GameObject canvas, focofunctionobject;
    public static GameObject instanciacanvas;
    private GameObject[] jogadores;
    public static NetworkObject canvasnetwork;
    private static bool foiinstanciado;
    // Start is called before the first frame update

    void Awake()
    {
        focofunction = focofunctionobject.GetComponent<foco_function>();
        valoresatribuidosafocofunction += focofunction.focofunctioninitialessentials;
    }
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
                }
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
        canvasnetwork = instanciacanvas?.GetComponent<NetworkObject>();
        var botaodesenho = canvasnetwork.transform.Find("botaodesenho").gameObject;
        var botaodesenhoscript = botaodesenho.GetComponent<botaodesenho>();
        var focofunctionObject = jogadorlocal.transform.Find("focofunction");
        var focofunctionscript = focofunctionObject.GetComponent<foco_function>();
        focofunctionscript.botaodesenho = botaodesenho;
        var warrior = canvasnetwork.transform.Find("warrior").gameObject;
        focofunctionscript.warriorobject = warrior;
        var left = canvasnetwork.transform.Find("Select Left").gameObject;
        var guerreiroesquerdo = left.GetComponent<qual_guerreiro>();
        focofunctionscript.guerreiroesquerdo = guerreiroesquerdo;
        guerreiroesquerdopronto += focofunctionscript.atribuirguerreiroesquerdoatamanhovetor;
        guerreiroesquerdopronto?.Invoke();
        botaodesenhoscript.foco = focofunctionscript;
        valoresatribuidosafocofunction?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    private void instanciarcanvasServerRpc(ServerRpcParams serverrpcparams = default)
    {
        var clientId = serverrpcparams.Receive.SenderClientId;
        jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject jogadorlocal in jogadores)
        {
            if(jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
                if(!jogadorlocal.transform.Find("Canvas(Clone)"))
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
}
