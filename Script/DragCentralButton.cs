using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Unity.Netcode;
using System.Linq;
using System.Collections.Generic;


public class DragCentralButton : NetworkBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public NetworkVariable<ulong> warriorfatherdoclienteID = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> guerreiroID = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> guerreiroInstanciadoID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<ulong> avoID = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> balaoID = new NetworkVariable<ulong>();
    public DragCentralButton[] instanciasderede;
    public GameObject[] todosobjetos, pergaminhosdesenrolados, jogadores;
    public observarmira observarmira;
    public static int indices = 0, indicescliente = 0;
    private bool isButtonPressed = false;
    private int maisumguerreiro;
    public static int tamanho, tamanhocliente;
    private Vector3 scaleNormal = Vector3.one; // Escala normal do botão
    private Coroutine shrinkCoroutine;
    private Vector3 offset, posicao_botaodesenho;
    private RectTransform position;
    public GameObject[] guerreiros, baloes, filhosdemascarapergaminho;
    public GameObject warriorsParent;
    public static GameObject paiinstanciado, guerreiroInstanciado; 
    public GameObject mask, escudoespada, arcoflecha, desarmado, armado, pergaminho_enrolado, desenrolado; // Objetos relacionados
    public GameObject left, right, select_, warrior, focus, joystick, botaodedesenho, canvas, camaracharacter, xzinho;
    public GameObject ataquebutton, mira, playerprefab, balaoInstanciado;
    public static GameObject mirainstance;
    public static NetworkObject instanciaguerreiro, instanciamira, instanciarpai;
    public botaodesenho desenho;
    public qual_guerreiro guerreiroesquerdo;
    public qual_guerreirodireito guerreirodireito;
    public warrior_function warriorfunction;
    public sowrdshieldfight escudoespadaluta;
    public NetworkObject jogador;
    public int tamanhototal  = 0;
    private string tagCentral;

    [Header("Scale Settings")]
    public float scaleDuration = 2f; // Duração da animação de escala
    public float smallSizeThreshold ; // Limite para determinar quando o botão está pequeno]
    [SerializeField]
    private bool hasInstantiated = false; // Controle para garantir que a instância ocorra apenas uma vez

    private AnimationCurve scaleCurve; // Curva de animação para controle de escala
    private ScaleType currentScaleType; // Tipo de variação de escala atual

    [Header("Score")]
    public float totalScore = 0f; // Pontuação total acumulada do jogador

    private void Awake() 
    {
        guerreiroesquerdo = FindFirstObjectByType<qual_guerreiro>();
        guerreirodireito = FindFirstObjectByType<qual_guerreirodireito>();
        desenho = FindFirstObjectByType<botaodesenho>();
        warriorfunction = FindFirstObjectByType<warrior_function>();
        position = desenho.GetComponent<RectTransform>();
        posicao_botaodesenho = position.position;


    }

    void Start()
    {
        maisumguerreiro = 0;
        tamanho = 0;
    }

    void Update()
    {
        GameObject jogadorobject = jogador.gameObject;
        if (!jogadorobject.transform.Find("warrior's father(Clone)"))
        {
            if(IsServer)
            {
                paiinstanciado = Instantiate(warriorsParent);
                paiinstanciado.transform.position = new Vector3(0, 0, 0);
                instanciarpai = paiinstanciado.GetComponent<NetworkObject>();
                instanciarpai.Spawn();
            }
            else if(IsClient)
            {
                warriorfatherhaveafatherServerRpc();
            }
            if(IsServer)
            {
                paiinstanciado.transform.SetParent(jogador.transform, false);
                avoID.Value = jogador.GetComponent<NetworkObject>().NetworkObjectId;
            }
        }
        if (isButtonPressed)
        {
            // A escala é controlada pela coroutine
        }

        // Verificar se o botão atingiu o tamanho pequeno
        if (hasInstantiated && transform.localScale.y < smallSizeThreshold)
        {
            InstantiateBalaoEGuerreiro();
            hasInstantiated = false; // Garantir que a instância ocorra apenas uma vez
        }
        //originalPosition = botoes_pergaminho.originalscale; 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        hasInstantiated = true;
        // Indicar que o botão está pressionado
        isButtonPressed = true;

        // Salvar a escala normal do botão quando é pressionado
        scaleNormal = transform.localScale;

        // Inicializar a escala atual com a escala normal
        Vector3 currentScale = scaleNormal;

        // Selecionar aleatoriamente um tipo de variação de escala
        currentScaleType = GetRandomScaleType();

        // Atualizar a curva de escala com base no tipo selecionado
        scaleCurve = GetCurveForType(currentScaleType);

        // Iniciar a diminuição contínua da escala
        shrinkCoroutine = StartCoroutine(ShrinkButton());

        // Calcular o offset para arrastar o botão
        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out var globalMousePos);
        offset = transform.position - globalMousePos;

        // Ativar o comportamento desejado ao pressionar o botão
        Image maskImage = mask.GetComponent<Image>();
        Mask mascara = mask.GetComponent<Mask>();
        if (maskImage != null)
        {
            // Desativar o componente Image
            maskImage.enabled = false;
            mascara.enabled = false;
            if (escudoespada != botoes_pergaminho.botaocentral)
            {
                escudoespada.SetActive(false);
            }
            if (arcoflecha != botoes_pergaminho.botaocentral)
            {
                arcoflecha.SetActive(false);
            }
            if (desarmado != botoes_pergaminho.botaocentral)
            {
                desarmado.SetActive(false);
            }
            if (armado != botoes_pergaminho.botaocentral)
            {
                armado.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("O componente Image não foi encontrado no GameObject mask.");
        }

        // Iniciar a troca de variações de escala durante a animação
        StartCoroutine(ChangeScaleTypeRandomly());
    }

    IEnumerator ChangeScaleTypeRandomly()
    {
        while (isButtonPressed)
        {
            float waitTime = UnityEngine.Random.Range(0.05f, scaleDuration); // Tempo aleatório para trocar a variação
            yield return new WaitForSeconds(waitTime);

            // Escolher uma nova variação de escala aleatoriamente
            currentScaleType = GetRandomScaleType();

            // Atualizar a curva de escala com base no novo tipo selecionado
            scaleCurve = GetCurveForType(currentScaleType);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Indicar que o botão não está mais pressionado
        isButtonPressed = false;

        // Cancelar a diminuição contínua da escala quando o botão é solto
        if (shrinkCoroutine != null)
        {
            StopCoroutine(shrinkCoroutine);
        }

        // Calcular e somar a pontuação com base na escala atual no eixo Y
        float score = CalculateScore();
        totalScore += score; // Acumula a pontuação total
        transform.localScale = scaleNormal;

        // Resetar a variável de controle de instância
        hasInstantiated = true;

        // Redefinir a posição do botão para a posição original
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isButtonPressed)
        {
            // Move o botão para a nova posição do ponteiro com o deslocamento
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out var globalMousePos);
            transform.position = globalMousePos + offset;
        }
    }

    IEnumerator ShrinkButton()
    {
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float scaleMultiplier = scaleCurve.Evaluate(elapsedTime / scaleDuration);
            Vector3 currentScale = scaleNormal * scaleMultiplier;
            transform.localScale = currentScale;
            yield return null;
        }

        // Após a animação, defina a escala para a normal
        transform.localScale = scaleNormal;
    }

    private float CalculateScore()
    {
        // Calcula a pontuação com base na escala atual do botão no eixo Y
        float currentScaleY = transform.localScale.y;
        // Ajuste para que a pontuação seja positiva
        float score = 1f - currentScaleY;
        return score; // Multiplicando por 100 para tornar a pontuação mais significativa
    }





    private void InstantiateBalaoEGuerreiro()
    {
        transform.localScale = scaleNormal;

        Vector3 fixedPosition = new Vector3(-11.83f, 0.6f, 0.5042808f);

        tagCentral = transform.tag;
        balaoInstanciado = null;

        foreach (GameObject guerreiro in guerreiros)
        {
            if (guerreiro.CompareTag(tagCentral))
            {
                foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
                {
                    NetworkObject objrede = obj.GetComponent<NetworkObject>();
                    if(objrede.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        if(IsClient && !IsHost)
                        {
                            if(instanciarpai == null)
                            {
                                if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(warriorfatherdoclienteID.Value, out var warriorfathernetworkObject))
                                {
                                    instanciarpai = warriorfathernetworkObject;
                                }
                            }
                            if(instanciarpai != null)
                            {
                                if(instanciarpai.IsSpawned)
                                {
                                    paiinstanciado = instanciarpai.gameObject;
                                    instanciadeguerreirosServerRpc(fixedPosition);
                                    
                                }
                            }
                        }
                        if(IsServer)
                        {
                            guerreiroInstanciado = Instantiate(guerreiro, fixedPosition, Quaternion.identity, paiinstanciado.transform);
                            instanciaguerreiro = guerreiroInstanciado.GetComponent<NetworkObject>();
                            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(avoID.Value, out var networkObject))
                            {
                                var avo = networkObject;
                                instanciaguerreiro.SpawnWithOwnership(avo.OwnerClientId);
                            }
                            guerreiroID.Value = instanciaguerreiro.NetworkObjectId;
                            valoresdoguerreirobrancoClientRpc(guerreiroID.Value, fixedPosition, avoID.Value);
                            if (ataquebutton != null && instanciaguerreiro.CompareTag("guerreiroescudoespada"))
                            {
                                sowrdshieldfight scriptataqueespada = instanciaguerreiro.GetComponent<sowrdshieldfight>();
                                scriptataqueespada.ataquebutton = ataquebutton;
                            }
                            if (instanciaguerreiro != null && instanciaguerreiro.CompareTag("guerreiroescudoespada"))
                            {
                                escudoespadaluta = instanciaguerreiro.GetComponent<sowrdshieldfight>();
                                escudoespadaluta.ataquebutton = ataquebutton;
                            }
                            instanciaguerreiro.transform.SetParent(instanciarpai.transform, false);
                            instanciaguerreiro.transform.position = fixedPosition;
                            if(instanciaguerreiro.CompareTag("guerreiroarqueiro") || instanciaguerreiro.CompareTag("guerreirosniper"))
                            {
                                mirainstance = Instantiate(mira);
                                instanciamira = mirainstance.GetComponent<NetworkObject>();
                                objrede = obj.GetComponent<NetworkObject>();
                                if(objrede.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                                {
                                    instanciamira.SpawnWithOwnership(objrede.OwnerClientId);
                                    instanciamira.transform.SetParent(objrede.transform, true);    
                                }
                                if(instanciaguerreiro.CompareTag("guerreiroarqueiro") || instanciaguerreiro.CompareTag("guerreirosniper"))
                                {
                                    instanciamira.GetComponent<movimentar>().mover = joystick.GetComponent<FixedJoystick>();
                                    observarmira = instanciaguerreiro.GetComponent<observarmira>();
                                    observarmira.mira = mira;
                                }
                                if (mira != null)
                                {
                                    movimentar movimento = instanciaguerreiro.GetComponent<movimentar>();
                                    movimento.miraId.Value = mira.GetComponent<NetworkObject>().NetworkObjectId;
                                }
                            }
                        }
                    }
                }
                break;
            }
        }

        foreach (GameObject balao in baloes)
        {
            if (balao.CompareTag(tagCentral))
            {
                if(IsClient && !IsHost)
                {
                    jogadores = GameObject.FindGameObjectsWithTag("Player");
                    foreach(var jogadorlocal in jogadores)
                    {
                        instanciarpai = jogadorlocal.transform.Find("warrior's father(Clone)").GetComponent<NetworkObject>();
                        if (instanciarpai.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                        {
                            balaoInstanciado = Instantiate(balao, fixedPosition, Quaternion.identity, instanciarpai.transform);
                        }
                    }
                    if (guerreiroesquerdo.balao[0] != null)
                    {
                        Array.Resize(ref guerreiroesquerdo.balao, guerreiroesquerdo.balao.Length + 1);
                        tamanhototal = guerreiroesquerdo.balao.Length - 1;
                    }
                    guerreiroesquerdo.balao[tamanhototal] = balaoInstanciado;
                    guerreiroesquerdo.balao_atualizar();
                    balaoInstanciado.transform.position = fixedPosition;
                    botoes_pergaminho.botaocentral.transform.position = botoes_pergaminho.originalscale;
                    left.SetActive(true);
                    right.SetActive(true);
                    select_.SetActive(true);
                    warrior.SetActive(true);
                    joystick.SetActive(true);
                    botaodedesenho.SetActive(true);
                    pergaminho_enrolado.SetActive(false);
                    desenrolado.SetActive(false);
                    break;
                }
                if(IsServer)
                {
                    balaoInstanciado = Instantiate(balao, fixedPosition, Quaternion.identity, paiinstanciado.transform);

                    if (guerreiroesquerdo.balao[0] != null)
                    {
                        Array.Resize(ref guerreiroesquerdo.balao, guerreiroesquerdo.balao.Length + 1);
                        tamanhototal = guerreiroesquerdo.balao.Length - 1;
                    }
                    guerreiroesquerdo.balao[tamanhototal] = balaoInstanciado;
                    guerreiroesquerdo.balao_atualizar();
                    break;
                }
            }
        }

        
        if(IsServer)
        {
            tamanho++;
            Array.Resize(ref warriorfunction.guerreiros, tamanho);
            maisumguerreiro = warriorfunction.guerreiros.Length - 1;
            warriorfunction.guerreiros[maisumguerreiro] = guerreiroInstanciado;
            maisumguerreiro++;
            if (warriorfunction.guerreiros[indices] != null)
            {
                if (warriorfunction.guerreiros.Length - 1 > indices)
                {
                    indices++;
                }
            }

        }
        if (guerreiroInstanciado == null || balaoInstanciado == null)
        {
            Debug.LogWarning("Não foi possível encontrar guerreiro ou balão com a tag do botão central.");
        }
        else
        {
            // Desativar o pergaminho enrolado e desenrolado
            botoes_pergaminho.botaocentral.transform.position = botoes_pergaminho.originalscale;
            left.SetActive(true);
            right.SetActive(true);
            select_.SetActive(true);
            warrior.SetActive(true);
            joystick.SetActive(true);
            botaodedesenho.SetActive(true);
            pergaminho_enrolado.SetActive(false);
            desenrolado.SetActive(false);
            if(IsClient && !IsHost)
            {
                Balao balaoScript = balaoInstanciado.GetComponent<Balao>();
                if (balaoScript != null)
                {
                    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(guerreiroInstanciadoID.Value, out var guerreiroInstanciadonetworkobject))
                    balaoScript.guerreiro = guerreiroInstanciadonetworkobject.transform;
                    balaoScript.diferencaInicial = new Vector3(-1.18f, 1.008f, 0);
                }
            }
            else
            {
                // Configurar o balão e guerreiro instanciado
                Balao balaoScript = balaoInstanciado.GetComponent<Balao>();
                if (balaoScript != null)
                {
                    balaoScript.guerreiro = guerreiroInstanciado.transform;
                    balaoScript.diferencaInicial = new Vector3(-1.18f, 1.008f, 0);
                }
                else
                {
                    Debug.LogWarning("O balão instanciado não possui o componente BalaoScript.");
                }
            }
        }
    }
    [ClientRpc]
    private void valoresdoguerreirodetiroClientRpc()
    {

    }
    [ClientRpc]
    private void valoresdoguerreirobrancoClientRpc(ulong guerreiroID, Vector3 posicaofixa, ulong avoID)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(guerreiroID, out var networkObject2))
        {
            guerreiroInstanciado = networkObject2.gameObject;
        }
        instanciaguerreiro = guerreiroInstanciado.GetComponent<NetworkObject>();
        if(IsServer)
        {
            guerreiroInstanciado.transform.SetParent(instanciarpai.transform, false);
        }
        FixedJoystick fixedJoystick = joystick.GetComponent<FixedJoystick>();
        if (fixedJoystick != null)
        {
            movimentar guerreiroScript = instanciaguerreiro.GetComponent<movimentar>();
            if (guerreiroScript != null)
            {
                guerreiroScript.mover = fixedJoystick;
                guerreiroScript.enabled = false;
                instanciaguerreiro.enabled = false;
                guerreiroInstanciado.SetActive(false);
            }
        }
    }

    [ClientRpc]
    private void guerreirosdewarriorfunctionClientRpc(ulong guerreiroInstanciadoID)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(guerreiroInstanciadoID, out var guerreiroInstanciadonetwork))
        {
            tamanhocliente++;
            Array.Resize(ref warriorfunction.guerreiros, tamanhocliente);
            maisumguerreiro = warriorfunction.guerreiros.Length - 1;
            var newguerreiroInstanciado = guerreiroInstanciadonetwork.gameObject;
            warriorfunction.guerreiros[maisumguerreiro] = newguerreiroInstanciado;
            maisumguerreiro++;
            if (warriorfunction.guerreiros[indicescliente] != null)
            {
                if (warriorfunction.guerreiros.Length - 1 > indicescliente)
                {
                    indicescliente++;
                }
            }
        }
    }

    [ServerRpc]
    private void warriorfatherhaveafatherServerRpc()
    {
        todosobjetos = FindObjectsOfType<GameObject>(true);
        foreach(GameObject objeto in todosobjetos)
        {
            if(objeto.CompareTag("desenrolado"))
            {
                if(!objeto.activeSelf)
                {
                    objeto.SetActive(true);
                }
            }
        }
        pergaminhosdesenrolados = todosobjetos.Where(obj => obj.CompareTag("desenrolado")).ToArray();
        foreach(GameObject desenrolado in pergaminhosdesenrolados)
        {
            if(desenrolado.activeSelf)
            {
                GameObject[] jogadores = todosobjetos.Where(obj => obj.CompareTag("Player")).ToArray();
                foreach(GameObject jogador in jogadores)
                {
                    if (!jogador.transform.Find("warrior's father(Clone)"))
                    {
                        paiinstanciado = Instantiate(warriorsParent);
                        paiinstanciado.transform.position = new Vector3(0, 0, 0);
                        instanciarpai = paiinstanciado.GetComponent<NetworkObject>();
                    }
                    if(instanciarpai != null)
                    {
                        if(!instanciarpai.IsSpawned)
                        {
                            avoID.Value = jogador.GetComponent<NetworkObject>().NetworkObjectId;
                            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(avoID.Value, out var networkObjectavo))
                            {
                                instanciarpai.SpawnWithOwnership(networkObjectavo.OwnerClientId);
                                warriorfatherdoclienteID.Value = instanciarpai.NetworkObjectId;
                            }
                        }
                    }
                    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(avoID.Value, out var networkObject))
                    {
                        paiinstanciado.transform.SetParent(networkObject.transform, true);
                    }
                }
                desenrolado.SetActive(false);
            }
        }
    }

    [ServerRpc]
    private void instanciadeguerreirosServerRpc(Vector3 fixedPosition, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach(var jogadorlocal in jogadores)
        {
            var jogadorlocalnetwork = jogadorlocal.GetComponent<NetworkObject>();
            if (clientId == jogadorlocalnetwork.OwnerClientId)
            {
                foreach (GameObject guerreiro in guerreiros)
                {
                    tagCentral = transform.tag;
                    if (guerreiro.CompareTag(tagCentral))
                    {
                        guerreiroInstanciado = Instantiate(guerreiro, fixedPosition, Quaternion.identity, paiinstanciado.transform);
                        instanciaguerreiro = guerreiroInstanciado.GetComponent<NetworkObject>();
                        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(avoID.Value, out var avonetworkobject))
                        {
                            instanciaguerreiro.SpawnWithOwnership(avonetworkobject.OwnerClientId);
                            instanciarpai = jogadorlocal.transform.Find("warrior's father(Clone)").GetComponent<NetworkObject>();
                            if(instanciarpai.OwnerClientId == clientId)
                            {
                                instanciaguerreiro.transform.SetParent(instanciarpai.transform, false);
                            }
                        }
                    }
                }
            }
        }
        guerreirosdewarriorfunctionClientRpc(instanciaguerreiro.NetworkObjectId);
    }





    private ScaleType GetRandomScaleType()
    {
        ScaleType[] scaleTypes = (ScaleType[])System.Enum.GetValues(typeof(ScaleType));
        int randomIndex = UnityEngine.Random.Range(0, scaleTypes.Length);
        return scaleTypes[randomIndex];
    }



    private AnimationCurve GetCurveForType(ScaleType type)
    {
        AnimationCurve curve;
        Keyframe[] keyframes;
        switch (type)
        {
            case ScaleType.SlowDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.AcceleratedDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.DeceleratedDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.FastDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.SlowThenFastDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.FastThenSlowDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.SlowDecreaseToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.FastDecreaseToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.SmoothDecreaseToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.RapidOscillationToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration * 0.25f, smallSizeThreshold * 1.2f),
                    new Keyframe(scaleDuration * 0.5f, smallSizeThreshold * 0.8f),
                    new Keyframe(scaleDuration * 0.75f, smallSizeThreshold * 1.1f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.SuddenDropToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration * 0.5f, 1f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.SlowOscillationToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration * 0.33f, smallSizeThreshold * 1.2f),
                    new Keyframe(scaleDuration * 0.66f, smallSizeThreshold * 0.8f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            default:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0.5f)
                };
                curve = new AnimationCurve(keyframes);
                break;
        }
        return curve;
    }

    private enum ScaleType
    {
        SlowDecrease,
        AcceleratedDecrease,
        DeceleratedDecrease,
        FastDecrease,
        SlowThenFastDecrease,
        FastThenSlowDecrease,
        SlowDecreaseToThreshold,
        FastDecreaseToThreshold,
        SmoothDecreaseToThreshold,
        RapidOscillationToThreshold,
        SuddenDropToThreshold,
        SlowOscillationToThreshold
    }


}
