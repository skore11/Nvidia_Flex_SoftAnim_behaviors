using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace uFlex
{

    /// <summary>
    /// GUI for creating Flex bodies (assets)
    /// </summary>
    public class FlexWindow : EditorWindow
    {

        string newName = "FlexRigidBody";

        bool phaseSelfCollide = false;
        bool phaseFluid = false;
        bool phaseSelfCollideFilter = false;

        int group = -1;
        float mass = 1.0f;
        Flex.Phase phase = 0;
        FlexInteractionType interactionType = FlexInteractionType.None;

        float stretchStiffness = 1.0f;
        float bendStiffness = 1.0f;
        float tetherStiffness = 0.0f;
        float tetherGive = 0.0f;
        float pressure = 1.0f;
        float maxStrain = 2.0f;
        int maxSplits = 4;
        bool inflatable = false;
        bool tearable = false;

        float rigidRadius = 1.0f;
        float rigidExpand = 0.0f;

        float particleSpacing = 1.0f;
        float volumeSampling = 1.0f;
        float surfaceSampling = 0.0f;
        float clusterSpacing = 1.0f;
        float clusterRadius = 0.0f;
        float clusterStiffness = 0.5f;
        float linkRadius = 0.0f;
        float linkStiffness = 1.0f;

        float skinFalloff = 1.0f;
        float skinMaxDist = 100.0f;

        float spacingRandomness = 0.0f;
    //    float massRandomness = 0.0f;
        float fluidSpacing = 1.0f;

        
        int ropeSegments = 100;
        float ropeRestLength = 0.5f;
        float ropeStretch = 1.0f;
        float ropeBend = 0.5f;
        float ropeGive = 0.0f;
        float ropeMeshRadius = 0.5f;

        bool ropeLockFirt = false;
        float ropeInextensible = 0.0f;
        bool ropeLockLast = false;

        int fluidX = 32;
        int fluidY = 32;
        int fluidZ = 32;


        //Color color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
        Color color = Color.green;
        Mesh inputMesh;
        bool softMesh = true;

        GameObject nodePrefab;
        FlexBodyType bodyType = FlexBodyType.Rigid;
        GUIContent[] tabCaptions = new GUIContent[] {
                new GUIContent("Rigid", "Create a shape body asset from a closed triangle mesh. The mesh is first voxelized at a spacing specified by the radius, and particles are placed at occupied voxels."),
                new GUIContent("Soft", "Create a shape body asset from a closed triangle mesh. The mesh is first voxelized at a spacing specified by the radius, and particles are placed at occupied voxels."),
                new GUIContent("Cloth", "Create a cloth asset consisting of stretch and bend distance constraints given an indexed triangle mesh. Stretch constraints will be placed along triangle edges, while bending constraints are placed over two edges."),
            //    new GUIContent("Inflatable", "an inflatable is a range of dynamic triangles (wound CCW) that represent a closed mesh. Each inflatable has a given rest volume, constraint scale (roughly equivalent to stiffness), and over pressure that controls how much the shape is inflated."),
                new GUIContent("Fluid", "Create a xyz block of fluid with given spacing (mind to set the fluid rest density parameter accordingly in the solver to avoid explosions or implosions)"),
                new GUIContent("Rope", "Create a rope"),
             //   new GUIContent("Tearable", "Create a cloth asset consisting of stretch and bend distance constraints given an indexed triangle mesh. This creates an asset with the same structure as Cloth, however tether constraints are not supported, and additional information regarding mesh topology will be stored with the asset.")
        };
        int tab = 0;

        bool warnLevel = false;
        void OnGUI()
        {
            EditorGUILayout.Separator();
            GUILayout.Label("uFlex v0.55", EditorStyles.whiteLargeLabel);

            EditorGUILayout.Separator();
            newName = EditorGUILayout.TextField("Name", newName);
            mass = EditorGUILayout.FloatField(new GUIContent("Particle Mass", "Particle mass"), mass);
            group = EditorGUILayout.IntField(new GUIContent("Group", "Body collision group, leave -1 for automatic group generation and be careful when mixing manual and automatic settings"), group);
            color = EditorGUILayout.ColorField(new GUIContent("Color", "Body color (debug info)"), color);
            //    spacingRandomness = EditorGUILayout.FloatField("Spacing Randomness", spacingRandomness);
            //    massRandomness = EditorGUILayout.FloatField("Mass Randomness", massRandomness);

            EditorGUILayout.Separator();
            GUILayout.Label("Phase Flags", EditorStyles.boldLabel);
            interactionType = (FlexInteractionType)EditorGUILayout.EnumPopup(new GUIContent("Interaction Type", "A type of interaction within the same group (usuallly also the same body)"), interactionType);

            switch (interactionType)
            {
                case FlexInteractionType.None:
                    phaseSelfCollide = false;
                    phaseSelfCollideFilter = false;
                    phaseFluid = false;
                    break;

                case FlexInteractionType.SelfCollideAll:
                    phaseSelfCollide = true;
                    phaseSelfCollideFilter = false;
                    phaseFluid = false;
                    break;

                case FlexInteractionType.SelfCollideFiltered:
                    phaseSelfCollide = true;
                    phaseSelfCollideFilter = true;
                    phaseFluid = false;
                    break;

                case FlexInteractionType.Fluid:
                    phaseSelfCollide = true;
                    phaseSelfCollideFilter = false;
                    phaseFluid = true;
                    break;

            }

            EditorGUI.BeginDisabledGroup(true);
            phaseSelfCollide = EditorGUILayout.Toggle(new GUIContent("Self Collide", "If set this particle will interact with particles of the same group."), phaseSelfCollide);
            phaseSelfCollideFilter = EditorGUILayout.Toggle(new GUIContent("Self Collide Filter", "If set this particle will ignore collisions with particles closer than the radius in the rest pose, this flag should not be specified unless valid rest positions have been specified using flexSetRestParticles()"), phaseSelfCollideFilter);
            phaseFluid = EditorGUILayout.Toggle(new GUIContent("Fluid", "If set this particle will generate fluid density constraints for its overlapping neighbors."), phaseFluid);
            EditorGUI.EndDisabledGroup();

            //apply setting to phase
            phase = 0;
            if (phaseSelfCollide) phase |= Flex.Phase.eFlexPhaseSelfCollide;
            if (phaseSelfCollideFilter) phase |= Flex.Phase.eFlexPhaseSelfCollideFilter;
            if (phaseFluid) phase |= Flex.Phase.eFlexPhaseFluid;

            tab = Tabs(tabCaptions, tab);

            if (tab == 0)
            {
                GUILayout.Label("Rigid Body", EditorStyles.boldLabel);
                bodyType = FlexBodyType.Rigid;
                if (inputMesh == null)
                    EditorGUILayout.HelpBox("Pick a CLOSED triangle mesh!", warnLevel ? MessageType.Warning : MessageType.Info);
                inputMesh = EditorGUILayout.ObjectField(new GUIContent("Input Mesh", "The input mesh, note that using large meshes may result in very long processing times or even running out of memory"), inputMesh, typeof(Mesh), true) as Mesh;


                rigidRadius = EditorGUILayout.FloatField(new GUIContent("Radius", "The spacing used for voxelization, note that the number of voxels grows proportional to the inverse cube of radius, currently this method limits construction to resolutions < 64 ^ 3"), rigidRadius);
                rigidExpand = EditorGUILayout.FloatField(new GUIContent("Expand", "Particles will be moved inwards (if negative) or outwards (if positive) from the surface of the mesh according to this factor"), rigidExpand);

            }

            if (tab == 1)
            {
                GUILayout.Label("Soft Body", EditorStyles.boldLabel);
                bodyType = FlexBodyType.Soft;
                if (inputMesh == null)
                    EditorGUILayout.HelpBox("Pick a CLOSED triangle mesh!", warnLevel ? MessageType.Warning : MessageType.Info);
                inputMesh = EditorGUILayout.ObjectField(new GUIContent("Input Mesh", "The input mesh, note that using large meshes may result in very long processing times or even running out of memory"), inputMesh, typeof(Mesh), true) as Mesh;
                particleSpacing = EditorGUILayout.FloatField(new GUIContent("Particle Spacing", "The spacing to use when creating particles"), particleSpacing);
                volumeSampling = EditorGUILayout.FloatField(new GUIContent("Volume Sampling", "Control the resolution the mesh is voxelized at in order to generate interior sampling, if the mesh is not closed then this should be set to zero and surface sampling should be used instead"), volumeSampling);
                surfaceSampling = EditorGUILayout.FloatField(new GUIContent("Surface Sampling", "	Controls how many samples are taken of the mesh surface, this is useful to ensure fine features of the mesh are represented by particles, or if the mesh is not closed"), surfaceSampling);

                EditorGUILayout.Separator();
                clusterSpacing = EditorGUILayout.FloatField(new GUIContent("Cluster Spacing", "	The spacing for shape-matching clusters, should be at least the particle spacing"), clusterSpacing);
                clusterRadius = EditorGUILayout.FloatField(new GUIContent("Cluster Radius", "	Controls the overall size of the clusters, this controls how much overlap the clusters have which affects how smooth the final deformation is, if parts of the body are detaching then it means the clusters are not overlapping sufficiently to form a fully connected set of clusters"), clusterRadius);
                clusterStiffness = EditorGUILayout.FloatField(new GUIContent("Cluster Stiffness", "Controls the stiffness of the resulting clusters"), clusterStiffness);

                EditorGUILayout.Separator();
                linkRadius = EditorGUILayout.FloatField(new GUIContent("Link Radius", "Any particles below this distance will have additional distance constraints created between them"), linkRadius);
                linkStiffness = EditorGUILayout.FloatField(new GUIContent("Link Stiffness", "	The stiffness of distance links"), linkStiffness);

                clusterStiffness = Mathf.Clamp(clusterStiffness, 0.0f, 1.0f);
                linkStiffness = Mathf.Clamp(linkStiffness, 0.0f, 1.0f);

                EditorGUILayout.Separator();
                softMesh = EditorGUILayout.Toggle(new GUIContent("Soft Mesh", "Adds Unity's MeshFilter and MeshRenderer"), softMesh);
                if (softMesh)
                {
                    skinFalloff = EditorGUILayout.FloatField("Skin Falloff: ", skinFalloff);
                    skinMaxDist = EditorGUILayout.FloatField("Skin Max Distance: ", skinMaxDist);
                }
            }

            if (tab == 2)
            {
                GUILayout.Label("Cloth", EditorStyles.boldLabel);
                bodyType = FlexBodyType.Cloth;
                if (inputMesh == null)
                    EditorGUILayout.HelpBox("Pick an input mesh!", warnLevel ? MessageType.Warning : MessageType.Info);
                inputMesh = EditorGUILayout.ObjectField(new GUIContent("Input Mesh", "The input mesh, note that using large meshes may result in very long processing times or even running out of memory"), inputMesh, typeof(Mesh), true) as Mesh;


                stretchStiffness = EditorGUILayout.FloatField(new GUIContent("Stretch Stiffness", "The stiffness coefficient for stretch constraints"), stretchStiffness);
                bendStiffness = EditorGUILayout.FloatField(new GUIContent("Bend Stifness", "The stiffness coefficient used for bending constraints"), bendStiffness);
                tetherStiffness = EditorGUILayout.FloatField(new GUIContent("Tether Stifness", "If > 0.0f then the function will create tethers attached to particles with zero inverse mass. These are unilateral, long-range attachments, which can greatly reduce stretching even at low iteration counts."), tetherStiffness);
                tetherGive = EditorGUILayout.FloatField(new GUIContent("Tether Give", "Because tether constraints are so effective at reducing stiffness, it can be useful to allow a small amount of extension before the constraint activates."), tetherGive);
                
                inflatable = EditorGUILayout.Toggle(new GUIContent("Inflatable", "An inflatable is a range of dynamic triangles (wound CCW) that represent a closed mesh. Each inflatable has a given rest volume, constraint scale (roughly equivalent to stiffness), and over pressure that controls how much the shape is inflated."), inflatable);
                if (inflatable)
                {
                    EditorGUILayout.HelpBox("Pick a CLOSED triangle mesh!", warnLevel ? MessageType.Warning : MessageType.Info);
                    pressure = EditorGUILayout.FloatField(new GUIContent("Pressure", "	If > 0.0f then a volume (pressure) constraint will also be added to the asset, the rest volume and stiffness will be automatically computed by this function"), pressure);
                }
                
                tearable = EditorGUILayout.Toggle(new GUIContent("Tearable", "Create a cloth asset consisting of stretch and bend distance constraints given an indexed triangle mesh. This creates an asset with the same structure as Cloth, however tether constraints are not supported, and additional information regarding mesh topology will be stored with the asset."), tearable);
                if (tearable)
                {
                    maxStrain = EditorGUILayout.FloatField(new GUIContent("Max Strain", "The maximum allowable strain on each edge"), maxStrain);
                    maxSplits = EditorGUILayout.IntField(new GUIContent("Max Splits", "The maximum number of constraint breaks that will be performed, this controls the 'rate' of mesh tearing"), maxSplits);
                }

                stretchStiffness = Mathf.Clamp(stretchStiffness, 0.0f, 1.0f);
                bendStiffness = Mathf.Clamp(bendStiffness, 0.0f, 1.0f);
                tetherStiffness = Mathf.Clamp(tetherStiffness, 0.0f, 1.0f);
                tetherGive = Mathf.Clamp(tetherGive, -1.0f, 1.0f);
                pressure = Mathf.Clamp(pressure, -1.0f, 1.0f);

                maxStrain = Mathf.Clamp(maxStrain, 1.0f, 100.0f);
                maxSplits = Mathf.Clamp(maxSplits, 1, 1000);
            }

            //if (tab == 3)
            //{
            //    GUILayout.Label("Inflatable", EditorStyles.boldLabel);
            //    bodyType = FlexBodyType.Inflatable;
            //    if (inputMesh == null)
            //        EditorGUILayout.HelpBox("Pick a CLOSED triangle mesh!", warnLevel ? MessageType.Warning : MessageType.Info);
            //    inputMesh = EditorGUILayout.ObjectField(new GUIContent("Input Mesh", "The input mesh, note that using large meshes may result in very long processing times or even running out of memory"), inputMesh, typeof(Mesh), true) as Mesh;


            //    stretchStiffness = EditorGUILayout.FloatField(new GUIContent("Stretch Stiffness", "The stiffness coefficient for stretch constraints"), stretchStiffness);
            //    bendStiffness = EditorGUILayout.FloatField(new GUIContent("Bend Stifness", "The stiffness coefficient used for bending constraints"), bendStiffness);
            //    tetherStiffness = EditorGUILayout.FloatField(new GUIContent("Tether Stifness", "If > 0.0f then the function will create tethers attached to particles with zero inverse mass. These are unilateral, long-range attachments, which can greatly reduce stretching even at low iteration counts."), tetherStiffness);
            //    tetherGive = EditorGUILayout.FloatField(new GUIContent("Tether Give", "Because tether constraints are so effective at reducing stiffness, it can be useful to allow a small amount of extension before the constraint activates."), tetherGive);
            //    pressure = EditorGUILayout.FloatField(new GUIContent("Pressure", "	If > 0.0f then a volume (pressure) constraint will also be added to the asset, the rest volume and stiffness will be automatically computed by this function"), pressure);

            //    stretchStiffness = Mathf.Clamp(stretchStiffness, 0.0f, 1.0f);
            //    bendStiffness = Mathf.Clamp(bendStiffness, 0.0f, 1.0f);
            //    tetherStiffness = Mathf.Clamp(tetherStiffness, 0.0f, 1.0f);
            //    tetherGive = Mathf.Clamp(tetherGive, -1.0f, 1.0f);
            //    pressure = Mathf.Clamp(pressure, -1.0f, 1.0f);
            //}

            if (tab == 0 || tab == 1 || tab == 2)
            {
                EditorGUILayout.HelpBox("Processing large meshes or setting small spacings  may result in very long generation times or even running out of memory!", MessageType.Warning);
            }

            if (tab == 3)
            {
                GUILayout.Label("Fluid", EditorStyles.boldLabel);
                bodyType = FlexBodyType.Fluid;
                fluidSpacing = EditorGUILayout.FloatField("Spacing", fluidSpacing);
                spacingRandomness = EditorGUILayout.FloatField("Spacing Randomness", spacingRandomness);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Grid");
                fluidX = EditorGUILayout.IntField(fluidX);
                fluidY = EditorGUILayout.IntField(fluidY);
                fluidZ = EditorGUILayout.IntField(fluidZ);
                EditorGUILayout.EndHorizontal();

                fluidX = Mathf.Clamp(fluidX, 1, 128);
                fluidY = Mathf.Clamp(fluidY, 1, 128);
                fluidZ = Mathf.Clamp(fluidZ, 1, 128);

                if(fluidX * fluidY * fluidZ > 65536)
                    EditorGUILayout.HelpBox("Processing such a large number of particles ("+ (fluidX * fluidY * fluidZ) +" > 65536) may be slow", MessageType.Warning);
            }


            if (tab == 4)
            {
                GUILayout.Label("Rope", EditorStyles.boldLabel);
                bodyType = FlexBodyType.Rope;

                ropeSegments = EditorGUILayout.IntField(new GUIContent("Segments Count", "The number Of segments. Total rope length = (segments count * rest length)"), ropeSegments);
                ropeRestLength = EditorGUILayout.FloatField(new GUIContent("Rest Length", "The rest length between subseqent rope particles"), ropeRestLength);
                ropeStretch = EditorGUILayout.FloatField(new GUIContent("Stretch Stiffness", "The stiffness coefficient for stretch constraints"), ropeStretch);
                ropeBend = EditorGUILayout.FloatField(new GUIContent("Bend Stifness", "The stiffness coefficient used for bending constraints"), ropeBend);
                ropeGive = EditorGUILayout.FloatField(new GUIContent("Rope Give", "It can be useful to allow a small amount of extension before the constraint activates."), ropeGive);
                ropeMeshRadius = EditorGUILayout.FloatField(new GUIContent("Rope Visual Mesh Radius", "The radius of the visual mesh."), ropeMeshRadius);
                ropeLockFirt = EditorGUILayout.Toggle(new GUIContent("Lock Start", "Sets mass of rope's first particle to 0"), ropeLockFirt);
                if (ropeLockFirt)
                {
                    ropeInextensible = EditorGUILayout.FloatField(new GUIContent("Inextensibility", "if >0 make rope inextensible using long range attachments with given stiffness"), ropeInextensible);
                }

                ropeLockLast = EditorGUILayout.Toggle(new GUIContent("Lock End", "Sets mass of rope's last particle to 0"), ropeLockLast);


                ropeRestLength = Mathf.Clamp(ropeRestLength, 0.00001f, 10f);
                ropeSegments = Mathf.Clamp(ropeSegments, 3, 999);
                ropeStretch = Mathf.Clamp(ropeStretch, 0.0f, 1.0f);
                ropeBend = Mathf.Clamp(ropeBend, 0.0f, 1.0f);
                ropeInextensible = Mathf.Clamp(ropeInextensible, 0.0f, 1.0f);
                ropeGive = Mathf.Clamp(ropeGive, -1.0f, 1.0f);
                ropeMeshRadius = Mathf.Clamp(ropeMeshRadius, 0.0001f, 100000.0f);
            }
            EditorGUILayout.Separator();

            //if (tab == 6)
            //{
            //    GUILayout.Label("Tearable", EditorStyles.boldLabel);
            //    bodyType = FlexBodyType.Tearable;
            //    if (inputMesh == null)
            //        EditorGUILayout.HelpBox("Pick an input mesh!", warnLevel ? MessageType.Warning : MessageType.Info);
            //    inputMesh = EditorGUILayout.ObjectField(new GUIContent("Input Mesh", "The input mesh, note that using large meshes may result in very long processing times or even running out of memory"), inputMesh, typeof(Mesh), true) as Mesh;


            //    stretchStiffness = EditorGUILayout.FloatField(new GUIContent("Stretch Stiffness", "The stiffness coefficient for stretch constraints"), stretchStiffness);
            //    bendStiffness = EditorGUILayout.FloatField(new GUIContent("Bend Stifness", "The stiffness coefficient used for bending constraints"), bendStiffness);
            //    //    tetherStiffness = EditorGUILayout.FloatField(new GUIContent("Tether Stifness", "If > 0.0f then the function will create tethers attached to particles with zero inverse mass. These are unilateral, long-range attachments, which can greatly reduce stretching even at low iteration counts."), tetherStiffness);
            //    //   tetherGive = EditorGUILayout.FloatField(new GUIContent("Tether Give", "Because tether constraints are so effective at reducing stiffness, it can be useful to allow a small amount of extension before the constraint activates."), tetherGive);
            //    //    pressure = EditorGUILayout.FloatField(new GUIContent("Pressure", "	If > 0.0f then a volume (pressure) constraint will also be added to the asset, the rest volume and stiffness will be automatically computed by this function"), pressure);
            //    pressure = 0;
            //    tetherStiffness = 0;
            //    tetherGive = 0;

            //    stretchStiffness = Mathf.Clamp(stretchStiffness, 0.0f, 1.0f);
            //    bendStiffness = Mathf.Clamp(bendStiffness, 0.0f, 1.0f);
            //    //tetherStiffness = Mathf.Clamp(tetherStiffness, 0.0f, 1.0f);
            //    //tetherGive = Mathf.Clamp(tetherGive, -1.0f, 1.0f);
            //    //   pressure = Mathf.Clamp(pressure, -1.0f, 1.0f);
            //}

            if (GUILayout.Button("Generate"))
            {

                if ((bodyType != FlexBodyType.Fluid && bodyType != FlexBodyType.Rope) && inputMesh == null)
                {
                    warnLevel = true;
                    return;
                }


                GameObject go = new GameObject();
                go.name = newName;

                //FlexBody flexBody = go.GetComponent<FlexBody>();
                //if (flexBody == null)
                //    flexBody = go.AddComponent<FlexBody>();

                //flexBody.m_bodyType = bodyType;
                //flexBody.m_group = group;
                //flexBody.m_interactionType = interactionType;
                ////flexBody.m_phase = phase;
                //flexBody.m_mass = mass;
                //flexBody.m_color = color;


                if (bodyType == FlexBodyType.Fluid)
                {
                    GenerateFluid(go);
                    go.AddComponent<FlexParticlesRenderer>();

                }
                else if (bodyType == FlexBodyType.Rope)
                {
                    GenerateRope(go);

                    FlexParticlesRenderer partRend = go.AddComponent<FlexParticlesRenderer>();
                    partRend.enabled = false;
                }
                else
                {
                    GenerateFromMesh(go);
                    FlexParticlesRenderer partRend = go.AddComponent<FlexParticlesRenderer>();
                    partRend.enabled = false;
                }

                Selection.activeGameObject = go;
                //   this.Close();
            }
        }
        

        private void GenerateFluid(GameObject go)
        {
            int particlesCount = fluidX * fluidY * fluidZ;

            FlexParticles part = go.AddComponent<FlexParticles>();
            part.m_particlesCount = particlesCount;
            part.m_maxParticlesCount = particlesCount;
            part.m_particles = new Particle[particlesCount];
            part.m_velocities = new Vector3[particlesCount];
            part.m_restParticles = new Particle[particlesCount];
            part.m_smoothedParticles = new Particle[particlesCount];
            part.m_phases = new int[particlesCount];
            part.m_particlesActivity = new bool[particlesCount];
            part.m_densities = new float[particlesCount];
            part.m_colours = new Color[particlesCount];
            part.m_colour = color;
            part.m_interactionType = interactionType;
            part.m_collisionGroup = group;
            part.m_bounds.SetMinMax(new Vector3(), new Vector3(fluidX * fluidSpacing, fluidY * fluidSpacing, fluidZ * fluidSpacing));
            part.m_type = FlexBodyType.Fluid;

            int i = 0;
            float invMass = 1.0f / mass;
            for (int x = 0; x < fluidX; x++)
            {
                for (int y = 0; y < fluidY; y++)
                {
                    for (int z = 0; z < fluidZ; z++)
                    {
       
                        part.m_particles[i].pos = new Vector3(x, y, z) * fluidSpacing;
                        part.m_particles[i].invMass = invMass;
                        //   flexBody.m_colours[i] = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
                        part.m_colours[i] = color;
                        part.m_particlesActivity[i] = true;

                        part.m_restParticles[i] = part.m_particles[i];
                        part.m_smoothedParticles[i] = part.m_particles[i];
                        part.m_phases[i] = (int)phase;


                        if (spacingRandomness != 0)
                            part.m_particles[i].pos += UnityEngine.Random.insideUnitSphere * spacingRandomness;

                        i++;
                    }
                }
            }
            
        }

        private void GenerateRope(GameObject go)
        {
            int particlesCount = ropeSegments + 1;

            FlexParticles part = go.AddComponent<FlexParticles>();
            part.m_particlesCount = particlesCount;
            part.m_maxParticlesCount = particlesCount;
            part.m_particles = new Particle[particlesCount];
            part.m_restParticles = new Particle[particlesCount];
            part.m_smoothedParticles = new Particle[particlesCount];
            part.m_phases = new int[particlesCount];
            part.m_colours = new Color[particlesCount];
            part.m_velocities = new Vector3[particlesCount];
            part.m_particlesActivity = new bool[particlesCount];
            part.m_densities = new float[particlesCount];
            part.m_colours = new Color[particlesCount];
            part.m_colour = color;
            part.m_interactionType = interactionType;
            part.m_collisionGroup = group;
            
            part.m_bounds.SetMinMax(new Vector3(), new Vector3(ropeSegments * ropeRestLength, 1, 1));

            for (int i = 0; i < particlesCount; i++)
            {
                part.m_particles[i].pos = new Vector3(i*ropeRestLength, 0, 0);
                part.m_particles[i].invMass =  1.0f / mass;
                part.m_colours[i] = color;
                part.m_particlesActivity[i] = true;

                part.m_restParticles[i] = part.m_particles[i];
                part.m_smoothedParticles[i] = part.m_particles[i];
                part.m_phases[i] = (int)phase;
            }

            if (ropeLockFirt)
            {
                part.m_particles[0].invMass = 0;
                part.m_restParticles[0].invMass = 0;
            }

            if (ropeLockLast)
            {
                part.m_particles[particlesCount - 1].invMass = 0;
                part.m_restParticles[particlesCount - 1].invMass = 0;
            }

            FlexSprings springs = go.AddComponent<FlexSprings>();

            if (ropeLockFirt && ropeInextensible > 0)
            {
                
                //distance + bend + tether (long range attachments)
                int springsCount = ropeSegments + (ropeSegments - 1) + ropeSegments;

                springs.m_springsCount = springsCount;
                springs.m_springsCount = springsCount;
                springs.m_springIndices = new int[springsCount * 2];
                springs.m_springCoefficients = new float[springsCount];
                springs.m_springRestLengths = new float[springsCount];

                for (int i = 0; i < ropeSegments; i++)
                {
                    springs.m_springIndices[i * 2 + 0] = i;
                    springs.m_springIndices[i * 2 + 1] = i + 1;

                    springs.m_springCoefficients[i] = ropeStretch;
                    springs.m_springRestLengths[i] = ropeRestLength * (1.0f + ropeGive);
                }

                for (int i = 0; i < ropeSegments - 1; i++)
                {
                    springs.m_springIndices[ropeSegments * 2 + i * 2 + 0] = i;
                    springs.m_springIndices[ropeSegments * 2 + i * 2 + 1] = i + 2;

                    springs.m_springCoefficients[ropeSegments + i] = ropeBend;
                    springs.m_springRestLengths[ropeSegments + i] = ropeRestLength * 2 * (1.0f + ropeGive);
                }

                for (int i = 2; i < ropeSegments; i++)
                {

                    springs.m_springIndices[(ropeSegments + ropeSegments - 1) * 2 + i * 2 + 0] = 0;
                    springs.m_springIndices[(ropeSegments + ropeSegments - 1) * 2 + i * 2 + 1] = i + 1;

                    //negative stiffness means tether constraints 
                    //(i.e. prevents spring stretch but not its compression)
                    springs.m_springCoefficients[(ropeSegments + ropeSegments - 1) + i] = -ropeInextensible;
                    springs.m_springRestLengths[(ropeSegments + ropeSegments - 1) + i] = (i+1) * ropeRestLength;
                }
            }
            else
            {
                int springsCount = ropeSegments + ropeSegments - 1;
                springs.m_springsCount = springsCount;
                springs.m_springIndices = new int[springsCount * 2];
                springs.m_springCoefficients = new float[springsCount];
                springs.m_springRestLengths = new float[springsCount];

                for (int i = 0; i < ropeSegments; i++)
                {
                    springs.m_springIndices[i * 2 + 0] = i;
                    springs.m_springIndices[i * 2 + 1] = i + 1;

                    springs.m_springCoefficients[i] = ropeStretch;
                    springs.m_springRestLengths[i] = ropeRestLength * (1.0f + ropeGive);
                }

                for (int i = 0; i < ropeSegments - 1; i++)
                {
                    springs.m_springIndices[ropeSegments * 2 + i * 2 + 0] = i;
                    springs.m_springIndices[ropeSegments * 2 + i * 2 + 1] = i + 2;

                    springs.m_springCoefficients[ropeSegments + i] = ropeBend;
                    springs.m_springRestLengths[ropeSegments + i] = ropeRestLength * 2 * (1.0f + ropeGive);
                }
            }

            
         


            FlexRopeMesh frm = go.AddComponent<FlexRopeMesh>();
            frm.m_radius = ropeMeshRadius;
            frm.m_particles = part;
            frm.InitRope(part.m_particlesCount);

            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = frm.m_mesh;


            MeshRenderer rend = go.AddComponent<MeshRenderer>();


            Material mat = new Material(Shader.Find("Diffuse"));
            mat.name = this.newName + "Mat";
            mat.color = color;
            rend.material = mat;
        }

        private void GenerateFromMesh(GameObject go)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            Vector3[] vertices = inputMesh.vertices;
            int vertexCount = inputMesh.vertexCount;

            int[] triangles = inputMesh.triangles;
            int triIndicesCount = triangles.Length;
            int trianglesCount = triIndicesCount / 3;

            IntPtr flexAssetPtr = IntPtr.Zero;

            int[] uniqueVerticesIds = new int[vertexCount];
            int[] origToUniqueVertMapping = new int[vertexCount];
            int uniqueVerticesCount = FlexExt.flexExtCreateWeldedMeshIndices(vertices, vertexCount,  uniqueVerticesIds,  origToUniqueVertMapping, 0.00001f);

            Debug.Log("Welding Mesh: " + uniqueVerticesCount + "/" + vertexCount);

            Vector3[] uniqueVertices = new Vector3[uniqueVerticesCount];
            Vector4[] uniqueVerticesWithInvMass = new Vector4[uniqueVerticesCount];
            for (int i = 0; i < uniqueVerticesCount; i++)
            {
                uniqueVertices[i] = vertices[uniqueVerticesIds[i]];

                uniqueVerticesWithInvMass[i] = vertices[uniqueVerticesIds[i]];
                uniqueVerticesWithInvMass[i].w = 1.0f / this.mass;

                min = FlexUtils.Min(min, uniqueVertices[i]);
                max = FlexUtils.Max(max, uniqueVertices[i]);
            }

            int[] uniqueTriangles = new int[trianglesCount*3];
            for (int i = 0; i < trianglesCount * 3; i++)
            {
                uniqueTriangles[i] = origToUniqueVertMapping[triangles[i]];
            }


            if (bodyType == FlexBodyType.Rigid)
            {
                //flexAssetPtr = FlexExt.flexExtCreateRigidFromMesh(vertices, vertexCount, triangles, triIndicesCount, rigidRadius, rigidExpand);
                flexAssetPtr = FlexExt.flexExtCreateRigidFromMesh(uniqueVertices, uniqueVerticesCount, uniqueTriangles, triIndicesCount, rigidRadius, rigidExpand);
            }

            if (bodyType == FlexBodyType.Soft)
            {
                // flexAssetPtr = FlexExt.flexExtCreateSoftFromMesh(vertices, vertexCount, triangles, triIndicesCount, particleSpacing, volumeSampling, surfaceSampling, clusterSpacing, clusterRadius, clusterStiffness, linkRadius, linkStiffness);
                flexAssetPtr = FlexExt.flexExtCreateSoftFromMesh(uniqueVertices, uniqueVerticesCount, uniqueTriangles, triIndicesCount, particleSpacing, volumeSampling, surfaceSampling, clusterSpacing, clusterRadius, clusterStiffness, linkRadius, linkStiffness);
            }

            if (bodyType == FlexBodyType.Cloth)
            {
                // flexAssetPtr = FlexExt.flexExtCreateClothFromMesh(verticesWithInvMass, vertexCount, triangles, trianglesCount, stretchStiffness, bendStiffness, tetherStiffness, tetherGive, 0);
                if (!tearable)
                {
                    if (inflatable)
                        flexAssetPtr = FlexExt.flexExtCreateClothFromMesh(uniqueVerticesWithInvMass, uniqueVerticesCount, uniqueTriangles, trianglesCount, stretchStiffness, bendStiffness, tetherStiffness, tetherGive, pressure);
                    else
                        flexAssetPtr = FlexExt.flexExtCreateClothFromMesh(uniqueVerticesWithInvMass, uniqueVerticesCount, uniqueTriangles, trianglesCount, stretchStiffness, bendStiffness, tetherStiffness, tetherGive, 0);
                }
                else
                {
                    if (inflatable)
                        flexAssetPtr = FlexExt.flexExtCreateTearingClothFromMesh(uniqueVerticesWithInvMass, uniqueVerticesCount, uniqueVerticesCount * 2, uniqueTriangles, trianglesCount, stretchStiffness, bendStiffness, pressure);
                    else
                        flexAssetPtr = FlexExt.flexExtCreateTearingClothFromMesh(uniqueVerticesWithInvMass, uniqueVerticesCount, uniqueVerticesCount * 2, uniqueTriangles, trianglesCount, stretchStiffness, bendStiffness, 0);
                }
            }

            //if (bodyType == FlexBodyType.Inflatable)
            //{
            ////    flexAssetPtr = FlexExt.flexExtCreateClothFromMesh(verticesWithInvMass, vertexCount, triangles, trianglesCount, stretchStiffness, bendStiffness, tetherStiffness, tetherGive, pressure);
            //    flexAssetPtr = FlexExt.flexExtCreateClothFromMesh(uniqueVerticesWithInvMass, uniqueVerticesCount, uniqueTriangles, trianglesCount, stretchStiffness, bendStiffness, tetherStiffness, tetherGive, pressure);
            //}


            //if (bodyType == FlexBodyType.Tearable)
            //{
            //    // flexAssetPtr = FlexExt.flexExtCreateClothFromMesh(verticesWithInvMass, vertexCount, triangles, trianglesCount, stretchStiffness, bendStiffness, tetherStiffness, tetherGive, 0);
            //    flexAssetPtr = FlexExt.flexExtCreateTearingClothFromMesh(uniqueVerticesWithInvMass, uniqueVerticesCount, uniqueVerticesCount * 2, uniqueTriangles, trianglesCount, stretchStiffness, bendStiffness, 0);
            //}


            if (flexAssetPtr != IntPtr.Zero)
            {      
                //Flex Asset Marshalling
                FlexExt.FlexExtAsset flexAsset = (FlexExt.FlexExtAsset)Marshal.PtrToStructure(flexAssetPtr, typeof(FlexExt.FlexExtAsset));
                Vector4[] particles = FlexUtils.MarshallArrayOfStructures<Vector4>(flexAsset.mParticles, flexAsset.mNumParticles);

                int[] springIndices = FlexUtils.MarshallArrayOfStructures<int>(flexAsset.mSpringIndices, flexAsset.mNumSprings * 2);
                float[] springRestLengths = FlexUtils.MarshallArrayOfStructures<float>(flexAsset.mSpringRestLengths, flexAsset.mNumSprings);
                float[] springCoefficients = FlexUtils.MarshallArrayOfStructures<float>(flexAsset.mSpringCoefficients, flexAsset.mNumSprings);

                int[] shapeIndices = FlexUtils.MarshallArrayOfStructures<int>(flexAsset.mShapeIndices, flexAsset.mNumShapeIndices);
                int[] shapeOffsets = FlexUtils.MarshallArrayOfStructures<int>(flexAsset.mShapeOffsets, flexAsset.mNumShapes);
                Vector3[] shapeCenters = FlexUtils.MarshallArrayOfStructures<Vector3>(flexAsset.mShapeCenters, flexAsset.mNumShapes);
                float[] shapeCoefficients = FlexUtils.MarshallArrayOfStructures<float>(flexAsset.mShapeCoefficients, flexAsset.mNumShapes);

                
                if (flexAsset.mNumParticles > 0)
                {
                    FlexParticles part = go.AddComponent<FlexParticles>();
                    part.m_particlesCount = flexAsset.mNumParticles;
                    part.m_maxParticlesCount = flexAsset.mMaxParticles;
                    part.m_particles = new Particle[flexAsset.mMaxParticles];
                    part.m_restParticles = new Particle[flexAsset.mMaxParticles];
                    part.m_smoothedParticles = new Particle[flexAsset.mMaxParticles];
                    part.m_colours = new Color[flexAsset.mMaxParticles];
                    part.m_velocities = new Vector3[flexAsset.mMaxParticles];
                    part.m_densities = new float[flexAsset.mMaxParticles];
                    part.m_phases = new int[flexAsset.mMaxParticles];
                    part.m_particlesActivity = new bool[flexAsset.mMaxParticles];


                    part.m_colour = color;
                    part.m_interactionType = interactionType;
                    part.m_collisionGroup = group;
                    part.m_bounds.SetMinMax(min, max);

                    for (int i = 0; i < flexAsset.mNumParticles; i++)
                    {
                        part.m_particles[i].pos = particles[i];
                        part.m_particles[i].invMass = particles[i].w;
                        part.m_restParticles[i] = part.m_particles[i];
                        part.m_smoothedParticles[i] = part.m_particles[i];
                        part.m_colours[i] = color;
                        part.m_particlesActivity[i] = true;

                        
                        part.m_phases[i] = (int)phase;



                        //if (spacingRandomness != 0)
                        //{
                        //    part.m_particles[i].pos  += UnityEngine.Random.insideUnitSphere * spacingRandomness;
                        //}
                    }

                }


                if (flexAsset.mNumSprings > 0)
                {
                    FlexSprings springs = go.AddComponent<FlexSprings>();
                    springs.m_springsCount = flexAsset.mNumSprings;
                    springs.m_springIndices = springIndices;
                    springs.m_springRestLengths = springRestLengths;
                    springs.m_springCoefficients = springCoefficients;


                }

                if(flexAsset.mNumTriangles > 0)
                {
                    FlexTriangles tris = go.AddComponent<FlexTriangles>();
                    tris.m_trianglesCount = trianglesCount;
                    tris.m_triangleIndices = uniqueTriangles; 

                }


                FlexShapeMatching shapes = null;
                if (flexAsset.mNumShapes > 0)
                {
                    shapes = go.AddComponent<FlexShapeMatching>();
                    shapes.m_shapesCount = flexAsset.mNumShapes;
                    shapes.m_shapeIndicesCount = flexAsset.mNumShapeIndices;
                    shapes.m_shapeIndices = shapeIndices;
                    shapes.m_shapeOffsets = shapeOffsets;
                    shapes.m_shapeCenters = shapeCenters;
                    shapes.m_shapeCoefficients = shapeCoefficients;
                    shapes.m_shapeTranslations = new Vector3[flexAsset.mNumShapes];
                    shapes.m_shapeRotations = new Quaternion[flexAsset.mNumShapes];
                    shapes.m_shapeRestPositions = new Vector3[flexAsset.mNumShapeIndices];

                    int shapeStart = 0;
                    int shapeIndex = 0;
                    int shapeIndexOffset = 0;
                    for (int s = 0; s < shapes.m_shapesCount; s++)
                    {
                        shapes.m_shapeTranslations[s] = new Vector3();
                        shapes.m_shapeRotations[s] = Quaternion.identity;

                    //    int indexOffset = shapeIndexOffset;

                        shapeIndex++;

                        int shapeEnd = shapes.m_shapeOffsets[s];

                        for (int i = shapeStart; i < shapeEnd; ++i)
                        {
                            int p = shapes.m_shapeIndices[i];

                            // remap indices and create local space positions for each shape
                            Vector3 pos = particles[p];
                            shapes.m_shapeRestPositions[shapeIndexOffset] = pos - shapes.m_shapeCenters[s];

                            //   m_shapeIndices[shapeIndexOffset] = shapes.m_shapeIndices[i] + particles.m_particlesIndex;

                            shapeIndexOffset++;
                        }

                        shapeStart = shapeEnd;



                    }
                }

                if (flexAsset.mInflatable)
                {
                    FlexInflatable infla = go.AddComponent<FlexInflatable>();
                    infla.m_pressure = flexAsset.mInflatablePressure;
                    infla.m_stiffness = flexAsset.mInflatableStiffness;
                    infla.m_restVolume = flexAsset.mInflatableVolume;
     
                }

                if (softMesh)
                {

                    Renderer rend = null;
                    if (bodyType == FlexBodyType.Soft)
                    {
                        float[] skinWeights = new float[this.inputMesh.vertexCount * 4];
                        int[] skinIndices = new int[this.inputMesh.vertexCount * 4];
                        FlexExt.flexExtCreateSoftMeshSkinning(this.inputMesh.vertices, this.inputMesh.vertexCount, shapes.m_shapeCenters, shapes.m_shapesCount, skinFalloff, skinMaxDist, skinWeights, skinIndices);

                        Mesh mesh = new Mesh();
                        mesh.name = this.inputMesh.name+"FlexMesh";
                        mesh.vertices = this.inputMesh.vertices;
                        mesh.triangles = this.inputMesh.triangles;
                        mesh.normals = this.inputMesh.normals;
                        mesh.uv = this.inputMesh.uv;

                        Transform[] bones = new Transform[shapes.m_shapesCount];
                        BoneWeight[] boneWeights = new BoneWeight[this.inputMesh.vertexCount];
                        Matrix4x4[] bindPoses = new Matrix4x4[shapes.m_shapesCount];

                        Vector3[] rigidRestPoses = new Vector3[shapes.m_shapesCount];

                        for (int i = 0; i < shapes.m_shapesCount; i++)
                        {
                            rigidRestPoses[i] = shapes.m_shapeCenters[i];

                            bones[i] = new GameObject("FlexShape_" + i).transform;
                            bones[i].parent = go.transform;
                            bones[i].localPosition = shapes.m_shapeCenters[i];
                            bones[i].localRotation = Quaternion.identity;

                            bindPoses[i] = bones[i].worldToLocalMatrix * go.transform.localToWorldMatrix;
                        }

                        for (int i = 0; i < this.inputMesh.vertexCount; i++)
                        {
                            boneWeights[i].boneIndex0 = skinIndices[i * 4 + 0];
                            boneWeights[i].boneIndex1 = skinIndices[i * 4 + 1];
                            boneWeights[i].boneIndex2 = skinIndices[i * 4 + 2];
                            boneWeights[i].boneIndex3 = skinIndices[i * 4 + 3];

                            boneWeights[i].weight0 = skinWeights[i * 4 + 0];
                            boneWeights[i].weight1 = skinWeights[i * 4 + 1];
                            boneWeights[i].weight2 = skinWeights[i * 4 + 2];
                            boneWeights[i].weight3 = skinWeights[i * 4 + 3];

                        }

                        mesh.bindposes = bindPoses;
                        mesh.boneWeights = boneWeights;

                        mesh.RecalculateNormals();
                        mesh.RecalculateBounds();

                        AssetDatabase.CreateAsset(mesh, "Assets/uFlex/Meshes/" + this.inputMesh.name + "FlexMesh.asset");
                        AssetDatabase.SaveAssets();

                        FlexSkinnedMesh skin = go.AddComponent<FlexSkinnedMesh>();
                        skin.m_bones = bones;
                        skin.m_boneWeights = boneWeights;
                        skin.m_bindPoses = bindPoses;

                        rend = go.AddComponent<SkinnedMeshRenderer>();
                        ((SkinnedMeshRenderer)rend).sharedMesh = mesh;
                        ((SkinnedMeshRenderer)rend).updateWhenOffscreen = true;
                        ((SkinnedMeshRenderer)rend).quality = SkinQuality.Bone4;
                        ((SkinnedMeshRenderer)rend).bones = bones;

                        
                    }

                    if (bodyType == FlexBodyType.Rigid)
                    {

                        go.AddComponent<FlexRigidTransform>();
                        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
                        meshFilter.sharedMesh = this.inputMesh;
   
                        rend = go.AddComponent<MeshRenderer>();
                    }

                    if (bodyType == FlexBodyType.Cloth)
                    {
                        if (tearable)
                        {
                            FlexTearableMesh tearableMesh = go.AddComponent<FlexTearableMesh>();
                            tearableMesh.m_stretchKs = stretchStiffness;
                            tearableMesh.m_bendKs = bendStiffness;
                            tearableMesh.m_maxSplits = maxSplits;
                            tearableMesh.m_maxStrain = maxStrain;
                        }
                        else
                        {
                            go.AddComponent<FlexClothMesh>();
                        }

                        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
                        meshFilter.sharedMesh = this.inputMesh;
                        rend = go.AddComponent<MeshRenderer>();
                    }


                    Material mat = new Material(Shader.Find("Diffuse"));
                    mat.name = this.newName + "Mat";
                    mat.color = color;
                    rend.material = mat;

                }
            }


        }


        [MenuItem("Tools/uFlex")]
        public static void ShowWindows()
        {
            FlexSolver solver = FindObjectOfType<FlexSolver>();
            if(solver == null)
            {
                Debug.Log("No FlexSolver in scene. Creating a default one (Assets/uFlex/Prefabs/Flex.prefab)");
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/uFlex/Prefabs/Flex.prefab");
                
                GameObject flex = GameObject.Instantiate(prefab);
                flex.name = "Flex";
            }

            EditorWindow.GetWindow<FlexWindow>(false, "uFlex", true);
            
        }

        private void OnTabChange(int tabId)
        {
            switch(tabId)
            {
                case 0: //rigid
                    newName = "uFlexRigidBody";
                    interactionType = FlexInteractionType.None;
                    break;

                case 1: //soft
                    newName = "uFlexSoftBody";
                    interactionType = FlexInteractionType.SelfCollideFiltered;
                    break;

                case 2: //cloth
                    newName = "uFlexCloth";
                    interactionType = FlexInteractionType.SelfCollideFiltered;
                    break;

                //case 3: //inflatable
                //    newName = "uFlexInflatable";
                //    interactionType = FlexInteractionType.SelfCollideFiltered;
                //    pressure = 1;
                //    break;

                case 3: //fluid
                    newName = "uFlexFluid";
                    interactionType = FlexInteractionType.Fluid;
                    break;

                case 4: //rope
                    newName = "uFlexRope";
                    interactionType = FlexInteractionType.SelfCollideFiltered;
                    break;

                //case 6: //tearable
                //    newName = "uFlexTearable";
                //    interactionType = FlexInteractionType.SelfCollideFiltered;
                //    break;
            }
        }

        public int Tabs(GUIContent[] options, int selected)
        {
            const float DarkGray = 0.4f;
            const float LightGray = 0.9f;
            const float StartSpace = 10;

            GUILayout.Space(StartSpace);
            Color storeColor = GUI.backgroundColor;
            Color highlightCol = new Color(LightGray, LightGray, LightGray);
            Color bgCol = new Color(DarkGray, DarkGray, DarkGray);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding.bottom = 8;

            GUILayout.BeginHorizontal();
            {   //Create a row of buttons
                for (int i = 0; i < options.Length; ++i)
                {
                    GUI.backgroundColor = i == selected ? highlightCol : bgCol;
                    if (GUILayout.Button(options[i], buttonStyle))
                    {
                        selected = i; //Tab click
                        OnTabChange(selected);
                    }
                }
            }
            GUILayout.EndHorizontal();
            //Restore color
            GUI.backgroundColor = storeColor;
            //Draw a line over the bottom part of the buttons (ugly haxx)
            //var texture = new Texture2D(1, 1);
            //texture.SetPixel(0, 0, highlightCol);
            //texture.Apply();
            //GUI.DrawTexture(new Rect(0, buttonStyle.lineHeight + buttonStyle.border.top + buttonStyle.margin.top + StartSpace, Screen.width, 4), texture);

            return selected;
        }

    }

}
