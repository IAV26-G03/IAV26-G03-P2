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
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para modelar el comportamiento de WANDER a otro agente
    /// </summary>
    public class MinoRotacion : ComportamientoAgente
    {
        [SerializeField]
        float maxTime = 2.0f;

        [SerializeField]
        float minTime = 1.0f;

        float t = 3.0f;
        float actualT = 2.0f;

        Direccion lastDir = new Direccion();

        public override Direccion GetDireccion(){
            if (t >= actualT)
            {
                Direccion direccion = new Direccion();

                Vector2 dir = Random.insideUnitCircle.normalized;

                direccion.lineal = new Vector3(0, 0, 0);

                direccion.angular = Random.Range(-1f, 1f) * agente.aceleracionMax;

                lastDir = direccion;

                actualT = Random.Range(minTime, maxTime);

                t = 0.0f;
                return lastDir;
            }
            else{
                t += Time.deltaTime;
                return new Direccion();
            }

            
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer != 7)
            {
                t = 0;
                lastDir.lineal = transform.position - collision.transform.position;
                lastDir.lineal.Normalize();
                lastDir.lineal *= agente.aceleracionMax;
            }
        }
    }
}
