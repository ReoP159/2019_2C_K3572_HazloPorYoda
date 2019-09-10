using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    public class Track
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

            for (int i = 0; i < repeticiones; i++)
            {
                walls_l.Add(new TgcPlane(center + new TGCVector3(-50, -80, -80 - (i * 160)), size_lados, TgcPlane.Orientations.YZplane, texture));
                walls_r.Add(new TgcPlane(center + new TGCVector3(50, -80, -80 - (i * 160)), size_lados, TgcPlane.Orientations.YZplane, texture));
                floor.Add(new TgcPlane(center + new TGCVector3(-50, -80, -80 - (i * 160)), size_suelo, TgcPlane.Orientations.XZplane, texture));
            }
        }

        public void Render()
        {
            foreach (var plane in walls_l) { plane.Render(); }
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
}
