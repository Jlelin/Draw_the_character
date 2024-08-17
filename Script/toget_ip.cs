using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class toget_ip : MonoBehaviour
{
    public static toget_ip instance;
    public TextMeshProUGUI sometext;
    public TMP_InputField inputfieldip;

    private static readonly string[] IpPrefixes = { "192.168", "10.", "172." };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void Start()
    {
        string localIP = await GetLocalIPv4Async();
        sometext.text = localIP;

        if (localIP != "IP não encontrado")
        {
            UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            //utp.SetConnectionData(localIP, 7777);
        }
    }

    private async Task<string> GetLocalIPv4Async()
    {
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var wifiInterface = interfaces.FirstOrDefault(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && ni.OperationalStatus == OperationalStatus.Up);
            var preferredInterface = wifiInterface ?? interfaces.FirstOrDefault(ni => ni.OperationalStatus == OperationalStatus.Up);

            if (preferredInterface != null)
            {
                var ipProps = preferredInterface.GetIPProperties();
                var ipv4Addresses = ipProps.UnicastAddresses
                    .Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select(ip => ip.Address.ToString())
                    .ToList();

                foreach (var ip in ipv4Addresses)
                {
                    if (IsLocalNetworkIP(ip) && await CanConnectToIPAsync(ip))
                    {
                        return ip;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro ao obter IP: {ex.Message}");
        }
        return "IP não encontrado";
    }

    private async Task<bool> CanConnectToIPAsync(string ipAddress)
    {
        try
        {
            using (var client = new TcpClient())
            {
                // Definindo um timeout de 3000 ms para a conexão
                var connectTask = client.ConnectAsync(ipAddress, 7777);
                var completedTask = await Task.WhenAny(connectTask, Task.Delay(3000));
                return completedTask == connectTask;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro ao conectar ao IP: {ipAddress}, Exceção: {ex.Message}");
            return false;
        }
    }

    private bool IsLocalNetworkIP(string ip)
    {
        // Verifica se o IP começa com um dos prefixos locais
        return IpPrefixes.Any(prefix => ip.StartsWith(prefix));
    }

    public string colocarip()
    {
        return inputfieldip.text;
    }
}
