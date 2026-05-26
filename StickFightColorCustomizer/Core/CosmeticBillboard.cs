using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Hijos del hueso (LineRenderer): posición local sigue la animación; rotación mira a la cámara.
    /// </summary>
    public static class CosmeticBillboard
    {
        /// <summary>
        /// Debe llamarse después de asignar localPosition en un hijo del hueso.
        /// </summary>
        public static void FaceMainCamera(Transform t)
        {
            if (t == null)
            {
                return;
            }

            Camera cam = Camera.main;
            if (cam == null)
            {
                return;
            }

            t.rotation = cam.transform.rotation;
        }
    }
}
