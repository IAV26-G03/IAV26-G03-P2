/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Inform�tica de la Universidad Complutense de Madrid (Espa�a).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Movimiento
{

    using UnityEngine;

    /// <summary>
    /// Clara para el comportamiento de agente que consiste en ser el jugador
    /// </summary>
    public class ControlJugador: ComportamientoAgente
    {
        /// <summary>
        /// Obtiene la direcci�n
        /// </summary>
        /// <returns></returns>
        /// 

        //float tiempoGiroSuave = 0.1f;
        //float velocidadGiroSuave;

        [SerializeField]
        private int maxAcel;
        [SerializeField]
        private int minAcel;

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();
            
           Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                Vector3 destino = hit.point;
                destino.y = transform.position.y;

                float distancia = Vector3.Distance(transform.position, destino);
                float umbralDistancia = 1f;

            
                if (distancia > umbralDistancia)
                {
                 
                    Vector3 vectorHaciaRaton = destino - transform.position;
                    vectorHaciaRaton.Normalize();

                    direccion.lineal.x = vectorHaciaRaton.x;
                    direccion.lineal.z = vectorHaciaRaton.z;
                }
                else
                {
                    direccion.lineal = Vector3.zero;
                }

            }
            direccion.lineal.Normalize();
            direccion.lineal *= agente.aceleracionMax;


            // Podr�amos meter una rotaci�n autom�tica en la direcci�n del movimiento, si quisi�ramos

            return direccion;
        }

        public void Correr()
        {
            agente.aceleracionMax = maxAcel;
        }

        public void Andar()
        {
            agente.aceleracionMax = minAcel;
        }
    }
}