using UnityEngine;

namespace MK.Gizmo
{
    public static class GizmoUtility
    {
        public static void DrawBasisVectorGizmo(Vector2 pos, Vector2 up, Vector2 right)
        {
            DrawVectorGizmo(pos, right, -up, Color.red);
            DrawVectorGizmo(pos, up, right, Color.green);
        }

        public static void DrawBasisVectorGizmo(Vector3 pos, Vector3 up, Vector3 right, Vector3 forward)
        {
            DrawVectorGizmo(pos, right, -up, Color.red);
            DrawVectorGizmo(pos, up, right, Color.green);
            DrawVectorGizmo(pos, forward, -up, Color.blue);
        }

        public static void DrawArrow2D(Vector3 start, Vector3 end, Color color)
        {
            Color resetColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawLine(start, end);
            Vector2 rightVector = MKUtility.CalcPerpVector2D(end - start, 1f);
            Gizmos.DrawLine(end, Vector2.Lerp(start, end, 0.9f) + rightVector * 0.1f);
            Gizmos.DrawLine(end, Vector2.Lerp(start, end, 0.9f) - rightVector * 0.1f);
            Gizmos.color = resetColor;
        }

        public static void DrawVectorGizmo(Vector3 pos, Vector3 vector, Vector3 localRight, Color color)
        {
            Color resetColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawLine(pos, pos + vector);
            Gizmos.DrawLine(pos + vector, pos + (vector * 0.9f) + localRight * 0.1f);
            Gizmos.DrawLine(pos + vector, pos + (vector * 0.9f) - localRight * 0.1f);
            Gizmos.color = resetColor;
        }

        public static void DrawGrid(Vector2 pos, Vector2Int size, float pointRadius, Color color)
        {
            Color resetColor = Gizmos.color;
            Gizmos.color = color;
            for (int x = (-(size.x/2)); x < size.x/2; x++)
            {
                for (int y = (-(size.y / 2)); y < size.y / 2; y++)
                {
                    Gizmos.DrawSphere(pos + new Vector2(x, y), pointRadius);
                }
            }

            Gizmos.color = resetColor;
        }

        public static void DrawColoredLine(Vector3 from, Vector3 to, Color color)
        {
            Color resetColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawLine(from, to);
            Gizmos.color = resetColor;
        }

        public static void DrawBoundingBox(Vector3 pos, Vector3 forward, Vector3 up, Vector3 scale, Color color)
        {
            Color resetColor = Gizmos.color;
            Matrix4x4 resetMatrix = Gizmos.matrix;

            Gizmos.color = color;

            Matrix4x4 boundingBoxToWorld = Matrix4x4.TRS
            (
                pos,
                Quaternion.LookRotation(forward, up),
                Vector3.one
            );

            Vector3[] boundingBoxPoints = new Vector3[]
            {
                // Bottom 4 positions
                new Vector3(1, 0, 1),
                new Vector3(-1, 0, 1),
                new Vector3(-1, 0, -1),
                new Vector3(1, 0, -1),
                // Top 4 positions
                new Vector3(1, 2, 1),
                new Vector3(-1, 2, 1),
                new Vector3(-1, 2, -1),
                new Vector3(1, 2, -1)
            };

            Gizmos.matrix = boundingBoxToWorld;

            int bottomIndexCount = boundingBoxPoints.Length / 2;
            for (int i = 0; i < bottomIndexCount; i++)
            {               
                Vector3 turretBoundingBoxWorldPos = boundingBoxPoints[i];

                // Bottom square
                int nextIndex = (i + 1) % bottomIndexCount;
                Vector3 nextTurretBoundingBoxWorldPos = boundingBoxPoints[nextIndex];
                
                Gizmos.DrawLine(turretBoundingBoxWorldPos, nextTurretBoundingBoxWorldPos);

                // Vertical Lines
                int topIndex = (i + 4);
                Vector3 topTurretBoundingBoxWorldPos = boundingBoxPoints[topIndex];

                Gizmos.DrawLine(turretBoundingBoxWorldPos, topTurretBoundingBoxWorldPos);

                // Top square
                int topNextIndex = ((topIndex + 1) % (bottomIndexCount)) + 4;
                Vector3 nextTopTurretBoundingBoxWorldPos = boundingBoxPoints[topNextIndex];

                Gizmos.DrawLine(topTurretBoundingBoxWorldPos, nextTopTurretBoundingBoxWorldPos);
            }

            Gizmos.matrix = resetMatrix;
            Gizmos.color = resetColor;
        }

        public static void DrawRegularPolygon(Vector3 pos, Vector3 forward, Vector3 up, Vector2[] verts, int density, Color color)
        {
            if (density < 1) return;

            Color resetColor = Gizmos.color;
            Matrix4x4 resetMatrix = Gizmos.matrix;

            Gizmos.color = color;

            Matrix4x4 localToWorld = Matrix4x4.TRS
            (
                pos,
                Quaternion.LookRotation(forward, up),
                Vector3.one
            );

            Gizmos.matrix = localToWorld;

            for (int i = 0; i < verts.Length; i++)
            {
                int nextIndex = (i + density) % verts.Length;
                Gizmos.DrawLine(verts[i], verts[nextIndex]);
            }

            Gizmos.matrix = resetMatrix;
            Gizmos.color = resetColor;
        }
    }
}

