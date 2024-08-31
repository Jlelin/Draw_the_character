using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class canvascomponent : NetworkBehaviour
{
    public delegate void assinaturapararodarmetodofocofunctioninicial();
    public event assinaturapararodarmetodofocofunctioninicial valoresatribuidosafocofunction;
    public delegate void assinaturaparaatribuirvalordescriptwarrior();
    public event assinaturaparaatribuirvalordescriptwarrior scriptwarriortemvalor;
    public GameObject ataque;
    private GameObject instanciaataque, instanciaataqueserver, jogadorlocal, warrior;
    private NetworkObject ataquenetwork, ataquenetworkserver;
    private static bool bastaumavez, clientrpcnotificarsobrevaloresdecanvasaconteceu, ataqueficafalseumavez;
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(clientesrecebemvaloresdeserver());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator aguardarataque(GameObject jogador)
    {
        var canvas = jogador.transform.Find("Canvas(Clone)")?.gameObject;
        while(canvas == null)
        {
            canvas = jogador.transform.Find("Canvas(Clone)")?.gameObject;
            yield return null;
        }
        instanciarataquenoclienteServerRpc();
        jogadorlocal = jogador;
        while(instanciaataque == null)
        {
            instanciaataque = canvas.transform.Find("Ataque(Clone)")?.gameObject;
            yield return null;
        }
        ataquenetwork = instanciaataque.GetComponent<NetworkObject>();
        var warrior = this.transform.Find("warrior").gameObject;
        var warriorfunction = warrior.GetComponent<warrior_function>();
        warriorfunction.ataque = instanciaataque;
        GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
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
                dragcentralbutton.jogador = jogador.GetComponent<NetworkObject>();
            }
        }
        var focofunctionobject = jogador.transform.Find("focofunction");
        var focofunctionscript = focofunctionobject.GetComponent<foco_function>();
        focofunctionscript.ataque = instanciaataque;
        var gunbowscript = ataquenetwork.GetComponent<gunbow>();
        gunbowscript.scriptwarrior = warrior;
        if(!ataqueficafalseumavez)
        {
            ataquenetwork.gameObject.SetActive(false);
            ataqueficafalseumavez = true;
        }
        var atirar = this.transform.Find("atirar").gameObject;
        focofunctionscript.atirar = atirar;
        var left = this.transform.Find("Select Left").gameObject;
        focofunctionscript.esquerdo = left;
        var right = this.transform.Find("Select Right").gameObject;
        focofunctionscript.direito = right;
        var selectwarrior = this.transform.Find("Select Warrior").gameObject;
        focofunctionscript.selectwarrior = selectwarrior;
        var foco = this.transform.Find("foco").gameObject;
        focofunctionscript.botaofoco = foco;
    }

    private IEnumerator aguardarcanvasdocliente(GameObject jogadorcliente)
    {
        var canvas = jogadorcliente.transform.Find("Canvas(Clone)")?.gameObject;
        var ataque = jogadorcliente.transform.Find("Ataque(Clone)")?.gameObject;
        while(canvas == null && ataque == null)
        {
            canvas = jogadorcliente.transform.Find("Canvas(Clone)")?.gameObject;
            ataque = jogadorcliente.transform.Find("Ataque(Clone)")?.gameObject;
            yield return null;
        }
        if(!clientrpcnotificarsobrevaloresdecanvasaconteceu)
        {
            notificarclientessobrevaloresdecanvasClientRpc();
        }
    }

    private IEnumerator clientesrecebemvaloresdeserver()
    {
        GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
        while(jogadores.Length < float.PositiveInfinity)
        {
            jogadores = GameObject.FindGameObjectsWithTag("Player");
            if(IsServer && !bastaumavez)
            {
                GameObject canvas = null;
                foreach(GameObject jogador in jogadores)
                {
                    if(jogador.GetComponent<NetworkObject>().IsOwnedByServer)
                    {
                       canvas = jogador.transform.Find("Canvas(Clone)").gameObject; 
                    }
                }
                instanciaataque = Instantiate(ataque, this.transform);
                ataquenetwork = instanciaataque.GetComponent<NetworkObject>();
                ataquenetwork.Spawn();
                var warrior = canvas.transform.Find("warrior");
                RectTransform warriorRectTransform = warrior.GetComponent<RectTransform>();
                RectTransform ataqueRectTransform = ataquenetwork.GetComponent<RectTransform>();
                ataqueRectTransform.anchoredPosition = warriorRectTransform.anchoredPosition;
                ataqueRectTransform.sizeDelta = warriorRectTransform.sizeDelta;
                ataquenetwork.transform.localScale = warrior.localScale;
                ataquenetwork.transform.SetParent(this.transform, false);
                bastaumavez = true;
            }
            else
            {
                GameObject[] jogadoreslocais = GameObject.FindGameObjectsWithTag("Player");
                foreach(GameObject jogadorlocal in jogadoreslocais)
                {
                    if(jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        StartCoroutine(aguardarataque(jogadorlocal));
                    }
                }
            }
            if(IsServer)
            {
                var warrior = this.transform.Find("warrior").gameObject;
                var warriorfunction = warrior.GetComponent<warrior_function>();
                warriorfunction.ataque = instanciaataque;
                foreach(GameObject jogador in jogadores)
                {
                    if(jogador.GetComponent<NetworkObject>().IsOwnedByServer)
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
                        var focofunctionobject = jogadorlocal.transform.Find("focofunction");
                        var focofunctionscript = focofunctionobject.GetComponent<foco_function>();
                        focofunctionscript.ataque = instanciaataque;
                        var gunbowscript = ataquenetwork?.GetComponent<gunbow>();
                        if(gunbowscript != null)
                        {
                            gunbowscript.scriptwarrior = warrior;
                        }
                        //ataquenetwork?.gameObject.SetActive(false);
                        var atirar = this.transform.Find("atirar").gameObject;
                        focofunctionscript.atirar = atirar;
                        var left = this.transform.Find("Select Left").gameObject;
                        focofunctionscript.esquerdo = left;
                        var right = this.transform.Find("Select Right").gameObject;
                        focofunctionscript.direito = right;
                        var selectwarrior = this.transform.Find("Select Warrior").gameObject;
                        focofunctionscript.selectwarrior = selectwarrior;
                        var foco = this.transform.Find("foco").gameObject;
                        focofunctionscript.botaofoco = foco;
                    }
                    else
                    {
                        StartCoroutine(aguardarcanvasdocliente(jogador));
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator aguardarataqueserver(GameObject jogador, foco_function focofunctionscript)
    {
        var canvas = jogador.transform.Find("Canvas(Clone)");
        instanciaataqueserver = canvas.transform.Find("Ataque(Clone)")?.gameObject;
        ataquenetworkserver = instanciaataqueserver?.GetComponent<NetworkObject>();
        while(instanciaataqueserver == null && ataquenetworkserver == null)
        {
            instanciaataqueserver = canvas.transform.Find("Ataque(Clone)")?.gameObject;
            ataquenetworkserver = instanciaataqueserver?.GetComponent<NetworkObject>();
            yield return null;
        }
        focofunctionscript.ataque = instanciaataqueserver;
        var gunbowscript = ataquenetworkserver.GetComponent<gunbow>();
        scriptwarriortemvalor += gunbowscript.receberscriptwarrior;
        if(IsClient && !IsHost && !IsServer)
        {
            GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject jogadorlocal in jogadores)
            {
                if(jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    var canvasjogadorlocal = jogadorlocal.transform.Find("Canvas(Clone)")?.gameObject;
                    while(canvasjogadorlocal == null)
                    {
                        canvasjogadorlocal = jogadorlocal.transform.Find("Canvas(Clone)")?.gameObject;
                        yield return null;
                    }
                    var warriorlocal = canvasjogadorlocal.transform.Find("warrior");
                    instanciaataque = canvasjogadorlocal.transform.Find("Ataque(Clone)")?.gameObject;
                    while(instanciaataque == null)
                    {
                        instanciaataque = canvasjogadorlocal.transform.Find("Ataque(Clone)")?.gameObject;
                        yield return null;
                    }
                    gunbowscript = instanciaataque.GetComponent<gunbow>();
                }
            }
        }
        gunbowscript.scriptwarrior = warrior;
        scriptwarriortemvalor?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    private void instanciarataquenoclienteServerRpc(ServerRpcParams serverrpcparams = default)
    {
        var clientId = serverrpcparams.Receive.SenderClientId;
        GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject jogadorlocal in jogadores)
        {
            if (jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
                var canvas = jogadorlocal.transform.Find("Canvas(Clone)");
                if(!canvas.transform.Find("Ataque(Clone)"))
                {
                    instanciaataque = Instantiate(ataque, canvas.transform);
                    ataquenetwork = instanciaataque.GetComponent<NetworkObject>();
                    ataquenetwork.SpawnWithOwnership(clientId);
                    
                    // Acessa o RectTransform de ataquenetwork
                    RectTransform ataqueRectTransform = ataquenetwork.GetComponent<RectTransform>();

                    // Acessa o RectTransform de warrior
                    var warrior = canvas.transform.Find("warrior");
                    RectTransform warriorRectTransform = warrior.GetComponent<RectTransform>();

                    // Copia posição, largura e altura
                    ataqueRectTransform.anchoredPosition = warriorRectTransform.anchoredPosition;
                    ataqueRectTransform.sizeDelta = warriorRectTransform.sizeDelta;
                    ataquenetwork.transform.localScale = warrior.localScale;
                    // Define como filho de canvas
                    ataquenetwork.transform.SetParent(canvas.transform, false);
                }
            }

        }
    }

    [ClientRpc]
    private void notificarclientessobrevaloresdecanvasClientRpc()
    {
        GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject jogador in jogadores)
        {
            if(jogador.GetComponent<NetworkObject>().IsOwnedByServer)
            {
                var canvas = jogador.transform.Find("Canvas(Clone)");
                warrior = canvas.transform.Find("warrior").gameObject;
                var warriorfunction = warrior.GetComponent<warrior_function>();
                warriorfunction.ataque = instanciaataqueserver;
                jogadorlocal = jogador;
                var macasradesenrolado = canvas.transform.Find("mascara para pergaminho desenrolado");
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
        var canvasserver = jogadorlocal.transform.Find("Canvas(Clone)").gameObject;
        var fococuntionobject = jogadorlocal.transform.Find("focofunction").gameObject;
        var focofunctionscript = fococuntionobject.GetComponent<foco_function>();
        StartCoroutine(aguardarataqueserver(jogadorlocal, focofunctionscript));
        var atirar = canvasserver.transform.Find("atirar").gameObject;
        focofunctionscript.atirar = atirar;
        var left = canvasserver.transform.Find("Select Left").gameObject;
        focofunctionscript.esquerdo = left;
        var right = canvasserver.transform.Find("Select Right").gameObject;
        focofunctionscript.direito = right;
        var selectwarrior = canvasserver.transform.Find("Select Warrior").gameObject;
        focofunctionscript.selectwarrior = selectwarrior;
        var foco = canvasserver.transform.Find("foco").gameObject;
        focofunctionscript.botaofoco = foco;
        clientrpcnotificarsobrevaloresdecanvasaconteceu = true;
    }
}
