using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Input;

namespace TGC.Group.Model
{
    public class Enemy
    {
        private string Path;
        private TgcMesh mesh;
        private TGCVector3 pivot;
        //private TGCVector3 lookAt;
        private float radio = 50;
        private float rotationSpeed = 10f;
        //private TGCMatrix YPRRrotation;
        //private TGCMatrix CircularRotation;


        public Enemy(string pathEnemy, TGCVector3 center)
        {
            Path = pathEnemy;
            mesh = new TgcSceneLoader().loadSceneFromFile(Path +"Enemy.xml").Meshes[0];
            pivot = center;

            mesh.Rotation = new TGCVector3(0, -FastMath.PI_HALF, 0);
            mesh.Position = new TGCVector3(0, 0, -300);
            mesh.Transform = TGCMatrix.Scaling(TGCVector3.One) * TGCMatrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z) * TGCMatrix.Translation(mesh.Position);

            mesh.Transform = TGCMatrix.Translation(pivot);

        }

        public bool DetectarClick(TgcPickingRay pickingRay)
        {
            pickingRay.updateRay();
            var output = TGCVector3.One;

            if (TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, mesh.BoundingBox, out output))
            {
                return true;
            }


            return false;
        }

        public void Update(float ElapsedTime)
        {
            //mesh.Position = pivot;
            //var rot = new TGCVector3(rotationSpeed * ElapsedTime, 0, 0);
            //mesh.Rotation += new TGCVector3(rotationSpeed * ElapsedTime, 0, 0);
            //var rotCentral = TGCMatrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z);

            //mesh.Position.X += radio;

            //mesh.Transform = rotCentral* TGCMatrix.Translation(new TGCVector3(radio,0,0)) * TGCMatrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z)* TGCMatrix.Translation(mesh.Position);

        }
        public void Render()
        {
            mesh.Render();
        }

        public TgcBoundingAxisAlignBox BoundingBox()
        {
            return mesh.BoundingBox;
        }
    }
}
