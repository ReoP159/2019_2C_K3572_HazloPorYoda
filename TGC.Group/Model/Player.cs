using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Player
    {
        private TgcSceneLoader loader;
        private String mediaDir;

        private TgcMesh ship;

        public Player(TgcSceneLoader loader, String mediaDir)
        {
            this.loader = loader;
            this.mediaDir = mediaDir;

            ship = loader.loadSceneFromFile(mediaDir + "StarWars-Speeder\\StarWars-Speeder-TgcScene.xml").Meshes[0];
            ship.Position = new TGCVector3(0, 0, 0);
            ship.Rotation = new TGCVector3(0, FastMath.PI / 2, 0);
            ship.Transform = TGCMatrix.Scaling(TGCVector3.One * 0.5f) * TGCMatrix.RotationYawPitchRoll(ship.Rotation.Y, ship.Rotation.X, ship.Rotation.Z) * TGCMatrix.Translation(ship.Position);
        }

        public void Rotate(TgcD3dInput input, float elapsedTime)
        {
            float rotationSpeed = 0.01F;
            TGCVector3 rotation = this.ship.Rotation;
            if (input.keyDown(Key.LeftShift) && input.keyDown(Key.A))
            {
                this.ship.RotateX(rotationSpeed);
                rotation.X = Math.Min((float)Math.PI / 2, this.ship.Rotation.X);
            }
            else if (input.keyDown(Key.LeftShift) && input.keyDown(Key.D))
            {
                this.ship.RotateX(rotationSpeed * -1);
                rotation.X = Math.Max((float)-Math.PI / 2, this.ship.Rotation.X);
            } 
            else if (this.ship.Rotation.X < 0)
            {
                this.ship.RotateX(rotationSpeed * 1);
                rotation.X = Math.Min(0, this.ship.Rotation.X);
            }
            else if (this.ship.Rotation.X > 0)
            {
                this.ship.RotateX(rotationSpeed * -1);
                rotation.X = Math.Max(0, this.ship.Rotation.X);
            }
            this.ship.Rotation = rotation;
        }

        public void Render()
        {
            this.ship.Render();
        }

        public void RenderBoundingBox()
        {
            this.ship.BoundingBox.Render();
        }

        public void Dispose()
        {
            this.ship.Dispose();
        }

        public TGCVector3 Position
        {
            get { return this.ship.Position; }
            set { this.ship.Position = value; }
        }

        public TGCMatrix Transform
        {
            get { return this.ship.Transform; }
            set { this.ship.Transform = value; }
        }

        public TGCVector3 Rotation
        {
            get { return this.ship.Rotation; }
            set { this.ship.Rotation = value; }
        }
    }
}
