using UnityEngine;
using System;

namespace uFlex
{
    public class FlexParameters : MonoBehaviour
    {
        /// <summary>
        /// How the relaxation is applied inside the solver.
        /// </summary>
        [Tooltip("How the relaxation is applied inside the solver.")]
        public Flex.FlexRelaxationMode m_RelaxationMode = Flex.FlexRelaxationMode.Local;

        /// <summary>
        /// Control the convergence rate of the parallel solver, default: 1, values greater than 1 may lead to instability.
        /// </summary>
        [Tooltip("Control the convergence rate of the parallel solver, default: 1, values greater than 1 may lead to instability.")]
        public float m_relaxationFactor = 1.0f;

        /// <summary>
        /// Number of solver iterations to perform per-substep.
        /// </summary>
        [Tooltip("Number of solver iterations to perform per-substep.")]
        public int m_numIterations = 1;

        /// <summary>
        /// The maximum interaction radius for particles.
        /// </summary>
        [Tooltip("The maximum interaction radius for particles.")]
        public float m_radius = 0.5f;

        /// <summary>
        /// The distance non-fluid particles attempt to maintain from each other, must be in the range (0, radius].
        /// </summary>
        [Tooltip("The distance non-fluid particles attempt to maintain from each other, must be in the range (0, radius].")]
        public float m_solidRestDistance = 0.5f;

        /// <summary>
        /// The distance fluid particles are spaced at the rest density, must be in the range (0, radius], 
        /// for fluids this should generally be 50-70% of mRadius, for rigids this can simply be the same as the particle radius.
        /// </summary>
        [Tooltip("The distance fluid particles are spaced at the rest density, must be in the range (0, radius], for fluids this should generally be 50-70% of mRadius, for rigids this can simply be the same as the particle radius.")]
        public float m_fluidRestDistance = 0.25f;

        /// <summary>
        /// Coefficient of friction used when colliding against shapes.
        /// </summary>
        [Tooltip("	Coefficient of friction used when colliding against shapes.")]
        public float m_dynamicFriction = 0.1f;

        /// <summary>
        /// Coefficient of static friction used when colliding against shapes.
        /// </summary>
        [Tooltip("	Coefficient of static friction used when colliding against shapes.")]
        public float m_staticFriction = 0.0f;

        /// <summary>
        /// Coefficient of friction used when colliding particles.
        /// </summary>
        [Tooltip("Coefficient of friction used when colliding particles.")]
        public float m_particleFriction = 0.1f;

        /// <summary>
        /// Coefficient of restitution used when colliding against shapes, particle collisions are always inelastic.
        /// </summary>
        [Tooltip("Coefficient of restitution used when colliding against shapes, particle collisions are always inelastic.")]
        public float m_restitution = 0.0f;

        /// <summary>
        /// Controls how strongly particles stick to surfaces they hit, default 0.0, range [0.0, +inf].
        /// </summary>
        [Tooltip("Controls how strongly particles stick to surfaces they hit, default 0.0, range [0.0, +inf].")]
        public float m_adhesion = 0.0f;

        /// <summary>
        /// Particles belonging to rigid shapes that move with a position delta magnitude > threshold will be permanently deformed in the rest pose.
        /// </summary>
        [Tooltip("Particles belonging to rigid shapes that move with a position delta magnitude > threshold will be permanently deformed in the rest pose.")]
        public float m_plasticThreshold;

        /// <summary>
        /// Controls the rate at which particles in the rest pose are deformed for particles passing the deformation threshold.
        /// </summary>
        [Tooltip("	Controls the rate at which particles in the rest pose are deformed for particles passing the deformation threshold.")]
        public float m_plasticCreep;

        /// <summary>
        /// Distance particles maintain against shapes, note that for robust collision against triangle meshes this distance should be greater than zero.
        /// </summary>
        [Tooltip("Distance particles maintain against shapes, note that for robust collision against triangle meshes this distance should be greater than zero.")]
        public float m_CollisionDistance;

        /// <summary>
        /// Increases the radius used during neighbor finding, this is useful if particles are expected to move significantly during a single step to ensure contacts aren't missed on subsequent iterations.
        /// </summary>
        [Tooltip("Increases the radius used during neighbor finding, this is useful if particles are expected to move significantly during a single step to ensure contacts aren't missed on subsequent iterations.")]
        public float m_ParticleCollisionMargin;

        /// <summary>
        /// Increases the radius used during contact finding against kinematic shapes.
        /// </summary>
        [Tooltip("Increases the radius used during contact finding against kinematic shapes.")]
        public float m_ShapeCollisionMargin;

        /// <summary>
        /// Particles with a velocity magnitude < this threshold will be considered fixed.
        /// </summary>
        [Tooltip("Particles with a velocity magnitude < this threshold will be considered fixed.")]
        public float m_sleepThreshold = 0.0f;

        /// <summary>
        /// The magnitude of particle velocity will be clamped to this value at the end of each step.
        /// </summary>
        [Tooltip("The magnitude of particle velocity will be clamped to this value at the end of each step.")]
        public float m_MaxSpeed = 1000;

        /// <summary>
        /// Artificially decrease the mass of particles based on height from a fixed reference point, 
        /// this makes stacks and piles converge faster.
        /// </summary>
        [Tooltip("Artificially decrease the mass of particles based on height from a fixed reference point, this makes stacks and piles converge faster.")]
        public float m_shockPropagation = 0.0f;

        /// <summary>
        /// Damps particle velocity based on how many particle contacts it has.
        /// </summary>
        [Tooltip("Damps particle velocity based on how many particle contacts it has.")]
        public float m_dissipation = 0.0f;

        /// <summary>
        /// Viscous drag force, applies a force proportional, and opposite to the particle velocity.
        /// </summary>
        [Tooltip("Viscous drag force, applies a force proportional, and opposite to the particle velocity.")]
        public float m_damping = 0.0f;

        /// <summary>
        /// Increase the amount of inertia in shape-matching clusters to improve stability.
        /// </summary>
        [Tooltip("Increase the amount of inertia in shape-matching clusters to improve stability.")]
        public float m_inertiaBias = 0.001f;

        /// <summary>
        /// If true then particles with phase 0 are considered fluid particles and interact using the position based fluids method.
        /// </summary>
        [Tooltip("If true then particles with phase 0 are considered fluid particles and interact using the position based fluids method.")]
        public bool m_Fluid = false;

        /// <summary>
        /// Control how strongly particles hold each other together, default: 0.025, range [0.0, +inf].
        /// </summary>
        [Tooltip("Control how strongly particles hold each other together, default: 0.025, range [0.0, +inf].")]
        public float m_cohesion = 0.025f;

        /// <summary>
        /// Controls how strongly particles attempt to minimize surface area, default: 0.0, range: [0.0, +inf].
        /// </summary>
        [Tooltip("Controls how strongly particles attempt to minimize surface area, default: 0.0, range: [0.0, +inf].")]
        public float m_surfaceTension = 0.0f;

        /// <summary>
        /// Smoothes particle velocities using XSPH viscosity.
        /// </summary>
        [Tooltip("Smoothes particle velocities using XSPH viscosity.")]
        public float m_viscosity = 0.0f;

        /// <summary>
        /// Increases vorticity by applying rotational forces to particles.
        /// </summary>
        [Tooltip("Increases vorticity by applying rotational forces to particles.")]
        public float m_vorticityConfinement = 0.0f;

        /// <summary>
        /// Add pressure from solid surfaces to particles.
        /// </summary>
        [Tooltip("Add pressure from solid surfaces to particles.")]
        public float m_solidPressure = 1.0f;

        /// <summary>
        /// Drag force applied to boundary fluid particles.
        /// </summary>
        [Tooltip("Drag force applied to boundary fluid particles.")]
        public float m_freeSurfaceDrag = 0.0f;

        /// <summary>
        /// Gravity is scaled by this value for fluid particles.
        /// </summary>
        [Tooltip("Gravity is scaled by this value for fluid particles.")]
        public float m_buoyancy = 1.0f;

        /// <summary>
        /// Constant acceleration applied to particles that belong to dynamic triangles, drag needs to be > 0 for wind to affect triangles.
        /// </summary>
        [Tooltip("Constant acceleration applied to particles that belong to dynamic triangles, drag needs to be > 0 for wind to affect triangles.")]
        public Vector3 m_Wind = new Vector3(0, 0, 0);

        /// <summary>
        /// Drag force applied to particles belonging to dynamic triangles, proportional to velocity^2*area in the negative velocity direction.
        /// </summary>
        [Tooltip("Drag force applied to particles belonging to dynamic triangles, proportional to velocity^2*area in the negative velocity direction.")]
        public float m_Drag = 0.0f;

        /// <summary>
        /// Lift force applied to particles belonging to dynamic triangles, proportional to velocity^2*area in the direction perpendicular to velocity and (if possible), parallel to the plane normal.
        /// </summary>
        [Tooltip("Lift force applied to particles belonging to dynamic triangles, proportional to velocity^2*area in the direction perpendicular to velocity and (if possible), parallel to the plane normal.")]
        public float m_Lift = 0.0f;

        /// <summary>
        /// Control the strength of Laplacian smoothing in particles for rendering, if zero then smoothed positions will not be calculated, see flexGetSmoothParticles()
        /// </summary>
        [Tooltip("Control the strength of Laplacian smoothing in particles for rendering, if zero then smoothed positions will not be calculated, see flexGetSmoothParticles()")]
        public float m_Smoothing = 1.0f;

        /// <summary>
        /// Control how much anisotropy is present in resulting ellipsoids for rendering, if zero then anisotropy will not be calculated, see flexGetAnisotropy()
        /// </summary>
        [Tooltip("Control how much anisotropy is present in resulting ellipsoids for rendering, if zero then anisotropy will not be calculated, see flexGetAnisotropy()")]
        [HideInInspector]
        public float m_AnisotropyScale = 1.0f;

        /// <summary>
        /// Clamp the anisotropy scale to this fraction of the radius.
        /// </summary>
        [Tooltip("Clamp the anisotropy scale to this fraction of the radius.")]
        [HideInInspector]
        public float m_AnisotropyMin = 0.1f;

        /// <summary>
        /// Clamp the anisotropy scale to this fraction of the radius.
        /// </summary>
        [Tooltip("Clamp the anisotropy scale to this fraction of the radius.")]
        [HideInInspector]
        public float m_AnisotropyMax = 2.0f;

        /// <summary>
        /// Particles with kinetic energy + divergence above this threshold will spawn new diffuse particles.
        /// </summary>
        [Tooltip("Particles with kinetic energy + divergence above this threshold will spawn new diffuse particles.")]
    //    [HideInInspector]
        public float m_DiffuseThreshold = 100.0f;

        /// <summary>
        /// Scales force opposing gravity that diffuse particles receive.
        /// </summary>
        [Tooltip("Scales force opposing gravity that diffuse particles receive.")]
      //  [HideInInspector]
        public float m_DiffuseBuoyancy = 0.0f;

        /// <summary>
        /// Scales force diffuse particles receive in direction of neighbor fluid particles.
        /// </summary>
        [Tooltip("Scales force diffuse particles receive in direction of neighbor fluid particles.")]
      //  [HideInInspector]
        public float m_DiffuseDrag = 0.0f;

        /// <summary>
        /// The number of neighbors below which a diffuse particle is considered ballistic.
        /// </summary>
        [Tooltip("The number of neighbors below which a diffuse particle is considered ballistic.")]
      //  [HideInInspector]
        public int m_DiffuseBallistic = 16;

        /// <summary>
        /// Diffuse particles will be sorted by depth along this axis if non-zero.
        /// </summary>
        [Tooltip("Diffuse particles will be sorted by depth along this axis if non-zero.")]
    //    [HideInInspector]
        public Vector3 m_DiffuseSortAxis = new Vector3(0, 0, 0);

        /// <summary>
        /// Time in seconds that a diffuse particle will live for after being spawned, particles will be spawned with a random lifetime in the range [0, mDiffuseLifetime].
        /// </summary>
        [Tooltip("Time in seconds that a diffuse particle will live for after being spawned, particles will be spawned with a random lifetime in the range [0, mDiffuseLifetime].")]
     //   [HideInInspector]
        public float m_DiffuseLifetime = 2.0f;

        /// <summary>
        /// Num collision planes.
        /// </summary>
        [Tooltip("Num collision planes.")]
        public int m_NumPlanes = 1;

        /// <summary>
        /// Collision planes in the form ax + by + cz + d = 0.
        /// float[8][4]
        /// </summary>
        [Tooltip("Collision planes in the form ax + by + cz + d = 0. float[8][4]")]
        public float[] m_Planes = new float[] { 0, 1, 0, 0 };

        /// <summary>
        /// Constant acceleration applied to all particles.
        /// </summary>
        [Tooltip("Constant acceleration applied to all particles.")]
        public Vector3 m_gravity = new Vector3(0, -9.81f, 0);


        // Use this for initialization
        void Start()
        {
            ClampParams();
        }

        // Update is called once per frame
        void Update()
        {
            ClampParams();
        }

        /// <summary>
        /// Clamp parameters to valid ranges
        /// </summary>
        private void ClampParams()
        {
            m_relaxationFactor = Mathf.Clamp(m_relaxationFactor, 0.0f, 2.0f);
            m_numIterations = Mathf.Clamp( m_numIterations, 1, 100);

            m_solidRestDistance = Mathf.Clamp(m_solidRestDistance, 0.0f, m_radius);
            m_fluidRestDistance = Mathf.Clamp(m_fluidRestDistance, 0.0f, m_radius);
        
            m_staticFriction = Mathf.Clamp(m_staticFriction, 0.0f, 1.0f);
            m_dynamicFriction = Mathf.Clamp(m_dynamicFriction, 0.0f, 1.0f);
            m_particleFriction = Mathf.Clamp(m_particleFriction, 0.0f, 1.0f);

            m_restitution = Mathf.Clamp(m_restitution, 0.0f, 1.0f);
            m_sleepThreshold = Mathf.Clamp(m_sleepThreshold, 0.0f, 1.0f);
            m_shockPropagation = Mathf.Clamp(m_shockPropagation, 0.0f, 100.0f);
            m_damping = Mathf.Clamp(m_damping, 0.0f, 100.0f);
            m_dissipation = Mathf.Clamp(m_dissipation, 0.0f, 1.0f);

            m_adhesion = Mathf.Clamp(m_adhesion, 0.0f, float.MaxValue);
            m_cohesion = Mathf.Clamp(m_cohesion, 0.0f, float.MaxValue);
            m_surfaceTension = Mathf.Clamp(m_surfaceTension, 0.0f, float.MaxValue);
            m_viscosity = Mathf.Clamp(m_viscosity, 0.0f, float.MaxValue);
            m_vorticityConfinement = Mathf.Clamp(m_vorticityConfinement, 0.0f, float.MaxValue);
            m_solidPressure = Mathf.Clamp(m_solidPressure, 0.0f, 1.0f);
            m_freeSurfaceDrag = Mathf.Clamp(m_freeSurfaceDrag, 0.0f, 1.0f);
            m_buoyancy = Mathf.Clamp(m_buoyancy, 0.0f, 1.0f);

            m_plasticCreep = Mathf.Clamp(m_plasticCreep, 0.0f, 1.0f);
            m_plasticThreshold = Mathf.Clamp(m_plasticThreshold, 0.0f, 1.0f);

            m_inertiaBias = Mathf.Clamp(m_inertiaBias, 0.0f, 1.0f);

        }

        /// <summary>
        /// Get the FlexAPI compatible structure with params
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Flex.Params GetParams(ref Flex.Params p)
        {
            p.mGravity = new float[3];
            p.mGravity[0] = m_gravity.x;
            p.mGravity[1] = m_gravity.y;
            p.mGravity[2] = m_gravity.z;

            p.mWind = new float[3];
            p.mWind[0] = m_Wind.x;
            p.mWind[1] = m_Wind.y;
            p.mWind[2] = m_Wind.z;

            p.mRadius = m_radius;
            p.mViscosity = m_viscosity;
            p.mDynamicFriction = m_dynamicFriction;
            p.mStaticFriction = m_staticFriction;
            p.mParticleFriction = m_particleFriction; // scale friction between particles by default
            p.mFreeSurfaceDrag = m_freeSurfaceDrag;
            p.mDrag = m_Drag;
            p.mLift = m_Lift;
            p.mNumIterations = m_numIterations;
            p.mFluidRestDistance = m_fluidRestDistance;
            p.mSolidRestDistance = m_solidRestDistance;

            p.mAnisotropyScale = m_AnisotropyScale;
            p.mAnisotropyMin = m_AnisotropyMin;
            p.mAnisotropyMax = m_AnisotropyMax;
            p.mSmoothing = m_Smoothing;

            p.mDissipation = m_dissipation;
            p.mDamping = m_damping;
            p.mParticleCollisionMargin = m_ParticleCollisionMargin;
            p.mShapeCollisionMargin = m_ShapeCollisionMargin;
            p.mCollisionDistance = m_CollisionDistance;
            p.mPlasticThreshold = m_plasticThreshold;
            p.mPlasticCreep = m_plasticCreep;
            p.mFluid = m_Fluid;
            p.mSleepThreshold = m_sleepThreshold;
            p.mShockPropagation = m_shockPropagation;
            p.mRestitution = m_restitution;
            p.mMaxSpeed = m_MaxSpeed;
            p.mRelaxationMode = m_RelaxationMode;
            p.mRelaxationFactor = m_relaxationFactor;
            p.mSolidPressure = m_solidPressure;
            p.mAdhesion = m_adhesion;
            p.mCohesion = m_cohesion;
            p.mSurfaceTension = m_surfaceTension;
            p.mVorticityConfinement = m_vorticityConfinement;
            p.mBuoyancy = m_buoyancy;
            p.mDiffuseThreshold = m_DiffuseThreshold;
            p.mDiffuseBuoyancy = m_DiffuseBuoyancy;
            p.mDiffuseDrag = m_DiffuseDrag;
            p.mDiffuseBallistic = m_DiffuseBallistic;

            p.mDiffuseSortAxis = new float[3];
            p.mDiffuseSortAxis[0] = m_DiffuseSortAxis.x;
            p.mDiffuseSortAxis[1] = m_DiffuseSortAxis.y;
            p.mDiffuseSortAxis[2] = m_DiffuseSortAxis.z;

            p.mNumPlanes = m_NumPlanes;
            p.mPlanes = new float[m_NumPlanes * 4];
            Array.Copy(m_Planes, p.mPlanes, m_NumPlanes * 4);

            p.mDiffuseLifetime = m_DiffuseLifetime;
            p.mInertiaBias = m_inertiaBias;
            return p;
        }
    }
}