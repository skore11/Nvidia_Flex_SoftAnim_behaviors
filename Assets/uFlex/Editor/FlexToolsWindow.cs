using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEditor.SceneManagement;

namespace uFlex
{
    public class FlexToolsWindow : EditorWindow
    {

        float m_mass = 1.0f;
        float m_clusterStiffness = 1.0f;
        float m_skinFalloff = 1.0f;
        float m_skinMaxDist = 100.0f;

        Mesh inputMesh;
        void OnGUI()
        {

            bool hasFlexComponent = false;
            GUILayout.Label("uFlex v0.5 - Developer Utils", EditorStyles.whiteLargeLabel);

            EditorGUILayout.HelpBox("This window is for advanced users. Improper use may break the uFlex objects.", MessageType.Warning);

            if (Selection.activeGameObject)
            {
                GUILayout.Label(Selection.activeGameObject.name, EditorStyles.boldLabel);

                FlexParticles particles = Selection.activeGameObject.GetComponent<FlexParticles>();
                if (particles)
                {
                    EditorGUILayout.Separator();
                    GUILayout.Label("Particles", EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal();
                    m_mass = EditorGUILayout.FloatField("Particles Masses: ", m_mass);
                    if (GUILayout.Button("Set "))
                    {
                        Debug.Log("Mass Set To " + m_mass);
                        float invMass = 1.0f / m_mass;
                        for (int i = 0; i < particles.m_particlesCount; i++)
                        {
                            //skip locked particles
                            if (particles.m_particles[i].invMass == 0)
                                continue;

                            particles.m_particles[i].invMass = invMass;
                        }
                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Apply Color"))
                    {

                        for (int i = 0; i < particles.m_particlesCount; i++)
                        {
                            particles.m_colours[i] = particles.m_colour;
                        }
                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                    if (GUILayout.Button("Randomize Colors"))
                    {

                        for (int i = 0; i < particles.m_particlesCount; i++)
                        {
                            //  particles.m_colours[i] = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
                            particles.m_colours[i] = UnityEngine.Random.ColorHSV(0.0f, 1.0f, 0.0f, 1.0f, 0.5f, 1.0f);
                        }
                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                    EditorGUILayout.EndHorizontal();

                    if (GUILayout.Button("Randomize Spacing"))
                    {

                        for (int i = 0; i < particles.m_particlesCount; i++)
                        {
                            particles.m_particles[i].pos += new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value) * 0.1f;
                     
                        }
                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                    if (GUILayout.Button("Activate All"))
                    {
                        particles.m_particlesActivity = new bool[particles.m_particlesCount];
                        for (int i = 0; i < particles.m_particlesCount; i++)
                        {
                            particles.m_particlesActivity[i] = true;

                        }
                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                    if (GUILayout.Button("Deactivate All"))
                    {
                        particles.m_particlesActivity = new bool[particles.m_particlesCount];
                        for (int i = 0; i < particles.m_particlesCount; i++)
                        {
                            particles.m_particlesActivity[i] = false;

                        }
                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                    //if (GUILayout.Button("Deactivate Half"))
                    //{
                    //    particles.m_particlesActivity = new bool[particles.m_particlesCount];
                    //    for (int i = 0; i < particles.m_particlesCount / 2; i++)
                    //    {
                    //        particles.m_particlesActivity[i] = true;
                    //    }

                    //    for (int i = particles.m_particlesCount / 2; i < particles.m_particlesCount; i++)
                    //    {
                    //        particles.m_particlesActivity[i] = false;
                    //    }
                    //    SceneView.RepaintAll();
                    //    EditorUtility.SetDirty(particles);

                    //}

                    if (GUILayout.Button("Upgrade to v0.5"))
                    {
                        particles.m_densities = new float[particles.m_maxParticlesCount];
                        particles.m_velocities = new Vector3[particles.m_maxParticlesCount];
                        particles.m_phases = new int[particles.m_maxParticlesCount];
                        particles.m_restParticles = new Particle[particles.m_maxParticlesCount];
                        particles.m_smoothedParticles = new Particle[particles.m_maxParticlesCount];
                        particles.m_particlesActivity = new bool[particles.m_maxParticlesCount];

                        int phase = Flex.MakePhase(particles.m_collisionGroup, (int)particles.m_interactionType);

                        for (int i = 0; i < particles.m_particlesCount; i++)
                        {
                            particles.m_velocities[i] = particles.m_initialVelocity;
                            particles.m_particlesActivity[i] = true;
                            particles.m_phases[i] = phase;

                            particles.m_restParticles[i] = particles.m_particles[i];
                            particles.m_smoothedParticles[i] = particles.m_particles[i];
                        }

                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                    if (GUILayout.Button("Add FlexRopeMesh"))
                    {

                        FlexRopeMesh frm = Selection.activeGameObject.AddComponent<FlexRopeMesh>();

                        frm.m_particles = particles;
                        frm.InitRope(particles.m_particlesCount);

                        MeshFilter meshFilter = Selection.activeGameObject.AddComponent<MeshFilter>();
                        meshFilter.sharedMesh = frm.m_mesh;

                        MeshRenderer rend = Selection.activeGameObject.AddComponent<MeshRenderer>();
                        Material mat = new Material(Shader.Find("Diffuse"));
                        mat.name = "RopeMat";
                        mat.color = Color.yellow;
                        rend.material = mat;

                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                    hasFlexComponent = true;
                }


                FlexSprings springs = Selection.activeGameObject.GetComponent<FlexSprings>();
                if (springs)
                {
                    EditorGUILayout.Separator();
                    GUILayout.Label("Springs", EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Long Range Attachments to Rope"))
                    {
                        int ropeSegments = particles.m_particlesCount - 1;
                        float restLength = Vector3.Distance(particles.m_particles[0].pos,  particles.m_particles[1].pos); 
                        //distance + bend + tether (long range attachments)
                        int springsCount = ropeSegments + (ropeSegments - 1) + ropeSegments;

                        springs.m_springsCount = springsCount;

                        Array.Resize<int>(ref springs.m_springIndices, springsCount*2);
                        Array.Resize<float>(ref springs.m_springCoefficients, springsCount);
                        Array.Resize<float>(ref springs.m_springRestLengths, springsCount);

                        for (int i = 0; i < ropeSegments; i++)
                        {
        
                            springs.m_springIndices[(ropeSegments + ropeSegments - 1) * 2 + i * 2 + 0] = 0;
                            springs.m_springIndices[(ropeSegments + ropeSegments - 1) * 2 + i * 2 + 1] = i+1;

                            //negative stiffness means tether constraints 
                            //(i.e. prevents spring stretch but not its compression)
                            springs.m_springCoefficients[(ropeSegments + ropeSegments - 1) + i] = -0.001f;
                            springs.m_springRestLengths[(ropeSegments + ropeSegments - 1) + i] = i * restLength;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }



                FlexShapeMatching shapes = Selection.activeGameObject.GetComponent<FlexShapeMatching>();
                if (shapes)
                {

                    EditorGUILayout.Separator();
                    GUILayout.Label("Shapes", EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();
                    m_clusterStiffness = EditorGUILayout.FloatField("Clusters Stiffness: ", m_clusterStiffness);
                    if (GUILayout.Button("Set"))
                    {

                        for (int i = 0; i < shapes.m_shapesCount; i++)
                        {
                            shapes.m_shapeCoefficients[i] = m_clusterStiffness;
                        }
                        Debug.Log("Clusters Ks Set To " + m_clusterStiffness);
                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }

                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.Separator();
                    GUILayout.Label("Skinning");
                    EditorGUILayout.BeginHorizontal();
                    m_skinFalloff = EditorGUILayout.FloatField("Falloff: ", m_skinFalloff);
                    m_skinMaxDist = EditorGUILayout.FloatField("Max Distance: ", m_skinMaxDist);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    inputMesh = EditorGUILayout.ObjectField(new GUIContent("Input Mesh", "The input mesh"), inputMesh, typeof(Mesh), true) as Mesh;

                    if (GUILayout.Button("Generate Skinning") && inputMesh)
                    {


                        Vector3[] vertices = inputMesh.vertices;

                        float[] skinWeights = new float[inputMesh.vertexCount * 4];
                        int[] skinIndices = new int[inputMesh.vertexCount * 4];
                        FlexExt.flexExtCreateSoftMeshSkinning(vertices, inputMesh.vertexCount, shapes.m_shapeCenters, shapes.m_shapesCount, m_skinFalloff, m_skinMaxDist, skinWeights, skinIndices);

                        Mesh mesh = new Mesh();
                        mesh.name = this.inputMesh.name + "FlexMesh";
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
                            bones[i].parent = Selection.activeGameObject.transform;
                            bones[i].localPosition = shapes.m_shapeCenters[i];
                            bones[i].localRotation = Quaternion.identity;

                            bindPoses[i] = bones[i].worldToLocalMatrix * Selection.activeGameObject.transform.localToWorldMatrix;
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

                        AssetDatabase.CreateAsset(mesh, "Assets/uFlex/Meshes/"+ this.inputMesh.name + "FlexMesh.asset");
                        AssetDatabase.SaveAssets();

                        FlexSkinnedMesh skin = Selection.activeGameObject.AddComponent<FlexSkinnedMesh>();
                        skin.m_bones = bones;
                        skin.m_boneWeights = boneWeights;
                        skin.m_bindPoses = bindPoses;

                        SkinnedMeshRenderer rend = Selection.activeGameObject.AddComponent<SkinnedMeshRenderer>();
                        rend.sharedMesh = mesh;
                      //  rend.sharedMesh = this.inputMesh;
                        rend.updateWhenOffscreen = true;
                        rend.quality = SkinQuality.Bone4;
                        rend.bones = bones;


                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                    EditorGUILayout.EndHorizontal();


                    if (GUILayout.Button("Upgrade to v0.5"))
                    {
                        int shapeIndex = 0;
                        int shapeIndexOffset = 0;

                        shapes.m_shapeTranslations = new Vector3[shapes.m_shapesCount];
                        shapes.m_shapeRotations = new Quaternion[shapes.m_shapesCount];
                        shapes.m_shapeRestPositions = new Vector3[shapes.m_shapeIndicesCount];

                        int shapeStart = 0;
                        for (int s = 0; s < shapes.m_shapesCount; s++)
                        {
                            shapes.m_shapeTranslations[s] = new Vector3();
                            shapes.m_shapeRotations[s] = Quaternion.identity;
                        
                        //    int indexOffset = shapeIndexOffset;

                            //m_shapeOffsets[shapeIndex + 1] = shapes.m_shapeOffsets[s] + indexOffset;
                            //m_shapeCoefficients[shapeIndex] = shapes.m_shapeCoefficients[s];
                            //m_shapeTranslations[shapeIndex] = shapes.m_shapeTranslations[s];
                            //m_shapeRotations[shapeIndex] = shapes.m_shapeRotations[s];

                            shapeIndex++;

                            int shapeEnd = shapes.m_shapeOffsets[s];

                            for (int i = shapeStart; i < shapeEnd; ++i)
                            {
                                int p = shapes.m_shapeIndices[i];

                                // remap indices and create local space positions for each shape
                                Vector3 pos = particles.m_particles[p].pos;
                                shapes.m_shapeRestPositions[shapeIndexOffset] = pos - shapes.m_shapeCenters[s];

                             //   m_shapeIndices[shapeIndexOffset] = shapes.m_shapeIndices[i] + particles.m_particlesIndex;

                                shapeIndexOffset++;
                            }

                            shapeStart = shapeEnd;
                        }

           

                        SceneView.RepaintAll();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                    hasFlexComponent = true;
                }

             
            }

            if(!hasFlexComponent)
            {
                GUILayout.Label("Select GameObject with Flex Components", EditorStyles.boldLabel);
            }
        }


        void OnSelectionChange()
        {
 
            if (Selection.activeGameObject)
            {
                MeshFilter mf = Selection.activeGameObject.GetComponent<MeshFilter>();
                if (mf)
                {
                    inputMesh = mf.sharedMesh;
                }
            }

            Repaint();
        }

        [MenuItem("Tools/uFlex Utils")]
        public static void ShowWindows()
        {

            FlexToolsWindow win = EditorWindow.GetWindow<FlexToolsWindow>(false, "uFlex Utils", true);
            if (Selection.activeGameObject)
            {
                MeshFilter mf = Selection.activeGameObject.GetComponent<MeshFilter>();
                if (mf)
                {
                    win.inputMesh = mf.sharedMesh;
                }
            }

        }



    }

}
