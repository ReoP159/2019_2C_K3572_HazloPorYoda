using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using Effect = Microsoft.DirectX.Direct3D.Effect;

namespace TGC.Group.Model
{
    class Player
    {
        private readonly TgcSceneLoader loader;
        private readonly String mediaDir;
        private readonly TgcD3dInput input;
        private readonly TgcMesh ship;
        private readonly Effect rollEffect;

        private Key LastKeyPressed;
        private int LastKeyPressedMillis;
        private Boolean Rolling = false;

        public Player(TgcSceneLoader loader, String mediaDir, String shadersDir, TgcD3dInput input)
        {
            this.loader = loader;
            this.mediaDir = mediaDir;
            this.input = input;

            rollEffect = TGCShaders.Instance.LoadEffect(shadersDir + "Varios.fx");

            ship = loader.loadSceneFromFile(mediaDir + "StarWars-Speeder\\StarWars-Speeder-TgcScene.xml").Meshes[0];
            ship.Position = new TGCVector3(0, 0, 0);
            ship.Rotation = new TGCVector3(0, FastMath.PI / 2, 0);
            ship.Transform = TGCMatrix.Scaling(TGCVector3.One * 0.5f) * TGCMatrix.RotationYawPitchRoll(ship.Rotation.Y, ship.Rotation.X, ship.Rotation.Z) * TGCMatrix.Translation(ship.Position);
        }

        public void Update(float elapsedTime)
        {
            if (this.Rolling)
            {
                this.DoRoll(elapsedTime);
            }
            else
            {
                this.CheckRoll(elapsedTime);
                this.DoRotate(elapsedTime);
            }
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

        private void CheckRoll(float elapsedTime)
        {
            if (input.keyPressed(Game.Default.LeftKey))
            {
                this.SetRolling(Game.Default.LeftKey == LastKeyPressed && Environment.TickCount - LastKeyPressedMillis < Game.Default.RollInterval);
                LastKeyPressed = Game.Default.LeftKey;
                LastKeyPressedMillis = Environment.TickCount;
            }
            else if (input.keyPressed(Game.Default.RightKey))
            {
                this.SetRolling(Game.Default.RightKey == LastKeyPressed && Environment.TickCount - LastKeyPressedMillis < Game.Default.RollInterval);
                LastKeyPressed = Game.Default.RightKey;
                LastKeyPressedMillis = Environment.TickCount;
            }
        }

        private void DoRoll(float elapsedTime)
        {
            TGCVector3 rotation = this.ship.Rotation;
            if (Game.Default.LeftKey == LastKeyPressed)
            {
                this.ship.RotateX(Game.Default.RotationSpeed);
                this.SetRolling(Math.PI * 2 > Math.Abs(this.ship.Rotation.X));
                rotation.X = this.Rolling ? ship.Rotation.X : 0; 
            }
            else if (Game.Default.RightKey == LastKeyPressed)
            {
                this.ship.RotateX(Game.Default.RotationSpeed * -1);
                this.SetRolling(Math.PI * 2 > Math.Abs(this.ship.Rotation.X));
                rotation.X = this.Rolling ? ship.Rotation.X : 0;
            }
            this.ship.Rotation = rotation;
        }

        private void DoRotate(float elapsedTime)
        {
            TGCVector3 rotation = this.ship.Rotation;
            if (this.input.keyDown(Key.LeftShift) && input.keyDown(Game.Default.LeftKey))
            {
                this.ship.RotateX(Game.Default.RotationSpeed);
                rotation.X = Math.Min((float)Math.PI / 2, this.ship.Rotation.X);
            }
            else if (this.input.keyDown(Key.LeftShift) && input.keyDown(Game.Default.RightKey))
            {
                this.ship.RotateX(Game.Default.RotationSpeed * -1);
                rotation.X = Math.Max((float)-Math.PI / 2, this.ship.Rotation.X);
            }
            else if (this.ship.Rotation.X < 0)
            {
                this.ship.RotateX(Game.Default.RotationSpeed * 1);
                rotation.X = Math.Min(0, this.ship.Rotation.X);
            }
            else if (this.ship.Rotation.X > 0)
            {
                this.ship.RotateX(Game.Default.RotationSpeed * -1);
                rotation.X = Math.Max(0, this.ship.Rotation.X);
            }
            this.ship.Rotation = rotation;
        }

        private void SetRolling(Boolean rolling)
        {
            this.Rolling = rolling;
        }

    }
}
