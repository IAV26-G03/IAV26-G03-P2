/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UCM.IAV.Movimiento;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace UCM.IAV.Navegacion
{


    // Posibles algoritmos para buscar caminos en grafos
    // REALMENTE PARA ESTA PRÁCTICA SÓLO SE NECESITA ASTAR, los otros no los usaremos...
    public enum TesterGraphAlgorithm
    {
        BFS, DFS, ASTAR
    }

    public class TheseusGraph : MonoBehaviour
    {
        [SerializeField]
        protected Graph graph;

        [SerializeField]
        private TesterGraphAlgorithm algorithm;

        [SerializeField]
        private bool smoothPath;

        [SerializeField]
        private string vertexTag = "Vertex"; // Etiqueta de un nodo normal

        [SerializeField]
        private string obstacleTag = "Wall"; // Etiqueta de un obstáculo, tipo pared...

        [SerializeField]
        private Color pathColor;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float pathNodeRadius = .3f;

        private bool ariadna;


        bool firstHeuristic = true;
        Camera mainCamera;
        protected GameObject srcObj;
        protected GameObject dstObj;
        protected List<Vertex> path; // La variable con el camino calculado

        protected LineRenderer hilo;
        protected float hiloOffset = 0.2f;

        // Despertar inicializando esto
        public virtual void Awake()
        {
            mainCamera = Camera.main;
            srcObj = GameManager.instance.GetPlayer();
            dstObj = null;
            path = new List<Vertex>();
            hilo = GetComponent<LineRenderer>();
            ariadna = false;

            hilo.startWidth = 0.15f;
            hilo.endWidth = 0.15f;
            hilo.positionCount = 0;
        }

        // Update is called once per frame
        public virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                updateAriadna(!ariadna);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                smoothPath = !smoothPath;
                if (ariadna) CalculatePath();
            }

            if (ariadna)
            {
                if (path != null && path.Count > 0)
                {
                    Transform nextNode = path[path.Count - 1].transform;
                    Vector3 posTeseo = srcObj.transform.position;
                    Vector3 posNodo = nextNode.position;
                    posTeseo.y = 0; posNodo.y = 0;
                    
                    if (Vector3.Distance(posTeseo, posNodo) < 0.5f)
                    {
                        path.RemoveAt(path.Count - 1);
                    }
                }

                if (path != null && path.Count > 0)
                {
                    DibujaHilo();
                }
                else
                {
                    hilo.positionCount = 0;
                }
            }
            else
            {
                if (srcObj != null)
                {
                    srcObj.GetComponent<ControlJugador>().enabled = true;
                    if (srcObj.GetComponent<SeguirCamino>() != null)
                        srcObj.GetComponent<SeguirCamino>().enabled = false;
                }
            }
        }

        public virtual Transform GetNextNode()
        {
            if (path.Count > 0)
                return path[path.Count - 1].transform;

            return null;
        }

        // Dibujado de artilugios en el editor
        // OJO, ESTO SÓLO SE PUEDE VER EN LA PESTAÑA DE SCENE DE UNITY
        virtual public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (ReferenceEquals(graph, null))
                return;

            Vertex v;
            if (!ReferenceEquals(srcObj, null))
            {
                Gizmos.color = Color.green; // Verde es el nodo inicial
                v = graph.GetNearestVertex(srcObj.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
            if (!ReferenceEquals(dstObj, null))
            {
                Gizmos.color = Color.red; // Rojo es el color del nodo de destino
                v = graph.GetNearestVertex(dstObj.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
            int i;
            Gizmos.color = pathColor;
            for (i = 0; i < path.Count; i++)
            {
                v = path[i];
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
                if (smoothPath && i != 0)
                {
                    Vertex prev = path[i - 1];
                    Gizmos.DrawLine(v.transform.position, prev.transform.position);
                }
            }
        }

        // Mostrar el camino calculado
        public void ShowPathVertices(List<Vertex> path, Color color)
        {
            int i;
            for (i = 0; i < path.Count; i++)
            {
                Vertex v = path[i];
                Renderer r = v.GetComponent<Renderer>();
                if (ReferenceEquals(r, null))
                    continue;
                r.material.color = color;
            }
        }

        // Cuantificación, cómo traduce de posiciones del espacio (la pantalla) a nodos
        private GameObject GetNodeFromScreen(Vector3 screenPosition)
        {
            GameObject node = null;
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit h in hits)
            {
                if (!h.collider.CompareTag(vertexTag) && !h.collider.CompareTag(obstacleTag))
                    continue;
                node = h.collider.gameObject;
                break;
            }
            return node;
        }

        // Dibuja el hilo de Ariadna
        public virtual void DibujaHilo()
        {
            hilo.positionCount = path.Count + 1;
            hilo.SetPosition(0, new Vector3(srcObj.transform.position.x, srcObj.transform.position.y + hiloOffset, srcObj.transform.position.z));

            for (int i = path.Count - 1; i >= 0; i--)
            {
                Vector3 vertexPos = new Vector3(path[i].transform.position.x, path[i].transform.position.y + hiloOffset, path[i].transform.position.z);
                hilo.SetPosition(path.Count - i, vertexPos);
            }
        }

        void updateAriadna(bool ar)
        {
            ariadna = ar;
            hilo.enabled = ariadna;
            ControlJugador controljg = GameManager.instance.GetPlayer().GetComponent<ControlJugador>();
            controljg.ToggleAriadna();

            if (ariadna)
            {
                if (srcObj == null) srcObj = GameManager.instance.GetPlayer();
                if (dstObj == null) dstObj = GameManager.instance.GetExitNode();

                srcObj.GetComponent<ControlJugador>().enabled = false;
                srcObj.GetComponent<SeguirCamino>().graph = this;
                srcObj.GetComponent<SeguirCamino>().enabled = true;

                CalculatePath();
            }
        }

        private void CalculatePath()
        {
            if (srcObj == null || dstObj == null) return;

            switch (algorithm)
            {
                case TesterGraphAlgorithm.ASTAR:
                    if (firstHeuristic) path = graph.GetPathAstar(srcObj, dstObj, graph.DistanciaEuclidiana);
                    else path = graph.GetPathAstar(srcObj, dstObj, graph.DistanciaChebyshev);
                    break;
                case TesterGraphAlgorithm.BFS:
                    path = graph.GetPathBFS(srcObj, dstObj);
                    break;
                case TesterGraphAlgorithm.DFS:
                    path = graph.GetPathDFS(srcObj, dstObj);
                    break;
            }
            if (smoothPath)
                path = graph.Smooth(path);
        }

        public string ChangeHeuristic()
        {
            // Está preparado para tener 2 heurísticas diferentes
            firstHeuristic = !firstHeuristic;
            if (ariadna) CalculatePath();
            return firstHeuristic ? "Euclediana" : "Chebyshev";
        }

        public virtual void ResetPath()
        {
            path = null;
        }

        void OnGUI()
        {
            if (!ariadna) return;

            if (graph != null && path != null)
            {
                int mapSize = graph.GetSize();
                float frameMs = Application.targetFrameRate > 0 ? (1000f / Application.targetFrameRate) : 16.666f;
                float framePercentage = (graph.duracionAstarMS / frameMs) * 100f;
                
                string metrics = $"[ MÉTRICAS DEL A* ]\n" +
                                 $"- Tamaño del entorno: {mapSize} baldosas\n" +
                                 $"- Nodos iterados (cerrada): {graph.nodosIterados}\n" +
                                 $"- Nodos visitados (abierta): {graph.nodosVisitados}\n" +
                                 $"- Longitud restante: {path.Count} baldosas/wayp.\n" +
                                 $"- Coste total calculado: {graph.costeCamino:F1}\n" +
                                 $"- Tiempo A*: {graph.duracionAstarMS:F4} ms\n" +
                                 $"- % de un fotograma consumido: {framePercentage:F2}%";
                                 
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 14;
                style.normal.textColor = Color.white;
                style.fontStyle = FontStyle.Bold;
                
                GUI.color = new Color(0, 0, 0, 0.8f);
                GUI.Box(new Rect(10, 180, 360, 160), "");
                GUI.color = Color.white;
                GUI.Label(new Rect(20, 190, 340, 150), metrics, style);
            }
        }
    }
}
