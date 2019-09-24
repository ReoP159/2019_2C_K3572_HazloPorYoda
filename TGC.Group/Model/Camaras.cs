using TGC.Core.Camara;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{

    public class CamaraNave : TgcCamera
    {
        private TGCVector3 position;

        /// <summary>
        ///     Desplazamiento en altura de la camara respecto del target
        /// </summary>
        public float DesplazarAltura { get; set; }

        /// <summary>
        ///     Desplazamiento hacia adelante o atras de la camara repecto del target.
        ///     Para que sea hacia atras tiene que ser negativo.
        /// </summary>
        public float DesplazarAdelante { get; set; }

        /// <summary>
        ///     Rotacion absoluta en Y de la camara
        /// </summary>
        public float RotationY { get; set; }

        /// <summary>
        ///     Objetivo al cual la camara tiene que apuntar
        /// </summary>
        public TGCVector3 Target { get; set; }

        //------------------------------------------------------------------
        
        private void seleccionCamara(int posicionCamara)
        {
            switch (posicionCamara)
            {
                case 0:
                    DesplazarAdelante = 200;
                    DesplazarAltura = 60;
                    break;
                case 1:
                    DesplazarAdelante = 250;
                    DesplazarAltura = 90;
                    break;
                case 2:
                    DesplazarAdelante = 300;
                    DesplazarAltura = 100;
                    break;
            }
        }
        public CamaraNave(TGCVector3 target, int posicionCamara)
        {
            Target = target;
            seleccionCamara(posicionCamara);
            
        }

        public void CambiarPosicionCamara(int posicionCamara)
        {
            seleccionCamara(posicionCamara);
        }
        
        public override void UpdateCamera(float elapsedTime)
        {
            TGCVector3 targetCenter;
            CalculatePositionTarget(out position, out targetCenter);
            SetCamera(position, targetCenter);
        }

        /// <summary>
        ///     Carga los valores default de la camara y limpia todos los cálculos intermedios
        /// </summary>
        public void resetCamera()
        {
            DesplazarAltura = 200;
            DesplazarAdelante = 60;
            RotationY = 0;
            Target = TGCVector3.Empty;
            position = TGCVector3.Empty;
        }

        
        public void setTargetOffsets(TGCVector3 target, float desplazarAltura, float desplazarAdelante)
        {
            Target = target;
            DesplazarAltura = desplazarAltura;
            DesplazarAdelante = desplazarAdelante;
        }

        /// <summary>
        ///     Genera la proxima matriz de view, sin actualizar aun los valores internos
        /// </summary>
        /// <param name="pos">Futura posicion de camara generada</param>
        /// <param name="targetCenter">Futuro centro de camara a generada</param>
        public void CalculatePositionTarget(out TGCVector3 pos, out TGCVector3 targetCenter)
        {
            //alejarse, luego rotar y lueg ubicar camara en el centro deseado
            targetCenter = Target;
            var m = TGCMatrix.Translation(0, DesplazarAltura, DesplazarAdelante) * TGCMatrix.RotationY(RotationY) * TGCMatrix.Translation(targetCenter);

            //Extraer la posicion final de la matriz de transformacion
            pos = new TGCVector3(m.M41, m.M42, m.M43);
        }

        /// <summary>
        ///     Rotar la camara respecto del eje Y
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateY(float angle)
        {
            RotationY += angle;
        }
    }
}
