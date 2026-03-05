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

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{

    Animator animator;
    new Rigidbody rigidbody; // Usar 'new' para ocultar el miembro heredado... se supone que es algo obsoleto pero que sigue heredando MonoBehavior de Component, o algo así

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rigidbody.velocity.magnitude >= 0)
            animator.SetInteger("speed", (int)rigidbody.velocity.magnitude);
    }
}
