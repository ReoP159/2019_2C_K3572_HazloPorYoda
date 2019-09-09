using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Collections.Generic;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Camara;


namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        //Caja que se muestra en el ejemplo.
        private TGCBox Box1 { get; set; }
        private TGCBox Box2 { get; set; }
        private TGCBox Box3 { get; set; }

        private class Track
        {
            private List<TgcPlane> walls_l;
            private List<TgcPlane> walls_r;
            private List<TgcPlane> floor;
            public int repeat;
            public TGCVector3 center;

            public Track(TGCVector3 centro, string pathTextura, int repeticiones)
            {
                repeat = repeticiones;
                center = centro;

                var size_lados = new TGCVector3(0, 160, 160);
                var size_suelo = new TGCVector3(100, 0, 160);

                var texture = TgcTexture.createTexture(pathTextura);

                walls_l = new List<TgcPlane>();
                walls_r = new List<TgcPlane>();
                floor = new List<TgcPlane>();

                for(int i = 0; i < repeticiones; i++)
                {
                    walls_l.Add(new TgcPlane(center + new TGCVector3(-50, -80, -80-(i*160)), size_lados, TgcPlane.Orientations.YZplane, texture));
                    walls_r.Add(new TgcPlane(center + new TGCVector3(50, -80, -80-(i * 160)), size_lados, TgcPlane.Orientations.YZplane, texture));
                    floor.Add(new TgcPlane(center + new TGCVector3(-50, -80, -80-(i * 160)), size_suelo, TgcPlane.Orientations.XZplane, texture));
                }
            }

            public void Render()
            {
                foreach (var plane in walls_l) { plane.Render();}
                foreach (var plane in walls_r) { plane.Render(); }
                foreach (var plane in floor) { plane.Render(); }
            }
            public void Dispose()
            {
                foreach (var plane in walls_l) { plane.Dispose(); }
                foreach (var plane in walls_r) { plane.Dispose(); }
                foreach (var plane in floor) { plane.Dispose(); }
            }

        }

        //Mesh de TgcLogo.
        private TgcMesh Mesh { get; set; }

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        //private TgcScene scene;

        private Track pista;

        private TgcMesh ship;
        //private float side_acceleration = 5f;
        private float side_speed = 70F;

        private TGCVector3 forward_movement = TGCVector3.Empty;

        private float max_forward_speed = 200F;
        private float forward_acceleration = 80F;
        private float forward_speed = 0;
        private float break_constant = 3f; //Constante por la cual se multiplica para que frene más rápido de lo que acelera


        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {

            var loader = new TgcSceneLoader();
            var center = TGCVector3.Empty;

            ship = loader.loadSceneFromFile(MediaDir + "StarWars-YWing\\StarWars-YWing-TgcScene.xml").Meshes[0];
            ship.Rotation += new TGCVector3(0, FastMath.PI_HALF, 0);
            ship.Position = new TGCVector3(0, 0, 0);
            ship.Transform = TGCMatrix.Scaling(TGCVector3.One) * TGCMatrix.RotationYawPitchRoll(ship.Rotation.Y, ship.Rotation.X, ship.Rotation.Z) * TGCMatrix.Translation(ship.Position);


            var pathTextura = MediaDir + "StarWars-ATAT\\Textures\\BlackMetalTexture.jpg";

            var texture = TgcTexture.createTexture(pathTextura);
            pista = new Track(center, pathTextura,4);



            ship.Scale = new TGCVector3(0.7f, 0.7f, 0.7f);

            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.
            var cameraPosition = new TGCVector3(0, 0, 125);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = TGCVector3.Empty;
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara.SetCamera(cameraPosition, lookAt);
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.

        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            var side_movement = TGCVector3.Empty;
            forward_movement = new TGCVector3(0,0,-1);


            //Capturar Input teclado

            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }

            if (Input.keyDown(Key.S))
            {
                side_movement.Y = -1;
            }

            if (Input.keyDown(Key.W))
            {
                side_movement.Y = 1;
            }

            if (Input.keyDown(Key.D))
            {
                side_movement.X = -1;
            }

            if (Input.keyDown(Key.A))
            {
                side_movement.X = 1;
            }

            if (Input.keyDown(Key.Space))
            {
                forward_speed = FastMath.Min(forward_speed + forward_acceleration * ElapsedTime,max_forward_speed);
                //forward_movement.Z = -1;
            }
            else
            {
                forward_speed = FastMath.Max(forward_speed-forward_acceleration*break_constant*ElapsedTime,0);
            }

            side_movement *= side_speed * ElapsedTime;
            forward_movement *= forward_speed * ElapsedTime;
            ship.Move(side_movement+forward_movement);

            //Para seguir a la nave
            var dis_camara_nave = new TGCVector3(0, 10, 125);
            Camara.SetCamera(ship.Position + dis_camara_nave, ship.Position);
            

            PostUpdate();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            ship.Render();
            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(Camara.Position), 0, 30, Color.OrangeRed);
            //DrawText.drawText("ElapsetTime: " + ElapsedTime,0,40, Color.OrangeRed);
            DrawText.drawText("Velocidad: " + forward_speed +"F", 0, 40, Color.OrangeRed);

            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            //Box.Transform = TGCMatrix.Scaling(Box.Scale) * TGCMatrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) * TGCMatrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja

            pista.Render();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            //Mesh.UpdateMeshTransform();
            //Render del mesh
            //Mesh.Render();

            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                Box1.BoundingBox.Render();
                ship.BoundingBox.Render();
            }

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {

            //Dispose del mesh.
            ship.Dispose();
            pista.Dispose();
        }
    }
}