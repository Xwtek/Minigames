using UnityEngine;
using UnityEditor;
using Unity.Mathematics;
[ExecuteInEditMode]
public class Zoomer : MonoBehaviour {
    public float2 center;
    public float2 size;
    private void Start() {
    }
    private void Update() {
        transform.localScale = new Vector3(Scale, Scale, 1);
        var actualCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        var currCenter = transform.TransformPoint(new Vector3(center.x, center.y, 0));
        var diff = actualCenter - currCenter;
        diff.z = 0;
        transform.position += diff;
    }
    private Vector3 WorldCenter => transform.TransformPoint(new Vector3(center.x, center.y, 0));
    private float Scale{
        get
        {
            float3 actualBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0));
            float3 actualTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
            return math.cmin((actualTopRight - actualBottomLeft).xy / size);
        }
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(WorldCenter, Scale *new Vector3(size.x, size.y));
    }
}