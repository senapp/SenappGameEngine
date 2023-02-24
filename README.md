# Senapp Game Engine

Senapp Game Engine is a simple game engine written in C# with OpenGL through OpenTK. 
<br />
<br />
The main idea behind the engine was to explore game engine programming and make something that would be easy to use when creating a game and give quite good performance, basically modeled like a simple Unity3D but in your code editor. 

### Tutorials used:
- [GalacticGlum - Game Engine Tutorials](https://www.youtube.com/watch?v=ztyoJfyOz5k)
- [ThinMatrix - OpenGL 3D Game Tutorials](https://www.youtube.com/watch?v=VS8wlS9hF8E&list=PLRIWtICgwaX0u7Rf9zkZhLoLuZVfUksDP)
- [Cherno - Game Engine](https://www.youtube.com/watch?v=JxIZbV_XjAs&list=PLlrATfBNZ98dC-V-N3m0Go4deliWHPFwT)

### Features:
- Game object system
- Component system
- Scenes & scene management
- Game object relation hierarchy
- Transformation with translation, rotation & scaling with relation hierarchy
- [UI] Font class with loading
- [UI] Texts
- [UI] Sprites
- [UI] Buttons
- [UI] TextButtons
- [UI] Input fields
- RaycastTarget/UI, enter/exit/click/unfocus with pixel precision for game objects
- Keyboard input
- Mouse input
- Controller input
- Model & texture loader
- OBJ & DAE loader
- Box collisions
- Component renderers & shaders
- Skybox rendering & shader
- FrameBuffer & FrameBuffer rendering
- Deferred rendering
- Frame rate info
- Wire frame rendering
- Settings class & file
- GraphicSettings, extension of Settings
- Resource loading
- Extension methods for vectors, math, & randomizing
- Example programs

### Simple guide:
The engine works on a component architecture, where you basically just create a `GameObject` and assign the components it will work with.
Create your own main C# file that inherits from `Game` and add it to `Program.cs`, look at the files in `/Programs` for setup.
<br />
<br />
There are 3 rendered components in the engine, `Entity`, `Text`, and `Sprite`. These components decide how the `GameObject` works and which base components can be used.
There are two types of GameObjects and Components. `GameObject` and `GameObjectUI`, `Component` and `ComponentUI`. Only `Component` can be added to `GameObject` and only `ComponentUI` can be added to `GameObjectUI`.
<br />
<br />
`Game` has some default objects that can be used, `MainCamera`, `SunLight`, and `MainScene`. As well as all manager classes such as `SceneManager`.
`MainScene` is automatically added but new scenes must be added to the `SceneManager` to be rendered. 
GameObjects need a parent of some sort, a `Scene` or a parent of the same `GameObject` type, `GameObject` or `GameObjectUI`, whose final parent is a scene.
<br />
<br />
Example `Entity` setup code:
<br />
`var sphere = new GameObject().WithParent(MainScene).AddComponent(new Entity(Geometries.Sphere));`
<br />
<br />
Example `Text` setup code:
<br />
`GameFont font = new();`
<br />
`font.LoadFont("opensans");`
<br />
`var text = new GameObjectUI().WithParent(MainScene).AddComponent(new Text("Example Text", font, 15));`
<br />
<br />
Look at implementations in `/Programs` for more detailed setups.

#### INFORMATION REGARDING MODELS AND TEXTURES

Some models and textures in the engine are owned by 3rd parties, the owners of these models and textures do not endorse or sponsor this project. These assets are just used for testing and prototyping.

#### INFORMATION REGARDING MOBAGAME AND ASSETS FROM RIOT GAMES

Assets and gameplay features in the MobaGame program are owned by Riot Games and their game League of Legends. 
MobaGame is just a recreation with no commercial gain of the previous game mode Twisted Treelines from League of Legends.
MobaGame was created under Riot Games' "Legal Jibber Jabber" policy using assets owned by Riot Games. Riot Games does not endorse or sponsor this project.
