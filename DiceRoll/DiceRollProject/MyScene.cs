#region Using Statements

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using StillDesign.PhysX;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Materials;
using Material = WaveEngine.Framework.Graphics.Material;
using Random = System.Random;
using Scene = WaveEngine.Framework.Scene;

#endregion

namespace DiceRollProject
{
    public class MyScene : Scene
    {
        //Instantiate the dice
        private Entity die;
        private Entity die2;
        //Instantiate the table
        //Instantiate the materials
        private Material dieMaterial;
        private Model dieModel;
        //Instantiate The Cameras
        private CameraPoint point1;
        private CameraPoint point2;
        private CameraPoint point3;
        private CameraPoint point4;
        private Entity table;
        private Material tableMaterial;
        private Input input;
        private Physic3DCollisionEventArgs physicsArgs;

        protected override void CreateScene()
        {
            Physic3DCollisionGroup DiceCollisionGroup = new Physic3DCollisionGroup();
            Physic3DCollisionGroup TableCollisionGroup = new Physic3DCollisionGroup();
            TableCollisionGroup.DefineCollisionWith(DiceCollisionGroup);
            DiceCollisionGroup.DefineCollisionWith(TableCollisionGroup);

            RenderManager.DebugLines = false;

            physicsArgs = new Physic3DCollisionEventArgs();

            RenderManager.BackgroundColor = Color.CornflowerBlue;
            CameraPoint point1 = new CameraPoint();
            CameraPoint point2 = new CameraPoint();
            CameraPoint point3 = new CameraPoint();
            CameraPoint point4 = new CameraPoint();

            Entity ground = this.CreateGround();
            this.Createwalls();
            ground.FindComponent<RigidBody3D>().CollisionGroup = TableCollisionGroup;


            //var AnimationCamera = new PathCamera("AnimationCamera", new Vector3(0, 50, 80), Vector3.Zero, );
            //var beh = new PathCameraBehavior(new List<point1,point2,point3,point4>());

            //EntityManager.Add(AnimationCamera);
            PhysicsManager.Gravity3D = new Vector3(0, -9.8f, 0);

            Entity Die1 = new Entity("Die1")
                .AddComponent(new Transform3D())
                .AddComponent(new RigidBody3D()
                {
                    Restitution = GenRand(0.4, 0.7),
                    AngularVelocity = new Vector3(2.5f, 0, 0)
                })
                .AddComponent(new MeshCollider())
                .AddComponent(new Model("Content/Dice_LowPoly_OBJ.wpk"))
                .AddComponent(
                    new MaterialsMap(new BasicMaterial("Content/Red_Diffuse.wpk")
                    {
                        ReferenceAlpha = 0.5f
                    }))
                //.AddComponent(new MaterialsMap(new BasicMaterial(Color.Black)))
                .AddComponent(new ModelRenderer());
            float scale = .005f;
            Die1.FindComponent<Transform3D>().Scale.X = scale;
            Die1.FindComponent<Transform3D>().Scale.Y = scale;
            Die1.FindComponent<Transform3D>().Scale.Z = scale;
            Die1.FindComponent<Transform3D>().Position.Y = 2f;
            Die1.FindComponent<Transform3D>().Position.X = 0f;
            Die1.FindComponent<RigidBody3D>().CollisionGroup = DiceCollisionGroup;
            //Die1.FindComponent<RigidBody3D>().EnableContinuousContact = true;
            Die1.FindComponent<RigidBody3D>().LinearVelocity = new Vector3(Getrandom(), 0f, -3f);
            Die1.FindComponent<MeshCollider>().IsActive = true;
            Console.WriteLine(Die1.FindComponent<RigidBody3D>().Restitution);
            EntityManager.Add(Die1);
            Thread.Sleep(new TimeSpan(1000));

            Entity Die2 = new Entity("Die2")
                .AddComponent(new Transform3D())
                .AddComponent(new RigidBody3D() {Restitution = GenRand(0.4, 0.7)})
                .AddComponent(new MeshCollider())
                .AddComponent(new Model("Content/Dice_LowPoly_OBJ.wpk"))
                .AddComponent(
                    new MaterialsMap(new BasicMaterial("Content/White_Diffuse.wpk")
                    {
                        ReferenceAlpha = 0.5f
                    }))
                //.AddComponent(new MaterialsMap(new BasicMaterial(Color.Black)))
                .AddComponent(new ModelRenderer());
            Die2.FindComponent<Transform3D>().Scale.X = scale;
            Die2.FindComponent<Transform3D>().Scale.Y = scale;
            Die2.FindComponent<Transform3D>().Scale.Z = scale;
            Die2.FindComponent<Transform3D>().Position.Y = 2f;
            Die2.FindComponent<Transform3D>().Position.X = 1f;
            Die2.FindComponent<RigidBody3D>().LinearVelocity = new Vector3(Getrandom(), 0f, -3f);
            Die2.FindComponent<RigidBody3D>().CollisionGroup = DiceCollisionGroup;
            //Die1.FindComponent<RigidBody3D>().EnableContinuousContact = true;
            Die2.FindComponent<MeshCollider>().IsActive = true;
            Console.WriteLine(Die2.FindComponent<RigidBody3D>().Restitution);
            EntityManager.Add(Die2);

            //var mainCamera = new ViewCamera("MainCamera", new Vector3(0, 3, 0),
            //    new Vector3(Die1.FindComponent<Transform3D>().Position.X,
            //        Die1.FindComponent<Transform3D>().Position.Y, Die1.FindComponent<Transform3D>().Position.Z)); //xyz
            //EntityManager.Add(mainCamera);
            //RenderManager.SetActiveCamera(mainCamera.Entity);

            var mainCamera = new FreeCamera("MainCamera", new Vector3(-10, 3, 0), Vector3.Zero);
            // new Vector3(Die1.FindComponent<Transform3D>().Position.X,
            //Die1.FindComponent<Transform3D>().Position.Y, Die1.FindComponent<Transform3D>().Position.Z));
            EntityManager.Add(mainCamera);
            RenderManager.SetActiveCamera(mainCamera.Entity);

            //mainCamera.Entity.AddComponent(new Cam(Die1));

            this.AddSceneBehavior(new sb(), sb.Order.PostUpdate);
        }

        private void Createwalls()
        {
            Entity VertWall1 = new Entity("VertWall1")
                .AddComponent(new Transform3D() {Position = new Vector3(10, 4f, 0), Scale = new Vector3(1, 10, 20)})
                .AddComponent(new BoxCollider())
                .AddComponent(Model.CreateCube())
                .AddComponent(new RigidBody3D() {IsKinematic = true})
                .AddComponent(new MaterialsMap(new BasicMaterial("Content/wood.wpk")))
                .AddComponent(new ModelRenderer());
            EntityManager.Add(VertWall1);

            Entity VertWall2 = new Entity("VertWall2")
                .AddComponent(new Transform3D() {Position = new Vector3(-10, 4, 0), Scale = new Vector3(1, 10, 20)})
                .AddComponent(new BoxCollider())
                .AddComponent(Model.CreateCube())
                .AddComponent(new RigidBody3D() {IsKinematic = true})
                .AddComponent(new MaterialsMap(new BasicMaterial("Content/wood.wpk")))
                .AddComponent(new ModelRenderer());
            EntityManager.Add(VertWall2);

            Entity vertWall3 = new Entity("VertWall3")
                .AddComponent(new Transform3D() {Position = new Vector3(0, 4f, 10), Scale = new Vector3(20, 10, 1)})
                .AddComponent(new BoxCollider())
                .AddComponent(Model.CreateCube())
                .AddComponent(new RigidBody3D() {IsKinematic = true})
                .AddComponent(new MaterialsMap(new BasicMaterial("Content/wood.wpk")))
                .AddComponent(new ModelRenderer());
            EntityManager.Add(vertWall3);

            Entity vertWall4 = new Entity("VertWall4")
                .AddComponent(new Transform3D() {Position = new Vector3(0, 4, -10), Scale = new Vector3(20, 10, 1)})
                .AddComponent(new BoxCollider())
                .AddComponent(Model.CreateCube())
                .AddComponent(new RigidBody3D() {IsKinematic = true})
                .AddComponent(new MaterialsMap(new BasicMaterial("Content/wood.wpk")))
                .AddComponent(new ModelRenderer());
            EntityManager.Add(vertWall4);
        }

        private Entity CreateGround()
        {
            Entity ground = new Entity("ground")
                .AddComponent(new Transform3D() {Position = new Vector3(0, -1, 0), Scale = new Vector3(20, 1, 20)})
                .AddComponent(new BoxCollider())
                .AddComponent(Model.CreateCube())
                .AddComponent(new RigidBody3D() {IsKinematic = true, Restitution = 0f})
                .AddComponent(new MaterialsMap(new BasicMaterial("Content/wood.wpk")))
                .AddComponent(new ModelRenderer());

            EntityManager.Add(ground);

            return ground;
        }

        private static readonly Random random = new Random();

        private static float GenRand(double one, double two)
        {
            Random rand = new Random();
            return (float) (one + rand.NextDouble()*(two - one));
        }

        private float Getrandom()
        {
            Random random = new Random();
            return random.Next(0, 30);
        }

        private void rigidBody3D_OnPhysic3DCollision(object sender, Physic3DCollisionEventArgs args)
        {
            Console.WriteLine("collision");
        }
    }

    public class sb : SceneBehavior
    {
        private KeyboardState state;
        private MouseState mouse;

        protected override void Update(TimeSpan gameTime)
        {
            if (mouse.LeftButton == ButtonState.Pressed)
                Console.WriteLine("ispressed");
        }

        protected override void ResolveDependencies()
        {
            //throw new NotImplementedException();
        }
    }

    public class Cam : ViewCameraBehavior
    {
        private MyScene scene;
        private Entity Die1;
        private KeyboardState state;

        public Cam(Entity Die1)
        {
            this.Die1 = Die1;
            this.Initialize();
        }

        protected override void Initialize()
        {
            MyScene scene = new MyScene();
        }

        protected override void Update(TimeSpan gameTime)
        {
            Camera.LookAt.Y = this.Die1.FindComponent<Transform3D>().Position.Y;
            Camera.LookAt.X = this.Die1.FindComponent<Transform3D>().Position.X;
            Camera.LookAt.Z = this.Die1.FindComponent<Transform3D>().Position.Z;
            if (this.Die1.FindComponent<Transform3D>().Position.X - Camera.Position.X > 5)
            {
                Camera.Position.X = Camera.Position.X + 0.1f;
            }
            Console.WriteLine("X:" + Camera.Position.X + ". Y:" + Camera.Position.Y + ". Z:" + Camera.Position.Z);
        }
    }
}