# Project-Tactica-Scripts

Scripts used in the development of the game "Almah X Tactica"

**Itch.io website**: https://redwolf-games.itch.io/almah-x-tactica<br/><br/>
**Portfolio**: https://redwolf-games.itch.io<br/>
**Sketchfab**: https://sketchfab.com/ycaroh/models<br/>
**Linkedin**: https://es.linkedin.com/in/yago-mira<br/>
**Email**: yagomira@gmail.com<br/>

# GAME

## Game Videos
### Teaser
https://www.youtube.com/watch?v=-WQa7ezUvd8&t=49s
### Gameplay
https://www.youtube.com/watch?v=XOIb2vmdpOg&t=8s

## Game Description
One way or another, you’ve ended up in the intranet—a completely unknown world that, by a twist of fate, is in desperate need of your help. From now on… YOU will take on the responsibility of leading an army to free this new world from the evil that threatens it and, in doing so, find your way back to what you’ve always considered your “home.”

## TUTORIAL
![me](https://github.com/YagoMira/Project-Tactica-Scripts/blob/main/Gifs_Videos/AlmahXTacticaTutorial-ezgif.com-resize.gif)

# SYSTEMS - MECHANICS & CORE GAMEPLAY
## 1.- Sistema Core
+ Selección individual de unidades
+ Selección grupal de unidades (Click y desplazar)
+ UI de selección de unidades
+ Movimiento de Cámara
+ Rotación de cámara sobre punto de escenario (Cálculo de punto sobre el que rotar)
+ Zoom de cámara
+ Reposición camera en alturas (Recolocación de altura baja > alta | alta > baja) para mantener siempre los límite de distancia al terreno
+ Límites en selección de unidades: comprobaciones fuera de terreno, terrenos altos/bajos, territorios no navegables por NavMeshAgents, etc
+ Niveles de altura de cámara: Nivel 3 (Se muestran sprites de los escuadrones), Nivel 2 (Se muestran modelos 3D de unidades)
## 2.- Sistema de Combate
+ Movimiento de unidades individuales a punto (Acción "Mover")
+ Ataque de unidades individuales a enemigo (Acción "Atacar")
+ UI de Acciones
+ Colisiones de balas
+ Unidades reacionan si son atacadas
+ Nivel 3 de cámara: Tropas aliadas seleccionadas seleccionan el enemigo más cercano en caso de realizar (Acción "Atacar") sobre un escuadrón enemigo
+ Nivel 2 de cámara: Tropas aliadas seleccionadas seleccionan el enemigo más cercano (NO SELECCIONADO PREVIAMENTE POR UNA UNIDAD ALIADA DEL MISMO ESCUADRÓN) en caso de realizar (Acción "Atacar") sobre un escuadrón enemigo
## 3.- Sistema de Stats (Dependiendo de Unidad)
+ Creación de Stats para diferentes unidades
+ Aplicación de stats sobre unidades (diferenciación dependiendo de tipo de unidad y cálculos de vida, daño, velocidad, ...)
+ Posibilidad de aplicar efectos mediante Tacticas
## 4.- Sistema de Recursos
+ Creación de recurso "Créditos"
+ Aumento / Disminución de créditos según función (Reclutamiento, cura, conquista de factorías, ...)
## 5.- Sistema de Escuadrones
+ Creación de escuadrones
+ Eliminación de escuadrones
+ Reordenamiento de escuadrones
+ UI según capitán de escuadrón: Permite añadir sprite de escuadrón al momento de creación del escuadrón (así como su formación pertinente)
+ En caso de ataque a un aliado del escuadrón, el escuadrón reacciona y selecciona al atacante como enemigo
+ Formación de escuadrones (**EXPLICACIÓN DETALLADA:**)
  - Formación de 5 Unidades (Flecha)
    ![me](https://github.com/YagoMira/Project-Tactica-Scripts/blob/main/Gifs_Videos/Squad_5.gif)
  - Formación de 4 Unidades (Rombo)
    ![me](https://github.com/YagoMira/Project-Tactica-Scripts/blob/main/Gifs_Videos/Squad_4.gif)
  - Formación de 3 Unidades (Flecha Pequeña)
    ![me](https://github.com/YagoMira/Project-Tactica-Scripts/blob/main/Gifs_Videos/Squad_3.gif)
  - Formación de 2 Unidades (Línea)
    ![me](https://github.com/YagoMira/Project-Tactica-Scripts/blob/main/Gifs_Videos/Squad_2.gif)
    
  *Las unidades individuales no necesitan formación (se les asigna un elemento "null")
  
  *Estas formaciones son usadas por los soldados cuando se mueven en escuadrones, se necesita seleccionar un escuadrón entero para movilizarlo de esta manera.
  
  *En caso de que el capitán actual de un escuadrón muera, se le reasigna al nuevo capitán la formación, es decir: Si son 5 unidades con una formación "Flecha" y el capitán muere -> nueva formación = 4 unidades "Rombo"
  
  **Las formaciones se reajustan en caso de colisionar con un elemento externo, moviendo los colliders de las formaciones para formar una línea recta con el capitán**

## 6.- Sistema de Generales/Capitanes  *(On Development)*
+ Actualmente el capitán de un escuadrón se escoge según: el primero elemento del array perteneciente a ese escuadrón
+ Reordenación de escuadrones: en caso de que un capitán muera, se asigna un nuevo escuadrón (así como la UI y formaciones correspondientes)
## 7.- Sistema Veteranía *(On Development)*
## 8.- Sistema de Almah’s *(On Development)*
## 9.- Sistema de Tactica’s
## 10.- Sistema de Tiempo (Desplazamiento en tiempo y cálculo de distancias) *(On Development)*
## 11.- Sistema de Gestión de Bases *(On Development)*
## 12.- Sistema Diálogo
## 13.- Sistema Social *(On Development)*
## 14.- Sistema Liberate


*ATTENTION: This are annotations of some of the systems of the game, to understand the fully implementation please view the gameplay video. This is because some systems are implemented but not used, some explanations here are not fully extensive or detailed (some mechanics maybe are missing). Thanks for the consideration*


# LICENSE
With the purpouse to protect the code if this repository:

Shield:  [![CC BY-NC-ND 4.0][cc-by-nc-nd-shield]][cc-by-nc-nd]

This work is licensed under a
[Creative Commons Attribution-NonCommercial-NoDerivs 4.0 International License][cc-by-nc-nd].

[![CC BY-NC-ND 4.0][cc-by-nc-nd-image]][cc-by-nc-nd]

[cc-by-nc-nd]: http://creativecommons.org/licenses/by-nc-nd/4.0/
[cc-by-nc-nd-image]: https://licensebuttons.net/l/by-nc-nd/4.0/88x31.png
[cc-by-nc-nd-shield]: https://img.shields.io/badge/License-CC%20BY--NC--ND%204.0-lightgrey.svg
