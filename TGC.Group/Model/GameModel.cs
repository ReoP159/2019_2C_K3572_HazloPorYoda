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
        //constructor de la aplicacion
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = "Naves";
            Name = "Hazlo por Yoda";
            Description = "Juego de Naves";
        }

        //variable para cargar la nave
        private TgcMesh ship;

        //-------variables adicionales para la nave

        /*private float side_acceleration = 5f;*/
        private float side_speed = 70F;

        private TGCVector3 forward_movement = TGCVector3.Empty;

        private float max_forward_speed = 200F;
        private float forward_acceleration = 80F;
        private float forward_speed = 0;
        private float break_constant = 3f; //Constante por la cual se multiplica para que frene más rápido de lo que acelera

        //-----------------------------------------

        //variable para cargar la escena 
        private TgcScene scene;

        //variable para el boundingbox (caja de coliciones)
        private bool BoundingBox { get; set; }

        //private Track pista;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            //-------ESCENA--------//
            var loader = new TgcSceneLoader(); //clase para cargar el terreno
            var center = TGCVector3.Empty; //posicion inicial para la scene
            scene = loader.loadSceneFromFile(MediaDir + "Selva\\Selva-TgcScene.xml");


            //-------NAVE--------//
            ship = loader.loadSceneFromFile(MediaDir + "StarWars-Speeder\\StarWars-Speeder-TgcScene.xml").Meshes[0];
            ship.Rotation = new TGCVector3(0, FastMath.PI_HALF, 0);
            ship.Position = new TGCVector3(0, 0, 0);
            ship.Transform = TGCMatrix.Scaling(TGCVector3.One) * TGCMatrix.RotationYawPitchRoll(ship.Rotation.Y, ship.Rotation.X, ship.Rotation.Z) * TGCMatrix.Translation(ship.Position);

            var pathTextura = MediaDir + "StarWars-ATAT\\Textures\\BlackMetalTexture.jpg";

            var texture = TgcTexture.createTexture(pathTextura);
            //this.pista = new Track(center, pathTextura,4);

            ship.Scale = new TGCVector3(0.7f, 0.7f, 0.7f);

            //-------CAMARA--------//
            /**var cameraPosition = new TGCVector3(0, 0, 125);
            var lookAt = TGCVector3.Empty;
            Camara.SetCamera(cameraPosition, lookAt);*/
            Camara = new TgcRotationalCamera(ship.BoundingBox.calculateBoxCenter(), ship.BoundingBox.calculateBoxRadius() * 6, Input);
            
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
            ship.Position += side_movement + forward_movement;
            ship.Transform = TGCMatrix.RotationYawPitchRoll(ship.Rotation.Y, ship.Rotation.X, ship.Rotation.Z) * TGCMatrix.Translation(ship.Position);

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
        
            PreRender();

            
            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("Velocidad: " + forward_speed +"F", 0, 40, Color.OrangeRed);
            DrawText.drawText("Posicion de la nave: " + ship.Position, 0, 60, Color.OrangeRed);

            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            //Box.Transform = TGCMatrix.Scaling(Box.Scale) * TGCMatrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) * TGCMatrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            //Mesh.UpdateMeshTransform();
            //Render del mesh
            //Mesh.Render();

            //pista.Render();
            ship.Render();
            scene.RenderAll();

            

            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                ship.BoundingBox.Render();
            }

            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            ship.Dispose();
            scene.DisposeAll();
        }
    }
}