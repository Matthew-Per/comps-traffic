using UnityEngine;

public class ClothoidSpiral : MonoBehaviour
{
    public float length = 10.0f;  // Total length of the spiral
    public float curvatureRate = 0.1f;
    public int keyframesCount = 100;  // Number of keyframes for the spiral

    void OnDrawGizmos()
    {
        GenerateSpiral();
    }

    void GenerateSpiral()
    {
        // Generate points for the clothoid spiral
        float deltaS = length / keyframesCount;
        float curvature = 0.0f;
        float xOffset = transform.position.x;
        float zOffset = transform.position.z;

        for (int i = 0; i < keyframesCount; i++)
        {
            float s = i * deltaS;
            float theta = 0.5f * curvature * s * s;
            float x = s * Mathf.Cos(theta);
            float z = s * Mathf.Sin(theta);
            Gizmos.DrawSphere(new Vector3(x+xOffset, 0.0f, z+zOffset), 0.1f);

            curvature += curvatureRate;
        }
    }
}
