# Inteligencia Artificial para Videojuegos - Práctica 2: El Secreto del Laberinto


## Autores
- Nerea Espinosa Calatayud - nebbber(https://github.com/nebbber)
- Víctor Valenzuela Martínez - imVictorVM(https://github.com/imVictorVM)

## Vídeo con las pruebas:
Enlace al vídeo con las pruebas realizadas:

## Resumen
Se trata de un prototipo básico de un juego que simula el legendario **Laberinto del Minotauro**, donde el jugador controla a **Teseo** y el resto de entidades, los **minotauros**, actúan con inteligencia artificial.

Los **minotauros** tienen 2 tipos de comportamiento: *vigía* y *patrullero*. 
Ambos se encuentran en el laberinto y, al detectar a **Teseo**, le comienzan a perseguir.

El objetivo es manejar a **Teseo** hacia la salida evitando ser atrapado por estos **minotauros**.

Existirá la posibilidad de navegar automáticamente a la salida mediante el **Hilo de Ariadna**, el cual es un algoritmo `A*` que encuentra el mejor camino hacia la salida y guía automáticamente a **Teseo**.

## Instalación y uso
Todo el contenido del proyecto está disponible aquí en el repositorio, pues **Unity 6 (6000.0.53f1 LTS)** o posterior debería ser capaz de bajar todos los paquetes necesarios.

Al no estar publicada todavía ninguna versión ejecutable del prototipo, ni enlazado ningún video con las pruebas realizadas, es necesario abrir el proyecto en **Unity** y usarlo desde allí.

## Introducción  
Este proyecto es una práctica de la asignatura de **Inteligencia Artificial para Videojuegos** del Grado en Desarrollo de Videojuegos de la UCM, cuyo enunciado original es este: [El Secreto del Laberinto](https://narratech.com/es/inteligencia-artificial-para-videojuegos/navegacion/el-secreto-del-laberinto/).

Esta práctica consiste en:

Al querer simular la huida del laberinto del mito, necesitamos simular los comportamientos de los **minotauros** que se encuentran dentro de este laberinto. Al tener dos tipos de **minotauro** necesitamos implementar dos comportamientos diferentes: para los *vigías*, deberán rotar cada cierto tiempo en el sitio sin moverse. Para los *patrulleros*, seguirán el camino de los mapas definido en el `grid` y en las encrucijadas decidirá si cambiar la dirección o seguir hacia el frente.

Todos los **minotauros** tendrán un cono de visión, donde si detectan a **Teseo**, comenzarán a seguirle. Dejarán el seguimiento cuando **Teseo** salga del cono de visión.
Además, tendrán una esfera con un rango visible donde si **Teseo** se encuentra verá reducida su velocidad, "simulando una batalla".

Para ayudar a **Teseo** en la huida, podrá activar el **Hilo de Ariadna**, el cual hará que aparezca en pantalla una línea blanca mostrando el camino más óptimo hacia la salida. Al estar activo, **Teseo** lo seguirá automáticamente y este irá desapareciendo a medida que **Teseo** avance. Cuando el hilo está activo, el jugador no puede controlar a **Teseo**, pero al desactivarlo, el jugador recupera totalmente el control.
Este movimiento manual se realiza persiguiendo el puntero del ratón al estar a cierta distancia de **Teseo**. El jugador puede correr, aumentando la velocidad normal de movimiento de **Teseo**.

## Punto de partida
Se parte de un proyecto base proporcionado por el profesor y disponible en este repositorio: [Minotaur - Base](https://github.com/narratech/minotaur-base)

Consiste en un entorno virtual 3D que representa el legendario **Laberinto del Minotauro**, generado a partir de una descripción en forma de fichero de texto, con un personaje controlable por el jugador que es **Teseo** y uno o varios **minotauros** que actúan como enemigos.

El proyecto define una arquitectura basada en **comportamientos de dirección** (`Steering Behaviors`).

El componente `Agente` es el principal, el cual se encarga de procesar todos los datos. Este componente se encuentra en cada entidad que quiera tener un comportamiento de movimiento.
Puede combinar los distintos tipos de comportamientos según peso o prioridad.

El componente `ComportamientoAgente` es la clase base de la que heredan todos los comportamientos. Tiene un peso, una prioridad y un objetivo.
Además, contiene un método que obtiene la información sobre cómo mover al `Agente` mediante un `ComportamientoDireccion`, que es una clase que contiene la aceleración lineal y angular a aplicar.

Además, existe un `GestorJuego` (`GameManager`) que maneja los diferentes estados del juego, y tiene el control técnico.

## Planteamiento del problema

Los objetivos de la práctica son los siguientes:

A. Hay un mundo virtual (el **Laberinto del Minotauro**) con habitaciones y pasillos de tamaño y complejidad configurable, con un esquema de división de grafo de baldosas que incluye una baldosa de entrada, donde se ubica inicialmente el avatar (**Teseo**), y una baldosa de salida. Hay varios caminos alternativos para llegar a la salida, alguno ancho y alguno estrecho (pasillos de una única baldosa de anchura, donde es muy probable toparnos con el enemigo). El avatar es controlado por el jugador mediante el ratón. Si el puntero está más allá de cierta distancia del avatar, este camina en línea recta hacia la posición del puntero. Mientras se mantiene pulsado el **clic izquierdo**, el avatar corre más rápido.

B. En mitad del laberinto están los enemigos (uno o varios **minotauros**, de dos tipos y número también configurable). Unos son *vigías* que permanecen quietos en su sitio, rotando aleatoriamente. Otros son *patrulleros* que siempre están caminando en línea recta, pudiendo cambiar de dirección aleatoriamente al llegar a las encrucijadas del laberinto. Los dos tipos de **minotauro** tienen un cono de visión y si detectan al avatar del jugador se ponen a perseguirlo. También tienen un área de influencia, que se muestra visualmente, consistente en las baldosas vecinas (vecindad Manhattan de radio 3 sin que haya obstáculos por medio) que ralentizan a **Teseo** si intenta atravesarlas para «escenificar» que héroe y **minotauro** están en pleno combate.

C. El camino más corto a la baldosa de salida (¡sin poder hacer navegación en diagonal, ojo!) calculado mediante `A*` (**Hilo de Ariadna**) se representa pintado con una línea blanca y marcando cada baldosa que lo compone con una esfera blanca (ovillo) en el centro de la baldosa. Un nuevo hilo se crea cada vez que intentamos salir del laberinto (con el **clic derecho** del ratón se crea o se destruye, como un interruptor). Se tienen en cuenta todos los costes, incluidos los de las baldosas con **minotauros** (coste infinito) y las baldosas de sus zonas de influencia (coste 5), siendo 1 el coste de una baldosa normal. La conexión que permite llegar a una baldosa es la que tiene el coste de esa baldosa de destino. Desde una opción de la interfaz de usuario es posible elegir la heurística a utilizar en el algoritmo `A*`, habiendo al menos 2 diferentes.

D. También se puede elegir si queremos suavizar o no el camino generado por el algoritmo anterior (activando o desactivando la funcionalidad desde otra opción de la interfaz de usuario). Esto reduce las baldosas que forman parte del camino original a la salida, y por tanto también reduce la cantidad de esferas blancas mostradas sobre ellas.

E. El avatar navega y se mueve automáticamente (con **comportamientos de dirección** creados en el proyecto) hacia la baldosa de salida en cuanto creamos un nuevo **Hilo de Ariadna**. Este movimiento va haciendo desaparecer la parte del hilo ya recorrida, para que sólo quede la parte del camino que falta por recorrer. Si destruimos el hilo, el avatar vuelve a su estado habitual, volviendo a ser controlado por el jugador. La implementación es eficiente y consigue maximizar las métricas (ej. escapar de un laberinto que tiene un gran número de casillas por el camino más corto, tardando el menor tiempo posible en calcular la ruta y explorando el menor número de nodos posibles durante esos cálculos).

Para ello, debemos ajustar diferentes partes del prototipo:
- Cambios en el movimiento para que siga el ratón mediante `raycast`, y posibilidad de aumentar la velocidad con el **clic izquierdo** del ratón, para satisfacer el apartado A.
- Deben existir 2 tipos de **minotauro**, y deben ser asignados aleatoriamente. Los **minotauros** *vigías* deberán tener un comportamiento de rotación que les permita girar sin moverse de su posición. Para los *patrulleros*, debemos implementar un comportamiento de merodeo ordenado, donde cambien de dirección en el momento correcto según el `grid`. Ambos **minotauros** deberán tener un comportamiento de seguimiento que simplemente seguirá a **Teseo** cuando sea activado. Esto se controlará con un `MinoManager`, el cual detectará en un cono de visión (con un `raycast`) a **Teseo** y cambiará los comportamientos. Esto para satisfacer el apartado B.
- Mediante el algoritmo de `A*`, debemos implementar una manera de detectar el camino más corto a la baldosa de salida, que devuelva las casillas por las que **Teseo** debe ir para llegar al destino. Este algoritmo será usado al pulsar el **clic derecho** del ratón, y mientras esté activo, se mostrará ese camino con una línea blanca en el juego. Esta funcionalidad debe desactivarse al volver a pulsar el **clic derecho** del ratón, lo que hará que deje de calcularse el camino y desaparecerá la línea blanca que guía. El algoritmo debe tener en cuenta los costes, tal como se implementa `A*` normalmente.
El algoritmo tendrá como mínimo 2 opciones de heurística al aplicarse, pudiendo cambiarse en la interfaz con la tecla **C**. Esto será para satisfacer el apartado C.

- Existirá una opción en la interfaz para activar el suavizado al calcular el algoritmo `A*`, que se activará con la tecla **S**. Este suavizado asegurará un camino con menos baldosas al usar el hilo, para satisfacer el apartado D.

- Al activar el hilo, el jugador pierde el control sobre **Teseo**, y se moverá automáticamente siguiendo las casillas designadas por el algoritmo. Este movimiento automático hará que **Teseo** recorra el camino designado por el algoritmo sin necesidad de intervenir. Además, la línea que muestra el camino debe ir desapareciendo según se va siguiendo. Esto deberá suceder cuando se ha pulsado el **clic derecho** del ratón y se ha mostrado una salida al laberinto, y dejará de funcionar al volver a pulsar el **clic derecho**, devolviendo el control al jugador. Con esto safisfacemos el apartado E.

El prototipo permitirá:
- Mover a **Teseo** con el puntero del ratón, de manera que seguirá la posición del puntero si se encuentra más allá de cierta distancia. Con el **clic izquierdo** del ratón se moverá más rápido (es decir, correrá), y con el **clic derecho** del ratón activará el **Hilo de Ariadna**. 

- En cuanto a interfaz, habrá información del juego en la parte superior, permitiendo cambiar la heurística y la opción de suavizado.

- Existirán **minotauros** con las características antes mencionadas a lo largo del laberinto.

## Diseño de la solución

Para la **UI** (Interfaz de Usuario), añadiremos el texto del botón de suavizado, indicando claramente la tecla con la que se activa. Además, se cambiarán los nombres de las heurísticas utilizadas.

Sobre el movimiento del jugador, se utilizará un `raycast` para encontrar la posición deseada, y se añadirán un par de métodos para cambiar la velocidad al pulsar el **clic izquierdo**.
Para el movimiento automático, utilizaremos los métodos de Ariadna en el script `Teseo.cs` para manejar el movimiento automático.

Ahora los comportamientos:

- Para la rotación de los *vigías*, cambiaremos la rotación del **minotauro** directamente en su **comportamiento de dirección**, pero sin darle una dirección para que permanezca en el sitio.
- Para el merodeo de los *patrulleros*, implementaremos un movimiento que vaya comprobando el `grid` sobre el que se mueve y tomando decisiones cada vez que cambia de casilla.
- Para el seguimiento de todos los **minotauros**, implementaremos un seguimiento básico sin predicción.
- Para el **Hilo de Ariadna**, implementaremos el algoritmo de `A*` en la jerarquía de grafos (clase `Graph` o `GraphGrid`), concretamente en el método virtual `GetPathAstar()`. Según la documentación del proyecto base, este está diseñado para permitir una implementación simplificada del algoritmo `A*`. Esta versión simplificada consta de una estructura del registro del nodo muy básica (identificador del nodo y coste *f*) ligada a los propios *GameObjects* (baldosas). Como se asume tener la información completa del grafo (costes incluidos) en forma de matriz en memoria, se prescinde de emplear una lista de cierre y solo se utiliza una lista de apertura implementada mediante una estructura de cola de prioridad (`PriorityQueue`).

El pseudocódigo adaptado a esta arquitectura simplificada sería:

```text
función GetPathAstar(nodo_inicio, nodo_objetivo):
    abierta = nueva ColaDePrioridad()
    abierta.Encolar(nodo_inicio, coste_f=Heuristica(nodo_inicio, nodo_objetivo))
    
    // Como conocemos el grafo al completo, podemos almacenar 'g' y 'padre' 
    // en diccionarios, arrays o matrices de manera directa
    coste_g[nodo_inicio] = 0
    padre[nodo_inicio] = nulo
    
    mientras abierta no este vacia:
        actual = abierta.Desencolar() // Extrae el nodo con menor coste_f
        
        si actual == nodo_objetivo:
            devolver ReconstruirCamino(padre, nodo_objetivo)
            
        para cada vecino en ObtenerVecinos(actual):
            nuevo_coste_g = coste_g[actual] + costeArista(actual, vecino)
            
            // Si el vecino no se ha evaluado aún, o si encontramos un camino más corto hacia él
            si vecino no esta en coste_g o nuevo_coste_g < coste_g[vecino]:
                coste_g[vecino] = nuevo_coste_g
                padre[vecino] = actual
                
                coste_f = nuevo_coste_g + Heuristica(vecino, nodo_objetivo)
                abierta.Encolar(vecino, coste_f) // Se procesará siempre la evaluación de menor coste_f
                
    devolver nulo // Sin camino posible
```

Tendremos una opción de calcular con suavizado que puede ser cambiada desde la interfaz, alojada en el método `Smooth()` de la clase `Graph`.

Este algoritmo acorta el camino generado por `A*` saltándose nodos cuando hay una trayectoria directa ("línea de visión").

El pseudocódigo sería:

```text
función Suavizar(camino):
    nuevo_camino = [camino[0]] // Empezamos por el inicio
    actual = camino[0]
    
    mientras actual no sea el último nodo:
        // Buscamos el nodo más lejano del camino con visión directa
        siguiente = nodo de 'camino' más lejano y visible desde 'actual'
        
        nuevo_camino.añadir(siguiente)
        actual = siguiente
        
    devolver nuevo_camino
```

## Implementación
Las tareas serán realizadas y el esfuerzo será repartido entre los autores, ya que se trabajará siempre en conjunto desde un único ordenador, evitando trabajar en solitario excepto que sea una urgencia. Esto puede no verse reflejado siempre en los *commits* del repositorio, pero se intentará utilizar la función de coautor siempre que sea posible.
De cualquier manera, ambos afirmamos en este documento que el trabajo ha sido realizado en conjunto, poniendo los dos un gran esfuerzo en realizar la práctica.

El flujo de trabajo ha sido el siguiente:
El día 5 de marzo se comenzó la práctica y realizamos el apartado **A**, teniendo ya implementado el movimiento de **Teseo** con el puntero del ratón y correr con el **clic izquierdo**. 
Empezamos el apartado **B**, sin mucho éxito.
La semana del 9 de marzo, conseguimos hacer las rotaciones de los *vigías* y el merodeo de los *patrulleros*, y realizamos cambios significativos en la documentación para preparar la entrega.
...

## Ampliaciones

De momento, no se han realizado ampliaciones.


## Pruebas y métricas
Todavía no se han realizado pruebas.

## Conclusiones
...

## Licencia
Nerea y Víctor, autores de la documentación, código y recursos de este trabajo, concedemos permiso permanente a los profesores de la Facultad de Informática de la Universidad Complutense de Madrid para utilizar nuestro material, con sus comentarios y evaluaciones, con fines educativos o de investigación; ya sea para obtener datos agregados de forma anónima como para utilizarlo total o parcialmente reconociendo expresamente nuestra autoría.

Una vez superada con éxito la asignatura se prevé publicar todo en abierto (la documentación con licencia **Creative Commons Attribution 4.0 International (CC BY 4.0)** y el código con licencia **GNU Lesser General Public License 3.0**).

## Referencias
* Lousberg, K. (s. f.). *KayKit - Animations* [Animaciones 3D]. itch.io. Recuperado de https://kaylousberg.itch.io/kaykit-animations
* Lousberg, K. (s. f.). *KayKit - Dungeon Pack* [Modelos 3D]. itch.io. Recuperado de https://kaylousberg.itch.io/kaykit-dungeon
* Lousberg, K. (s. f.). *KayKit - Medieval Builder Pack* [Modelos 3D]. itch.io. Recuperado de https://kaylousberg.itch.io/kaykit-medieval-builder-pack
* Microsoft. (s. f.). *PriorityQueue<TElement,TPriority> Class*. Microsoft Learn. Recuperado de https://learn.microsoft.com/es-es/dotnet/api/system.collections.generic.priorityqueue-2
* Millington, I., & Funge, J. (2009). *Artificial Intelligence for Games* (2.ª ed.). Morgan Kaufmann. Recuperado de http://www.matt-versaggi.com/mit_open_courseware/GameAI/ArtificialIntelligenceforGamesSecondEdition.pdf
* Patel, A. (s. f.). *Amit’s A* Pages*. Red Blob Games. Recuperado de http://theory.stanford.edu/~amitp/GameProgramming/
* Peinado, F. (s. f.). *El Secreto del Laberinto* [Práctica de asignatura]. Narratech. Recuperado de https://narratech.com/es/inteligencia-artificial-para-videojuegos/navegacion/el-secreto-del-laberinto/
* Peinado, F. (s. f.). *Minotaur - Base* [Código fuente]. GitHub. Recuperado de https://github.com/narratech/minotaur-base
* Peinado, F. (s. f.). *Navegación* [Diapositivas de la asignatura IAV 2025-2026]. Narratech. Recuperado de https://narratech.com/es/inteligencia-artificial-para-videojuegos/navegacion/
* Soares de Lima, E. (2018). *Game AI - Steering Behaviours* [Diapositivas en formato PDF]. Edirlei.com. Recuperado de https://edirlei.com/aulas/game-ai/GAME_AI_Lecture_07_Steering_Behaviours_2018.pdf
* Unity Technologies. (s. f.). *Physics.Raycast*. Unity Documentation. Recuperado de https://docs.unity3d.com/ScriptReference/Physics.Raycast.html