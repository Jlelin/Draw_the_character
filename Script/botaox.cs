using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class botaox : MonoBehaviour
{
    public int apertado_botao;
    public GameObject pergaminho_enrolado, desenrolado, left, right, select_, joystick, foco, warrior, desenho, canvas, camaracharacter;
    private RectTransform position;
    private Vector3 originalposition;
    public botaodesenho dedesenho;

    void Awake()
    {
        dedesenho = FindFirstObjectByType<botaodesenho>();
        position = dedesenho.GetComponent<RectTransform>();
        originalposition = position.position;
    }

    void Update()
    {
        if (apertado_botao == 6)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    left.SetActive(true);
                    right.SetActive(true);
                    select_.SetActive(true);
                    joystick.SetActive(true);
                    warrior.SetActive(true);
                    desenho.SetActive(true);
                    pergaminho_enrolado.SetActive(false);
                    desenrolado.SetActive(false);
                    apertado_botao = 0;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }
}
