using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerUIManager : NetworkBehaviour
{
    [SerializeField] private Canvas playerUI; // UI do jogador
    [SerializeField] private GameObject botoesguerreiro;
    [SerializeField] private UnityEngine.UI.Image pergaminho;

    // NetworkVariables para sincronizar os estados entre host e clientes
    public NetworkVariable<bool> botoesguerreiroActive = new NetworkVariable<bool>(
        default, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<bool> pergaminhoEnroladoEnabled = new NetworkVariable<bool>(
        default, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<bool> pergaminhoDesenroladoEnabled = new NetworkVariable<bool>(
        default, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    private void Start()
    {
        // Sincroniza a UI inicialmente com os valores das NetworkVariables
        /*botoesguerreiro.SetActive(botoesguerreiroActive.Value);
        pergaminho.enabled = pergaminhoEnroladoEnabled.Value;*/

        if (playerUI != null)
        {
            Transform desenrolado = playerUI.transform.Find("mascara para pergaminho desenrolado");
            botoesguerreiro = desenrolado.gameObject;
            botoesguerreiro.SetActive(botoesguerreiroActive.Value);

            Transform enrolado = playerUI.transform.Find("pergaminho enrolado");
            pergaminho = enrolado.GetComponent<UnityEngine.UI.Image>();
            pergaminho.enabled = pergaminhoEnroladoEnabled.Value;
        }
    }

    void Update()
    {
        // Executa apenas no cliente
        if (IsClient)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                NetworkObject objrede = obj.GetComponent<NetworkObject>();

                // Ignora o cliente local
                if (objrede.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    continue;
                }

                // Ajuste da UI do outro cliente
                Transform canvasobject = obj.transform.Find("Canvas");
                playerUI = canvasobject.GetComponent<Canvas>();
                playerUI.enabled = false;

                Transform desenrolado = canvasobject.transform.Find("mascara para pergaminho desenrolado");
                botoesguerreiro = desenrolado.gameObject;
                botoesguerreiro.SetActive(botoesguerreiroActive.Value);

                Image pergaminhoenrolado = botoesguerreiro.GetComponent<Image>();
                pergaminhoenrolado.enabled = pergaminhoDesenroladoEnabled.Value;

                Transform enrolado = canvasobject.transform.Find("pergaminho enrolado");
                pergaminho = enrolado.GetComponent<UnityEngine.UI.Image>();
                pergaminho.enabled = pergaminhoEnroladoEnabled.Value;
            }
        }

        // Executa apenas no servidor
        if (IsServer)
        {
            botoesguerreiroActive.Value = botoesguerreiro.activeSelf;
            pergaminhoEnroladoEnabled.Value = pergaminho.enabled;
        }
    }
}
