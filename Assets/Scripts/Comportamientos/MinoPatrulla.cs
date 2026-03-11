using System.Collections.Generic;
using UnityEngine;
using UCM.IAV.Navegacion;

namespace UCM.IAV.Movimiento
{
    public class MinoPatrulla : ComportamientoAgente
    {
        public GraphGrid graphGrid;

        private Vertex nodoDestino;
        private Vertex nodoAnterior;

        private bool inicializado = false;

        public float tiempoEspera = 1.5f;

        public float multiplicadorVelocidad = 0.5f;

        private float tiempoRestante = 0f;
        private bool esperando = false;

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            if (!inicializado)
            {
                InicializarPatrulla();
            }

            if (esperando)
            {
                tiempoRestante -= Time.deltaTime;
                if (tiempoRestante <= 0f)
                {
                    esperando = false;
                }
                else
                {
                    //Si está esperando, devuelve un vector 0 para que frene
                    direccion.lineal = Vector3.zero;
                    return direccion;
                }
            }

            if (nodoDestino != null)
            {
                Vector3 targetPos = nodoDestino.transform.position;
                targetPos.y = transform.position.y; // Evita que mire arriba/abajo

                Vector3 directionToTarget = targetPos - transform.position;
                float distance = directionToTarget.magnitude;

                //Si hemos llegado cerca del vértice destino, buscamos el siguiente y nos paramos a pensar
                if (distance < 0.2f)
                {
                    nodoAnterior = nodoDestino;
                    nodoDestino = ElegirSiguienteNodo(nodoDestino);
                    
                    esperando = true;
                    tiempoRestante = tiempoEspera;
                    
                    direccion.lineal = Vector3.zero;
                    return direccion;
                }

                //Calcular velocidad hacia el objetivo
                direccion.lineal = directionToTarget.normalized * (agente.aceleracionMax * multiplicadorVelocidad);
            }

            return direccion;
        }

        private void InicializarPatrulla()
        {
            if (graphGrid == null)
            {
                GameObject graphGO = GameObject.Find("GraphGrid");
                if (graphGO != null) graphGrid = graphGO.GetComponent<GraphGrid>();
            }

            if (graphGrid != null)
            {
                //Busca la baldosa más cercana a nuestra posición inicial
                nodoAnterior = graphGrid.GetNearestVertex(transform.position);
                //Escoge una baldosa vecina al azar como primer paso
                if (nodoAnterior != null)
                {
                    Vertex[] vecinos = graphGrid.GetNeighbours(nodoAnterior);
                    if (vecinos.Length > 0)
                    {
                        nodoDestino = vecinos[Random.Range(0, vecinos.Length)];
                    }
                }
                inicializado = true;
            }
        }

        private Vertex ElegirSiguienteNodo(Vertex actual)
        {
            Vertex[] vecinos = graphGrid.GetNeighbours(actual);

            if (vecinos == null || vecinos.Length == 0) return actual;

            //Si es un callejón sin salida
            if (vecinos.Length == 1)
            {
                return vecinos[0];
            }

            //Si hay más de un vecino, filtrar el nodo por el que acabamos de venir
            List<Vertex> vecinosValidos = new List<Vertex>();
            foreach (Vertex v in vecinos)
            {
                if (v != nodoAnterior)
                {
                    vecinosValidos.Add(v);
                }
            }

            //Devuelve un vecino aleatorio de las opciones restantes
            if (vecinosValidos.Count > 0)
            {
                return vecinosValidos[Random.Range(0, vecinosValidos.Count)];
            }
            else
            {
                return vecinos[0];
            }
        }
    }
}
