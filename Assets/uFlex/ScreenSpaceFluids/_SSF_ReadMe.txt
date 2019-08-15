Screen Space Fluids (SSF) v0.1
support: korzenio [at] gmail.com

Screen Space Fluids (SSF) is a set of shaders for rendering the surface of particle-based fluids with real-time performance and configurable speed/quality. 

The method is not based on polygonization and as such circumvents the usual grid artifacts of marching cubes.
The shaders smooths the surface to prevent the fluid from looking "blobby" or jelly-like.
It only renders the surface where it is visible, and has inherent view-dependent level-of-detail. 
All the processing, rendering and shading steps are directly implemented on graphics hardware. 

SSF provides only fluid visualization that can be used, for example, with Unity's Particle System. 
However, SSF works best with some third-party fluid simulation library such as uFlex (http://u3d.as/r50). 

SSF is under development and, at the current stage, is targeted at early adopters. 
Any bug reports and feature requests are more than welcome.

----Requirements---
This asset requires GPU with Compute Buffers support (e.g. DX11)


----QuickStart-----
1. Import ScreenSpaceFluids package
2. If Standard Effects package already imported -> delete ScreenSpaceFluids/Standard Assets folder (only ImageEffectsBase.cs required)
3. Add TurnOnDepthBuffer component to the Camera. 
4. Add SSF_ComposeFluid image effect to the Camera. Set the Blurred Depth Texture Slot to SSF_BlurredDepthTexture and set the Cubemap (usually same as skybox)
5. Add SSF_FluidRenderer to a ParticleSystem. Set Depth Texture Slot to SSF_DepthTexture, urred Depth Texture Slot to SSF_BlurredDepthTexture and Blurred Depth Temp Texture to SSF_BlurredDepthTempTexture.
6. Enjoy!

For video tutorials subscribe to SSF channel 
(https://www.youtube.com/playlist?list=PLkKm77tYcGu9Fz9NlMN0S-SgLYKC8oMzb)


----ChangeLog-----
v0.11
-Fix for the inversed z-buffer in Unity 5.5

v0.1 - first public release
-Opaque Ambient + Diffuse + Specular + Reflections fluid shader
