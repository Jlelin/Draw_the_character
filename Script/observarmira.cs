using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class observarmira : NetworkBehaviour
{
    private Transform guerreirocinemachine;
    public static GameObject mira;
    public Transform guerreiroPrincipal;
    public Vector3 posicaodamira; // Referência ao transform do guerreiro principal
    public warrior_function warriorfunction;
    public DragCentralButton arrastarbotaocentral;

    private float initialAngle = 90f; // Ângulo inicial do guerreiro principal (olhando para a direita)

    void Awake()
    {
        mira_ativada.bancoposicaomira += guardarposicaomira;
        arrastarbotaocentral = FindFirstObjectByType<DragCentralButton>();
        warriorfunction = arrastarbotaocentral.warrior.GetComponent<warrior_function>();
    }

    void Start()
    {
        // Define a rotação inicial do guerreiro principal para olhar para a direita
        for (int i = 0; i < warrior_function.guerreiro.Length; i++)
        {
            guerreirocinemachine = warrior_function.guerreiro[i].transform;
            if (warriorfunction.cinemachinecamera.Follow == guerreirocinemachine)
            {
                guerreiroPrincipal = warrior_function.guerreiro[i].transform;

                // Define a rotação inicial para olhar para a direita
                guerreiroPrincipal.rotation = Quaternion.Euler(new Vector3(0, 0, -90)); // Olhando para a direita
            }
        }
    }

    void Update()
    {
        // Verifica se a mira está ativada
        if (mira.activeSelf)
        {
            // Verifica se o guerreiro principal foi configurado
            if (guerreiroPrincipal != null)
            {
                
                // Calcula a direção da mira em relação ao guerreiro principal
                Vector3 direction = mira.transform.position - guerreiroPrincipal.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Ajusta o ângulo calculado considerando a rotação inicial
                float adjustedAngle = angle - initialAngle;
                if(posicaodamira != mira.transform.position)
                {
                    // Define a rotação do guerreiro para olhar na direção da mira
                    guerreiroPrincipal.rotation = Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
                }
            }
        }
    }

    public void guardarposicaomira()
    {
        mira.transform.position = gunbow.miras;
        posicaodamira = mira.transform.position;
    }
}
