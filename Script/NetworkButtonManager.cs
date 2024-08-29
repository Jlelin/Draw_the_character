using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using Unity.Netcode.Transports.UTP;

public class NetworkButtonManager : MonoBehaviour
{
    public Button hostButton, clientButton;
    public GameObject playerCharacterPrefab, canvas;
    public static GameObject canvasHostClient; // Prefab do player que você vai instanciar
    public SpriteRenderer terreno;
    public Camera maincamera;
    public Transform cameratransform;
    void Awake()
    {
        canvasHostClient = canvas;
    }
    private void Start()
    {
        // Adiciona os ouvintes de clique para os botões
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
    }

    private void StartHost()
    {
        // Inicia o NetworkManager como Host
        NetworkManager.Singleton.StartHost();
        ConfigurePlayerCharacter();
        canvas.SetActive(false);

        // Configura as variáveis no PlayerCharacter
    }

    private void StartClient()
    {
        string adress = toget_ip.instance.colocarip();
        UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.SetConnectionData(adress, 7777);
        // Inicia o NetworkManager como Cliente
        NetworkManager.Singleton.StartClient();
        canvas.SetActive(false);

        // Configura as variáveis no PlayerCharacter
    }

    public void ConfigurePlayerCharacter()
    {
        // Verifica se o NetworkManager está configurado como Host ou Cliente
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            // Obtém o objeto jogador local
            var player = NetworkManager.Singleton.LocalClient.PlayerObject;

            if (player != null)
            {
                // Acessa o transform do objeto jogador
                var playerTransform = player.transform;

                // Encontre um filho específico pelo nome
                Transform specificChild = playerTransform.Find("focofunction");

                if (specificChild != null)
                {
                    var focofunction = specificChild.gameObject.GetComponent<foco_function>();
                    focofunction.maincamera = maincamera;
                    focofunction.maincamera_position = cameratransform;
                    focofunction.terreno = terreno;
                }
                else
                {
                    Debug.LogError("Filho específico não encontrado!");
                }
            }
        }
    }
}
