using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balao : MonoBehaviour
{
    public Transform guerreiro;
    public Vector3 diferencaInicial;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        // Se o guerreiro for nulo, use a diferença fornecida
        if (guerreiro != null)
        {
            // Calcula e armazena a diferença inicial entre as posições
            diferencaInicial = new Vector3(-1.18f, 1.008f, 0); // Ajuste a diferença conforme necessário
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (guerreiro != null)
        {
            // Calcula a posição desejada do Balao com base na diferença inicial
            Vector3 posicaoDesejada = guerreiro.position + diferencaInicial;

            // Define a posição do Balao diretamente para a posição desejada
            transform.position = posicaoDesejada;
        }
    }
}
