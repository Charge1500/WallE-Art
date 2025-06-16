# **SUPER WALLE BROS: Wall-E en el Reino Champiñón**

## **Contexto del Proyecto 📖**

🌌 El cosmos, tan vasto y misterioso, a veces tiene un sentido del humor peculiar. Wall-E flotaba a la deriva, sumido en sus pensamientos ecológicos y soñando con su querida EVA. 💚 De repente, un destello verde esmeralda, una estela de polvo de estrellas y un inexplicablemente familiar sonido de "¡Pling\!" 🍄 lo envolvieron. Cuando el brillo se desvaneció, Wall-E no estaba en el frío vacío del espacio, sino sobre un vibrante bloque de interrogación flotante, con nubes pixeladas y un castillo reconocible a lo lejos. 🏰

Wall-E había aterrizado, sin querer, en el Reino Champiñón. Su inocente recolección de "basura" (Goombas y Koopa Troopas rotos) no tardó en llamar la atención de un furioso Mario. 😡 "¡Mamma mia\! ¡Estás pisoteando mis niveles y usando mis cosas sin licencia\!", exclamó Mario. Wall-E, sin entender, solo podía decir "¡Wall-E\!". 🤖 Sin saberlo, había activado la alarma de Nintendo por infracción de derechos de autor. 🚨 Ahora, este pacífico robot se encontraba en medio de una demanda interdimensional. ⚖️ Su única esperanza de salir de este lío y volver a casa era encontrar lo más preciado que traía consigo: su pequeña bota con la última planta viva. 🌱👢

Con abogados de setas afilando sus plumas, Wall-E debe navegar por los peligrosos niveles de Mario, evitando no solo a los enemigos sino también la factura de la demanda. 💸 Su misión: recuperar su bota antes de convertirse en una exhibición legal.

¿Podrá Wall-E recuperar su bota y escapar de las garras legales del Reino Champiñón? ¿O terminará como una exhibición en el Museo de la Historia de Nintendo por "uso no autorizado de activos"? 🏛️

El juego se divide en dos módulos principales:

1. **Editor de Pixel Art**: Un intérprete de comandos que permite a los usuarios "pintar" en un lienzo digital controlando a Wall-E con un lenguaje de programación personalizado.  
2. **Generador de Niveles de Plataformas**: Una función que transforma texturas 2D en niveles de plataformas jugables, donde Wall-E debe navegar y encontrar su bota.

## **Tecnologías Utilizadas 🛠️**

* **Unity**: Motor principal para el desarrollo del juego.  
* **C\#**: Lenguaje de programación para la lógica del juego.  
* **Git**: Para el control de versiones.

## **Editor de Pixel Art 🎨**

Este módulo permite a los usuarios interactuar con un lienzo digital utilizando un lenguaje de programación personalizado. Wall-E es el encargado de ejecutar las instrucciones, permitiendo crear arte de píxeles. El lenguaje incluye comandos para **inicializar la posición de Wall-E (Spawn)**, **cambiar el color y tamaño del pincel (Color, Size)**, y **dibujar diversas formas como líneas, círculos y rectángulos (DrawLine, DrawCircle, DrawRectangle)**, así como **rellenar áreas (Fill)**. También soporta la asignación de variables, expresiones aritméticas y booleanas, funciones para obtener información del lienzo y de Wall-E, y control de flujo mediante saltos condicionales (GoTo).

## **Mecánicas de Juego (Niveles de Plataformas) 🎮**

El objetivo principal de este módulo es que Wall-E navegue a través de niveles de plataformas para encontrar su "bota con la planta". Los niveles se generan dinámicamente a partir de texturas 2D predefinidas, donde cada color en la textura se mapea a un tipo específico de tile o prefab, construyendo el entorno del juego.

### **Controles: 🕹️**

* **ESPACIO**: Saltar  
* **FLECHA ABAJO**: Agacharse  
* **FLECHA IZQUIERDA**: Mover hacia la izquierda  
* **FLECHA DERECHA**: Mover hacia la derecha

## **Estructura del Proyecto 📂**

El proyecto sigue una arquitectura robusta de compilador/intérprete, con las siguientes áreas clave:

1. **Núcleo del Intérprete**:  
   * **Análisis Léxico** (Lexer.cs, Tokens.cs, FunctionRegistry.cs): Transforma el código fuente en tokens y define las funciones del lenguaje.  
   * **Análisis Sintáctico** (Parser.cs): Construye el Árbol de Sintaxis Abstracta (AST) y verifica la gramática.  
   * **Análisis Semántico** (SemanticAnalyzer.cs, SymbolTable.cs): Verifica la coherencia y el significado del programa.  
   * **Interpretación** (Interpreter.cs, InterpreterFunctions.cs): Ejecuta el AST, mantiene el estado de Wall-E y el lienzo, y maneja errores en tiempo de ejecución.  
2. **Gestión de UI y Archivos**:  
   * UIManager.cs: Controla la interfaz de usuario principal, gestionando el editor de código, el lienzo, y los botones de interacción (redimensionar, cargar/guardar scripts e imágenes, ejecutar).  
   * FileManager.cs: Gestiona la carga y guardado de scripts (.pw) y la exportación de imágenes JPG.  
3. **Gestión del Lienzo**:  
   * CanvasController.cs: Se encarga de la inicialización y gestión de la textura 2D que sirve como lienzo de Wall-E.  
4. **Gestión de Niveles y Temas**: Archivos como LevelThemeData.cs, JugarManager.cs, LevelManager.cs, y TextureToTileMap.cs manejan la creación y carga de niveles.

## **Requisitos para Abrir y Editar el Proyecto 💻**

Para trabajar con el código fuente del proyecto, necesitarás:

* **Unity Hub**: La herramienta de gestión de proyectos de Unity.  
* **Unity Editor**: Versión 2022.3.x LTS o superior.  
* **Visual Studio** (con la carga de trabajo "Desarrollo de juegos con Unity") o **Visual Studio Code** (con extensiones de C\# y Unity) para editar los scripts.  
* **Sistema Operativo**: Compatible con Unity (Windows, macOS, Linux).

### **Pasos para Abrir el Proyecto: ▶️**

1. **Clonar o Descargar el Repositorio**: Obtén el código fuente del proyecto.  
2. **Abrir con Unity Hub**: Inicia Unity Hub, haz clic en "Add" y selecciona la carpeta raíz del proyecto descargado.  
3. **Abrir en el Editor de Unity**: Selecciona el proyecto en Unity Hub y ábrelo con la versión de Unity Editor adecuada. Unity puede tardar un tiempo en importar los activos.

Una vez abierto, el proyecto está listo para ser ejecutado directamente desde el editor de Unity.

## **Mejoras Futuras 💡**

* Expandir el conjunto de comandos del lenguaje de Pixel Art para incluir más operaciones complejas.  
* Implementar un editor de niveles dentro del juego para facilitar la creación de nuevos desafíos.  
* Añadir más tipos de enemigos y power-ups en los niveles de plataforma.  
* Mejorar la interfaz de usuario con animaciones y transiciones más fluidas.# WallE-Art