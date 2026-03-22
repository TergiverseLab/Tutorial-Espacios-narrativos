# Tutorial Espacios Narrativos

Proyecto Unity para el taller de **narración espacial y poesía espacial** — BAU 2026.

## Requisitos

- **Unity 2022.3.62f3 LTS** (exactamente esta versión)
- Ratón de 3 botones
- Windows o Mac

## Cómo abrirlo

1. Descarga el proyecto: **Code → Download ZIP** (botón verde arriba)
2. Descomprime el ZIP en una ruta SIMPLE (sin tildes, eñes ni espacios largos):
   - `C:\Unity\TallerEspacios\` (bien)
   - `C:\Users\Mi Nombre\Escritorio\Taller Sesión (ñ)\` (mal)
3. Abre **Unity Hub**
4. Click en **Open → Add project from disk**
5. Selecciona la carpeta descomprimida (la que contiene `Assets`, `Packages`, `ProjectSettings`)
6. Unity Hub te pedirá la versión — selecciona **2022.3.62f3**
7. La primera vez que lo abra tardará **5-15 minutos** (Unity regenera archivos internos). Es normal. No cierres Unity aunque parezca que no hace nada.
8. Una vez abierto, ve a **Assets/Scenes/** y abre la escena **Level** (doble click)

## Contenido del proyecto

- **FirstPersonKit/** — Controlador en primera persona con scripts modulares (movimiento, interacciones, zoom, screenshot, quit)
- **LevelKit/** — Piezas modulares para construir niveles (blockout/prototipado)
- **Materials/** — 80+ materiales (PBR, cristales, toon, gradientes, glass, emissive...)
- **Shaders/** — 42 shaders (Standard, triplanar, toon, dissolve, crystal, blur, gradient, doubleSided...)
- **Models_UnityBasics/** — Modelos de ejemplo (david, diamond, star, gear...)
- **Textures/** — Texturas PBR, normal maps, terrain brushes, skyboxes
- **CameraEffects/** — Efectos de cámara (pixelate, halftone, anaglyph, godray...)
- **Resources/X-PostProcessing/** — 70+ efectos de post-procesado artísticos
- **PBRCreator/** — Script para crear materiales PBR con un click derecho
- **Particles/** — Sistemas de partículas prefabricados
- **Sounds/** — Archivos de audio de ejemplo
- **Museum/** — Kit de piezas modulares de museo/galería
- **Scenes/** — Escenas de demostración (Level, LightAndMaterials, SoundPlace, Museum, Desert...)

## Escenas de demostración

- **Level** — Escena principal con kit modular
- **LightAndMaterials** — Galería de materiales y shaders ("Chicken Museum")
- **SoundPlace** — Demostración de audio espacial
- **Museum** — Espacio de museo/galería
- **Desert** — Escena de desierto

## Lo primero al abrir Unity

1. **Edit → Preferences → Colors → Playmode Tint → ROJO**
   Esto hace que la interfaz se ponga roja cuando estás en Play Mode, para que no edites por accidente (los cambios en Play Mode se pierden).
2. Abre la escena **Level** desde Assets/Scenes/

## Créditos

- Proyecto base: [ExpressiveEnvironment](https://github.com/molleindustria/ExpressiveEnvironment) de Paolo Pedercini (molleindustria)
- Modelos y materiales: [UnityBasics](https://github.com/molleindustria/UnityBasics) de Paolo Pedercini
- Audio espacial: [SoundPlace](https://github.com/molleindustria/SoundPlace) de Paolo Pedercini
- PBR Creator: [PBRCreator](https://github.com/molleindustria/PBRCreator) de Paolo Pedercini
- X-PostProcessing: [XPostProcessing-URP](https://github.com/QianMo/X-PostProcessing-Library)
- Terrain Brushes: Unity Asset Store
