/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Inform�tica de la Universidad Complutense de Madrid (Espa�a).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using UnityEngine;

namespace UCM.IAV.Navegacion
{

    public class MinoManager : MonoBehaviour
    {
        public GameObject minotaurCentinela;

        public GameObject minotaurMerodeos;

        private Graph graph;

        public int numMinos = 1;

        private void Start()
        {
            numMinos = GameManager.instance.getNumMinos();
            StartUp();
        }

        void StartUp()
        {
            GameObject graphGO = GameObject.Find("GraphGrid");

            if (graphGO != null)
                graph = graphGO.GetComponent<GraphGrid>();

            for (int i = 0; i < numMinos; i++)
                GenerateMino();
        }

        void GenerateMino()
        {
            int randomNum = Random.Range(0, 2);
            GameObject minotaur = randomNum == 0 ? minotaurCentinela : minotaurMerodeos;

            GameObject minoGO = Instantiate(minotaur, graph.GetRandomPos().transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
        }
    }
}
