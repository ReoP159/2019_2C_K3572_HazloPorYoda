using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;




namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer m�s ejemplos chicos, en el caso de copiar para que se
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
        private TgcSkyBox skyBox;

        //-------variables adicionales para la nave

        /*private float side_acceleration = 5f;*/
        private float side_speed = 100F;

        private TGCVector3 forward_movement = TGCVector3.Empty;

        private float max_forward_speed = 500F;
        private float forward_acceleration = 80F;
        private float forward_speed = 0;
        private float break_constant = 3.5f; //Constante por la cual se multiplica para que frene m�s r�pido de lo que acelera

        //-----------------------------------------

        //variable para cargar la escena 
        private TgcScene scene;

        //variable para el boundingbox (caja de coliciones)
        private bool BoundingBox { get; set; }

        //variable para la camara;

        private CamaraNave camera;
        private int posicionCamara = 0;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, estructuras de optimizaci�n, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            //-------ESCENA--------//
            var loader = new TgcSceneLoader(); //clase para cargar el terreno
            var center = TGCVector3.Empty; //posicion inicial para la scene
            scene = loader.loadSceneFromFile(MediaDir + "Selva\\Selva-TgcScene.xml");

            //------SKYBOX------//
            skyBox = new TgcSkyBox();
            skyBox.Center = TGCVector3.Empty;
            skyBox.Size = new TGCVector3(10000, 10000, 10000);
            var pathSkybox = MediaDir + "SkyBox\\";

            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, pathSkybox + "Back.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, pathSkybox + "Front.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, pathSkybox + "Left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, pathSkybox + "Right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, pathSkybox + "Up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, pathSkybox + "Down.jpg");

            skyBox.Init();


            //-------NAVE--------//
            ship = loader.loadSceneFromFile(MediaDir + "StarWars-Speeder\\StarWars-Speeder-TgcScene.xml").Meshes[0];
            ship.Position = new TGCVector3(0, 600, 0);
            ship.Rotation = new TGCVector3(0, FastMath.PI/2, 0);


           
            //-------CAMARA--------//

            camera = new CamaraNave(ship.Position,0);
            Camara = camera;


        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la l�gica de computo del modelo, as� como tambi�n verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            var side_movement = TGCVector3.Empty;
            forward_movement = new TGCVector3(0,0,-1);


            //Capturar Input teclado
            ///Bounding-----------------
            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }
            ///Movimiento-------------------
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
            ///Aceleracion -----------------------
            if (Input.keyDown(Key.Space))
            {
                forward_speed = FastMath.Min(forward_speed + forward_acceleration * ElapsedTime,max_forward_speed);
                //forward_movement.Z = -1;
            }
            else
            {
                forward_speed = FastMath.Max(forward_speed-forward_acceleration*break_constant*ElapsedTime,0);
            }

            ///cambio posicion camara -----------------------
            if (Input.keyPressed(Key.C))
            {
                posicionCamara += 1;
                if(posicionCamara > 2)
                {
                    posicionCamara = 0;
                }
            }
            camera.CambiarPosicionCamara(posicionCamara);
            
            
            side_movement *= side_speed * ElapsedTime;
            
            forward_movement *= forward_speed * ElapsedTime;
            ship.Position += side_movement + forward_movement;
            
            ship.Transform = TGCMatrix.RotationYawPitchRoll(ship.Rotation.Y, ship.Rotation.X, ship.Rotation.Z) * TGCMatrix.Translation(ship.Position);

            camera.Target = ship.Position;
            skyBox.Center = Camara.Position;



            PostUpdate();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqu� todo el c�digo referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
        
            PreRender();


            //Dibuja un texto por pantalla
            DrawText.drawText("/--INTRUCCIONES------------",0,20,Color.LightYellow);
            DrawText.drawText("Con la tecla F se dibuja el bounding box.",0,35, Color.LightYellow);
            DrawText.drawText("Con la tecla C se cambia la posicion de la camara",0,50, Color.LightYellow);
            DrawText.drawText("/--DATOS-------------------", 0,80, Color.LightYellow);
            DrawText.drawText("Velocidad: " + forward_speed +"F",0,95, Color.LightYellow);
            DrawText.drawText("Posicion de la nave: X: " + ship.Position.X +
                                " Y: " + ship.Position.Y + " Z: " + ship.Position.Z, 0,110, Color.LightYellow);


            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jer�rquicos, tenemos control total.
            //Box.Transform = TGCMatrix.Scaling(Box.Scale) * TGCMatrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) * TGCMatrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aqu� solo nos hacia falta la traslaci�n.
            //Finalmente invocamos al render de la caja
            //Cuando tenemos modelos mesh podemos utilizar un m�todo que hace la matriz de transformaci�n est�ndar.
            //Es �til cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jer�rquicas o complicadas.
            //Mesh.UpdateMeshTransform();
            //Render del mesh
            //Mesh.Render();

            //pista.Render();
            skyBox.Render();
            ship.Render();
            scene.RenderAll();

            

            //Render de BoundingBox, muy �til para debug de colisiones.
            if (BoundingBox)
            {
                ship.BoundingBox.Render();
            }

            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecuci�n del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gr�ficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            ship.Dispose();
            scene.DisposeAll();
            skyBox.Dispose();
        }
    }
}