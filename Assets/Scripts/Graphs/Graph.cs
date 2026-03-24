/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Navegacion
{

    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract class for graphs
    /// </summary>
    public abstract class Graph : MonoBehaviour
    {
        // Métricas A*
        public int nodosIterados = 0;
        public int nodosVisitados = 0;
        public float costeCamino = 0f;
        public float duracionAstarMS = 0f;

        // Aquí el grafo entero es representado con estas listas, que luego puede aprovechar el algoritmo A*.
        // El pseudocódigo de Millington no asume que tengamos toda la información del grafo representada y por eso va guardando registros de los nodos que visita... pero si nos es posible, OPCIONALMENTE podemos usar estas variables como una CACHÉ donde tener toda la información
        public GameObject vertexPrefab;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbourVertex;
        protected List<List<float>> costs;
        protected bool[,] mapVertices;
        protected float[,] costsVertices; // Costes reales (g)... aunque también se podría crear una clase para las conexiones y poner los costes ahí, como en el pseudocódigo de Millington. Esto está 'optimizado' porque sabemos que trabajamos con una rejilla...
        protected int numCols, numRows;

        // Esto de la heurística es para algoritmos de búsqueda con estrategias informadas como A*, naturalmente.
        // Un delegado especifica la cabecera de una función, la que sea, que cumpla con esos parámetros y devuelva ese tipo.
        // Cuidado al implementarlas, porque no puede ser que la distancia -por ejemplo- entre dos casillas tenga una heurística más cara que el coste real de navegar de una a otra.
        public delegate float Heuristic(Vertex a, Vertex b);

        public float DistanciaEuclidiana(Vertex a, Vertex b) {
            return Vector3.Distance(a.transform.position, b.transform.position);
        }
        public float DistanciaChebyshev(Vertex a, Vertex b) {
            return Mathf.Max(Mathf.Abs(a.transform.position.x - b.transform.position.x), Mathf.Abs(a.transform.position.z - b.transform.position.z));
        }


        // Used for getting path in frames
        public List<Vertex> path;


        public virtual void Start()
        {
            Load();
        }

        public virtual void Load() { }

        public virtual int GetSize()
        {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual void UpdateVertexCost(Vector3 position, float costMultipliyer) { }

        public virtual Vertex GetNearestVertex(Vector3 position)
        {
            if (ReferenceEquals(vertices, null) || vertices.Count == 0)
            {
                Debug.LogWarning("El grafo no tiene vértices cargados.");
                return null;
            }

            Vertex nearestVertex = null;
           
            float minDistanceSqr = float.MaxValue; 

            foreach (Vertex v in vertices)
            {
                Vector3 offset = v.transform.position - position;
                float distSqr = offset.sqrMagnitude;

                if (distSqr < minDistanceSqr)
                {
                    minDistanceSqr = distSqr;
                    nearestVertex = v;
                }
            }

            return nearestVertex;
        }

        public virtual GameObject GetRandomPos()
        {
            return null;
        }

        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new Vertex[0];
            return neighbourVertex[v.id].ToArray();
        }

        public virtual float[] GetNeighboursCosts(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new float[0];

            Vertex[] neighs = neighbourVertex[v.id].ToArray();
            float[] costsV = new float[neighs.Length];
            for (int neighbour = 0; neighbour < neighs.Length; neighbour++) {
                int j = (int)Mathf.Floor(neighs[neighbour].id / numCols);
                int i = (int)Mathf.Floor(neighs[neighbour].id % numCols);
                costsV[neighbour] = costsVertices[j, i];
            }

            return costsV;
        }

        // Encuentra caminos óptimos
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO BFS
            return new List<Vertex>();
        }

        // No encuentra caminos óptimos
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO DFS
            return new List<Vertex>();
        }

        public List<Vertex> GetPathAstar(GameObject srcO, GameObject dstO, Heuristic h = null)
        {
            nodosIterados = 0;
            nodosVisitados = 0;
            costeCamino = 0f;
            duracionAstarMS = 0f;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Vertex start = GetNearestVertex(srcO.transform.position);
            Vertex goal = GetNearestVertex(dstO.transform.position);
            if(start == null || goal == null) {
                print ("ERROR: El nodo de origen o destino no es un vértice del grafo. Asegúrate de que el objeto tiene un componente 'Vertex' y que ese componente está bien configurado.");
                return new List<Vertex>();   
            }
            if (h == null) h = DistanciaEuclidiana;

            BinaryHeap<Vertex> openList = new BinaryHeap<Vertex>();
            
            int[] parent = new int[vertices.Count];
            float[] cost_g = new float[vertices.Count];

            for (int i = 0; i < cost_g.Length; i++) cost_g[i] = float.MaxValue;

            int startId = start.id;
            cost_g[startId] = 0;
            start.cost = 0 + h(start, goal); 
            parent[startId] = -1;
            
            openList.Add(start);
            nodosVisitados++;

            while (openList.Count > 0)
            {
                Vertex v = openList.Remove();
                nodosIterados++;

                if (v.id == goal.id)
                {
                    costeCamino = cost_g[goal.id];
                    sw.Stop();
                    duracionAstarMS = (float)sw.Elapsed.TotalMilliseconds;
                    return BuildPath(start.id, goal.id, ref parent);
                }

                Vertex[] neighbours = GetNeighbours(v);
                float[] costsV = GetNeighboursCosts(v);

                for (int i = 0; i < neighbours.Length; i++)
                {
                    Vertex next = neighbours[i];
                    float newCostG = cost_g[v.id] + costsV[i];

                    if (newCostG < cost_g[next.id])
                    {
                        bool isNew = (cost_g[next.id] == float.MaxValue);
                        
                        cost_g[next.id] = newCostG;
                        parent[next.id] = v.id;
                        
                        next.cost = newCostG + h(next, goal); 

                        if (!openList.Contains(next))
                        {
                            openList.Add(next);
                            if (isNew) nodosVisitados++;
                        }
                    }
                }
            }
            sw.Stop();
            duracionAstarMS = (float)sw.Elapsed.TotalMilliseconds;
            return new List<Vertex>();
        }
        public List<Vertex> Smooth(List<Vertex> inputPath)
        {
            if (inputPath == null || inputPath.Count <= 2)
                return inputPath;

            List<Vertex> outputPath = new List<Vertex>();
            outputPath.Add(inputPath[0]);

            int actualIndex = 0;

            while (actualIndex < inputPath.Count - 1)
            {
                int farthestVisibleIndex = actualIndex + 1;

                //Buscar el nodo más lejano del camino con visión directa
                for (int i = inputPath.Count - 1; i > actualIndex + 1; i--)
                {
                    Vector3 origen = inputPath[actualIndex].transform.position + Vector3.up * 1.5f;
                    Vector3 destino = inputPath[i].transform.position + Vector3.up * 1.5f;
                    Vector3 dir = destino - origen;

                    //Comprobar si hay obstáculos en línea recta.
                    if (!Physics.Raycast(origen, dir.normalized, dir.magnitude - 0.1f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                    {
                        farthestVisibleIndex = i;
                        break;
                    }
                }

                outputPath.Add(inputPath[farthestVisibleIndex]);
                actualIndex = farthestVisibleIndex;
            }

            return outputPath; 
        }

        // Reconstruir el camino, dando la vuelta a la lista de nodos 'padres' /previos que hemos ido anotando
        private List<Vertex> BuildPath(int srcId, int dstId, ref int[] prevList)
        {
            List<Vertex> path = new List<Vertex>();

            if (dstId < 0 || dstId >= vertices.Count) 
                return path;

            int prev = dstId;
            do
            {
                path.Add(vertices[prev]);
                prev = prevList[prev];
            } while (prev != srcId);
            return path;
        }

    }
}
