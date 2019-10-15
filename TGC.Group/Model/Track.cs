using Microsoft.DirectX.Direct3D;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    public class Track
    {
        private List<TgcMesh> walls_l;
        private List<TgcMesh> walls_r;
        private List<TgcMesh> floors;
        public int repeat;
        public TGCVector3 center;

        public TgcTexture textura;

        public Track(TGCVector3 centro, string pathTextura, int repeticiones)
        {
            //var d3dDevice = D3DDevice.Instance.Device;
            //var shader = Effect.FromFile(d3dDevice, MediaDir + "MotionBlur.fx",null, null, ShaderFlags.PreferFlowControl, null, out compilationErrors);

            repeat = repeticiones;
            center = centro;

            var size_lados = new TGCVector3(0, 320, 640);
            var size_suelo = new TGCVector3(160, 0, 640);

            textura = TgcTexture.createTexture(pathTextura);


            walls_l = new List<TgcMesh>();
            walls_r = new List<TgcMesh>();
            floors = new List<TgcMesh>();

            var plane = new TgcPlane(center,center, TgcPlane.Orientations.YZplane, textura);


            for (int i = 0; i < repeticiones; i++)
            {
                var walll = new TgcPlane(center + new TGCVector3(-80, -160, -320 - (i * 640)), size_lados, TgcPlane.Orientations.YZplane, textura);
                walls_l.Add(walll.toMesh("wall_l_"+i));
                var wallr = new TgcPlane(center + new TGCVector3(80, -160, -320 - (i * 640)), size_lados, TgcPlane.Orientations.YZplane, textura);
                walls_r.Add(wallr.toMesh("wall_r_" + i));
                var floor = new TgcPlane(center + new TGCVector3(-80, -160, -320 - (i * 640)), size_suelo, TgcPlane.Orientations.XZplane, textura);
                floors.Add(floor.toMesh("floor_" + i));
            }
        }

        public void Render()
        {
            foreach (var mesh in walls_l) { mesh.Render(); }
            foreach (var mesh in walls_r) { mesh.Render(); }
            foreach (var mesh in floors) { mesh.Render(); }
        }
        public void Dispose()
        {
            foreach (var mesh in walls_l) { mesh.Dispose(); }
            foreach (var mesh in walls_r) { mesh.Dispose(); }
            foreach (var mesh in floors) { mesh.Dispose(); }
            textura.dispose();
        }

        public void Move_forward(TGCVector3 forward_movement)
        {
            this.Move_forward(forward_movement, walls_l);
            this.Move_forward(forward_movement, walls_r);
            this.Move_forward(forward_movement, floors);
        }

        private void Move_forward(TGCVector3 forward_movement, List<TgcMesh> meshes)
        {
            foreach (var mesh in meshes)
            {
                mesh.Position += -forward_movement;// + forward_movement;
                if (mesh.Position.Z > this.center.Z + 640)
                {
                    mesh.Position += new TGCVector3(0, 0, -640);
                }
                mesh.Transform = TGCMatrix.Translation(mesh.Position);
            }
        }



    }
}
