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
    /// Clase para modelar el comportamiento de rotacion
    /// </summary>
    public class MinoRotacion : ComportamientoAgente
    {
        [SerializeField]
        float maxTime = 3.0f;

        [SerializeField]
        float minTime = 1.0f;

        float t = 0.0f;
        float actualT = 2.0f;

        float anguloDestino;
        float velocidadGiroSuave;

        void Start()
        {
            anguloDestino = transform.eulerAngles.y;
        }

        public override Direccion GetDireccion(){
            if (t >= actualT)
            {
                //Escoge un nuevo ángulo de mirada aleatorio
                anguloDestino = transform.eulerAngles.y + Random.Range(-90f, 90f);
                actualT = Random.Range(minTime, maxTime);
                t = 0.0f;
            }
            else{
                t += Time.deltaTime;
            }

            //Gira el transform localmente para forzar que el vigía mire a su alrededor de forma visual
            //sin depender de la fricción del Rigidbody ni de los AddTorque del Agente.
            float anguloSuave = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloDestino, ref velocidadGiroSuave, 0.5f);
            transform.rotation = Quaternion.Euler(0f, anguloSuave, 0f);

            //Devuelve dirección vacía para que Agente no aplique ninguna velocidad.
            return new Direccion();
        }
    }
}
