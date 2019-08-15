uFleX v0.55
NVidia FleX for Unity3D
support: korzenio [at] gmail.com
store: http://u3d.as/r50
forum: http://forum.unity3d.com/threads/uflex-nvidia-flex-for-unity3d-unified-visual-fx-released.397836/


uFLeX enables use of NVidia Flex in Unity3D.
FleX is the new GPU accelerated particle simulation library aimed at physically-based visual effects. 
The core idea of FleX is that every object (such as rigid body, soft body or fluids) is represented as a system of particles connected by constraints. 
Such unified representation allows efficient modelling of many different materials and natural interaction between elements of different types, 
for example, two-way coupling between rigid bodies and fluids, or fluids and soft bodies.
See https://developer.nvidia.com/flex for more.

uFleX is under development. Any bug reports and feature requests are more than welcome.

uFleX currently provides:
- Access to low-level Flex native library
- Support for rigid and soft bodies, cloths, fluids, inflatables, ropes, and granular materials
- Deformable mesh rendering
- Screen space fluid rendering via external package (free)
- GUI for straightforward creation of Flex assets
- Example scripted framework for quick integration with your games
- Cloth tearing
- Bunch of example scenes


-----Requirements----
- NVidia GPU with at least CUDA compute capability 3.0
- Latest drivers
- Windows x64 bit (x86, Android and Linux support planned)


-----Fluid Rendering----
NVidia Flex itself is a pure physics simulation library. However, uFlex already provides rigid body, soft body, cloths and ropes mesh rendering functionality via Unity. 

In terms of fluid rendering uFlex comes with a separate Screen Space Fluids (SSF) asset included. SSF is a set of experimental shaders for rendering the surface of particle-based fluids with real-time performance and configurable speed/quality.
Additionally, for free, uFlex customers receive a higher-quality version of SSF shaders which are part of paid SSF Pro package. 
Purchasing the full version of SSF Pro (http://u3d.as/vy7) will give you access to a complete feature set including fluid translucency, refractions, foam (more to come soon). 

To try out SSF with uFlex just unpack the file located in ScreenSpaceFluidsPro folder and see the included scenes.
However, please treat uFlex and SSF as separate products especially when rating them on the Asset Store.
Real-time fluid rendering is a very complex topic and will take much more time to develop and integrate with Unity than uFlex.


----QuickStart-----
1. Import uFlex package
2. If Standard Effects package already imported -> delete uFlex/Standard Assets folder (ImageEffectsBase required for point sprites rendering)
3  In Edit/ProjectSettings/Time decrease the FixedTimestep to 0.01 (the lower the better, but keep it adequate to your hardware capabilities)
4. In the main toolbar select Tools/uFlex
5. If missing, the Flex GameObject will be added to the scene
6. Select body type
7. If Rigid, Soft or Inflatable pick a CLOSED input mesh
8. Click Generate 

Check FlexManual.zip for a complete Flex manual.
For video tutorials subscribe to my channel http://www.youtube.com/playlist?list=PLkKm77tYcGu8Gk1w3033XDuDxoYRpQhHo


----ChangeLog-----
v0.55
-Improved performance
-Fixed SSF rendering in Unity 5.5
-Signed Distance Fields (SDF) support 

v0.5
-Mesh tearing (closed mesh tearing also supported but without textures)
-Inextensible ropes (via long range attachments)
-Diffuse particles (foam rendering with SSF Pro)
-Win x86 support (experimental)

v0.45
-Ropes rendering
-Fixed FlexParticlesRenderer size control
-Fixed Fluid spacing control
-Added SmallScale scene example

v0.4
-Dynamic containers
-Fluid emitters
-Shape-match transforms generation 
-Improved sphere ponints renderer
-Integration with Screen Space Fluids (SSF)
-FlexSolver/Container refactor
-FlexProcessors
-Some bugs fixed

v0.3
-Flex Components refactor
-Fast soft bodies rendering via SkinnedRenderer 
-Ropes
-Inflatables
-Air Meshes
-Update to all demo scenes

v0.2
-Deformable mesh rendering
-Mouse interactions
-Colliders update
-Support for Win32

v0.1 - first public release - early preview
-GUI

v0.05 - pre-release version 
-Rigid bodies
-Soft bodies
-Fluids 
-Cloths

v0.01 - pre-release version 
-fluids vs. triangle mesh collisions

------ToDo------
-Simulation recording
-Fix collision primitives
-Callbacks
-FlexPlanes

FLEX API:
-collision primitives
-multiple solvers

FLEX EXT:
-callbacks
-tetrahedral bodies

RENDERING:
-ropes texturing

IMPROVEMENTS:
-prevent very long processing times
-soft-body cutting

------Known Issues------
-deselect Flex GameObject in the editor during play mode. For some obscure Unity related reason, it degrades performance.

-some rigid bodies drift away. Seems like a problem with Flex's shape matching constraints. I have reported this to NVidia.

-do not duplicate ropes as the visual meshes will be mangled. Instead, simply create a new rope from scratch.


------FAQ------
"How do you make it so it actually looks like a liquid instead of looking at the little balls"

 NVidia Flex itself is a pure physics library and uFlex priority is to first cover all the core physics simulation functionality. 
 However, uFlex already provides rigid and soft body mesh rendering via Unity meshes. 

 Fluid rendering is much more complex topic and will take more time to develop and integrate with Unity. 
 Besides porting the GLSL fluid shaders from flex demos it would require adapting them to work with Unity's rendering engine (lights, shadows etc.)
 
 Therefore, the Screen Space Fluids (SSF) rendering is available as a separate asset, which can be used on its own without uFlex, for example, with Unity's particle systems.
 The  paid SSF Pro version includes higher quality fluid shaders and more features such as fluid translucency and refractions.
 
 Alternatively, nothing prevents you from using your own (e.g. screen-space fluids or marching cubes) 
 or 3rd party fluid rendering solutions. Flex already gives access to all the necessary data (even anisotropy of particular particles).﻿