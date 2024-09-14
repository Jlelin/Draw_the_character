using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class botaodesenho : NetworkBehaviour
{
    public int apertado_botao;
    public static GameObject paradestruir;
    public GameObject pergamnihoenrolado, papelaberto, joystick, warrior, focos, select, right, left, xzinho;
    public GameObject escudoespada, armafogo, desarmado, arqueiro, mascara;
    private Image maskImage;
    private Image pergaminhoImage; // Adicione uma referência para o Image do pergaminho
    public foco_function foco;
    public Canvas canvas; // Adicione o Canvas aqui

    void Start()
    {
        pergamnihoenrolado.SetActive(false);
        papelaberto.SetActive(false);

        // Inicialize maskImage e Mask componente aqui se necessário
        maskImage = mascara.GetComponent<Image>();
        maskImage.enabled = false;

        Mask componentemascara = mascara.GetComponent<Mask>();
        componentemascara.enabled = false;

        // Inicialize a referência ao Image do pergaminho
        pergaminhoImage = pergamnihoenrolado.GetComponent<Image>();
        pergaminhoImage.enabled = false;
    }

    void Update()
    {
        if (apertado_botao == 5)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    foco.apertado_botao = 1;
                    foco.Update();
                    foco.apertado_botao = 0;
                    // Ativa os GameObjects e Image
                    pergamnihoenrolado.SetActive(true);
                    papelaberto.SetActive(true);
                    xzinho.SetActive(true);
                    escudoespada.SetActive(true);
                    arqueiro.SetActive(true);
                    desarmado.SetActive(true);
                    armafogo.SetActive(true);

                    // Ativa o componente Image do pergaminho
                    pergaminhoImage.enabled = true;
                    maskImage.enabled = true;
                    Mask componentemascara = mascara.GetComponent<Mask>();
                    componentemascara.enabled = true;

                    joystick.SetActive(false);
                    warrior.SetActive(false);
                    focos.SetActive(false);
                    select.SetActive(false);
                    right.SetActive(false);
                    left.SetActive(false);

                    apertado_botao = 0;
                    this.gameObject.SetActive(false);

                    if (paradestruir != null)
                    {
                        Destroy(paradestruir);
                    }
                }
            }
        }
    }

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }
}
