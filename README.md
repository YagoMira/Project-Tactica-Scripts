# Project-Tactica-Scripts

![alt text](https://img.itch.zone/aW1nLzIxNjYwNDI5LnBuZw==/original/rshr4C.png)

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

<br/>

## Game Description
One way or another, you’ve ended up in the intranet—a completely unknown world that, by a twist of fate, is in desperate need of your help. From now on… YOU will take on the responsibility of leading an army to free this new world from the evil that threatens it and, in doing so, find your way back to what you’ve always considered your “home.”


<br/><br/>
## TUTORIAL
![me](https://github.com/YagoMira/Project-Tactica-Scripts/blob/main/Gifs_Videos/AlmahXTacticaTutorial-ezgif.com-resize.gif)
<br/><br/>
<br/><br/>


# SYSTEMS - MECHANICS & CORE GAMEPLAY
## 1.- Core System
+ Individual unit selection  
+ Group unit selection (Click and drag)  
+ Unit selection UI  
+ Camera movement  
+ Camera rotation around scene point (Calculates the pivot point)  
+ Camera zoom  
+ Camera repositioning at different heights (adjusts from low > high | high > low) to always maintain terrain distance limits  
+ Unit selection limits: checks for out-of-bounds terrain, high/low terrain, non-navigable areas by NavMeshAgents, etc.  
+ Camera height levels: Level 3 (Displays squad sprites), Level 2 (Displays 3D models of units)  
  
## 2.- Combat System
+ Move individual units to a point ("Move" action)  
+ Attack enemies with individual units ("Attack" action)  
+ Action UI  
+ Bullet collisions  
+ Units react when being attacked  
+ Camera Level 3: selected allied troops target the closest enemy when executing ("Attack") on an enemy squad  
+ Camera Level 2: selected allied troops target the nearest enemy (NOT PREVIOUSLY TARGETED by another ally in the same squad) when executing ("Attack") on an enemy squad  
+ Enemy AI integrated using A.D.A.P.T. AI tool (https://github.com/YagoMira/A.D.A.P.T.-AI-Tool o https://assetstore.unity.com/packages/tools/behavior-ai/a-d-a-p-t-ai-241029) <br/> *This tool had some modifications on the game development*
+ Enemy patrol AI  
+ Enemy attack AI 
  

## 3.- Stats System (Per Unit Type)
+ Stats creation for different units  
+ Apply stats to units (based on unit type: health, damage, speed...)  
+ Ability to apply effects via Tacticas
  
## 4.- Resource System
+ Creation of resource type "Credits"  
+ Credit increase/decrease depending on function (Recruitment, healing, factory conquest, ...)  
  
## 5.- Squad System
+ Create squads  
+ Remove squads  
+ Reorder squads  
+ Squad captain UI: allows assigning squad sprite during squad creation (as well as its formation)  
+ If an ally in the squad is attacked, the squad reacts and targets the attacker as an enemy  
+ Squad formations (**DETAILED EXPLANATION**):
  - Formación de 5 Unidades (Flecha)
    ![me](https://github.com/YagoMira/Project-Tactica-Scripts/blob/main/Gifs_Videos/Squad_5.gif)
  - Formación de 4 Unidades (Rombo)
    ![me](https://github.com/YagoMira/Project-Tactica-Scripts/blob/main/Gifs_Videos/Squad_4.gif)
  - Formación de 3 Unidades (Flecha Pequeña)
    ![me](https://github.com/YagoMira/Project-Tactica-Scripts/blob/main/Gifs_Videos/Squad_3.gif)
  - Formación de 2 Unidades (Línea)
    ![me](https://github.com/YagoMira/Project-Tactica-Scripts/blob/main/Gifs_Videos/Squad_2.gif)
    
  *Individual units don’t require a formation (assigned a “null” element)*
  
  *These formations are used by soldiers when moving as squads; you must select the entire squad to move them this way.*
  
  *If the current captain of a squad dies, the formation is reassigned to the new captain: e.g., 5-unit “Arrow” becomes 4-unit “Diamond”.*
  
  **Formations adjust if they collide with external elements, aligning colliders into a straight line with the captain.**  

## 6.- Generals/Captains System *(In Development)*
+ Currently, the squad captain is selected as the first element of the squad’s array  
+ On squad reordering: if a captain dies, a new one is assigned (along with corresponding UI and formation)
  
## 7.- Veteran System *(In Development)*

## 8.- Almah’s System *(In Development)*
+ Currently, Almahs act similarly to soldiers but with distinct traits: health, damage, speed, etc.  
+ Added conditions for the “Liberate” system 
  
## 9.- Tacticas System
+ Tacticas UI  
+ Use of tacticas on units (apply effects to allies / ability system)  
+ Tactica handling system: allows the creation and use of any type of future tacticas via scripts  
  
## 10.- Time System (Time Movement & Distance Calculation) *(In Development)*
+ Unit distance calculation  
+ UI showing estimated arrival time *(Hidden in gameplay)*  
+ Time control system: speed, pause, resume  
+ Time control UI 
  
## 11.- Base Management System *(In Development)*
+ Medical bases: Healing  
+ Medical base UI  
+ Military bases: Recruitment  
+ Military base UI  
+ Enemy base conquest  
+ Allied base conquest  
+ Conquest effects: healing, credit bonus  
+ Factories (Buildings that increase credits)  
+ Generic building UI: factories, medical bases, etc.  
  
## 12.- Dialogue System
+ Visual Novel-style dialogue  
+ Character animations  
+ Character names and dialogue integration  
+ Player name integration *(taken from game start UI)*  
+ Scene transitions / Cinematic management  
+ Integration of videos/cinematics into ongoing dialogues  
+ Dialogues handled via CSV parsing for easy multilingual support  
+ Dialogue control: speed and character-by-character reading  
 
## 13.- Social System *(In Development)*
+ Count of generals/Almahs defeated *(increases breathing frequency in final cinematics - code only)*  
  
## 14.- Liberate System
+ Cinematic handling  
+ Scene transition triggered by enemy defeat conditions

## 15.- First-Person System
 - First-person controller  
 - First-person object interaction  
 - First-person event handling 
 
 <br/><br/>
## There may be mechanics not described in detail due to their complexity, but the game also includes:
 - Victory / defeat conditions  
 - Options system (Exit game)  
 - Options UI  
 - Options handling: sound, language, etc.  
 - **Multilingual integration (ENG & ESP)**  
 - **Easy integration of new languages via CSV parsing**  
 - **Easy integration of recruitable soldier names via CSV parsing**  
 - Main Menu / Start UI  
 - Buttons, animations, and interactions in the main menu  
 - *Several voice lines have also been created for soldiers, such as: "Roger That, Moving On, Coming, Received Commander"*  
 - Etc.  


*ATTENTION: These are annotations of some of the game systems. To fully understand their implementation, please view the gameplay video or check the code in the scripts to see hidden functionalities. Some systems are implemented but not used; explanations here are not fully comprehensive or detailed (some mechanics may be missing). Thanks for your understanding.*


<br/>

# This game has been developed under the name of **RedWolf Games**
![alt text](https://img.itch.zone/aW1nLzIxNzM2NjgwLmpwZw==/original/UaDmEu.jpg)

# LICENSE
With the purpouse to protect the code if this repository:

Shield:  [![CC BY-NC-ND 4.0][cc-by-nc-nd-shield]][cc-by-nc-nd]

This work is licensed under a
[Creative Commons Attribution-NonCommercial-NoDerivs 4.0 International License][cc-by-nc-nd].

[![CC BY-NC-ND 4.0][cc-by-nc-nd-image]][cc-by-nc-nd]

[cc-by-nc-nd]: http://creativecommons.org/licenses/by-nc-nd/4.0/
[cc-by-nc-nd-image]: https://licensebuttons.net/l/by-nc-nd/4.0/88x31.png
[cc-by-nc-nd-shield]: https://img.shields.io/badge/License-CC%20BY--NC--ND%204.0-lightgrey.svg
