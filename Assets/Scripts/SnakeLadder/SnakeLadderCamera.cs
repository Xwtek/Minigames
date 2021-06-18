using UnityEngine;
using CommonData;
using Unity.Mathematics;
namespace SnakeLadder
{
    public class SnakeLadderCamera : MonoBehaviour
    {
        public Zoomer zoomer;
        public Token focusOn;
        private void Update()
        {
            if (focusOn == null)
            {
                zoomer.center = new float2(4.5f, 4.5f);
                zoomer.size = new float2(10, 10);
            }
            else
            {
                zoomer.center = ((float3)focusOn.transform.localPosition).xy;
                zoomer.size = new float2(3, 3);
            }
        }
    }
}