using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;
public class DiceRoller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        tr = GetComponent<Transform>();
    }
    private Rigidbody rb;
    private MeshRenderer mr;
    private Transform tr;
    public List<Vector3> sides;
    public float minForce;
    public float maxForce;
    public float3 cubeDimension;
    private Camera mainCamera;
    public bool ShowDice {
        get => mr.enabled;
        set => mr.enabled = value;
    }
    public bool cancel = false;
    public bool finished = true;
    public RecordedAnimation recordedPath = null;
    public void Roll(){
        if(!finished) throw new DiceException(DiceFailure.IsRecording);
        StartCoroutine(Recording());
    }
    /// <summary>
    /// Get A random horizontal force
    /// </summary>
    /// <returns>A random horizontal force</returns>
    Vector3 RandomForce() => Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) *Vector3.right * Random.Range(minForce, maxForce);
    /// <summary>
    /// Uniformly pick a random number inside of a cube
    /// </summary>
    /// <param name="center"> The center of a cube</param>
    /// <param name="size">The size of a cube</param>
    /// <param name="rotation">The rotation of a cube</param>
    /// <returns>A random point inside of a cube</returns> 
    Vector3 RandomInCube(Vector3 center, Vector3 size, Quaternion rotation){
        var randomPoint = new Vector3(Random.value-0.5f, Random.value-0.5f, Random.value-0.5f);
        randomPoint.Scale(size);
        return center + rotation * randomPoint;
    }
    /// <summary>
    /// Ensures the roll physics didn't get glitched out by checking whether the dice is inside the bounds
    /// </summary>
    /// <returns>true if the dice is inside the bounds</returns>
    private bool IsOnscreen(){
        var cameraCoord = mainCamera.WorldToViewportPoint(rb.position);
        return cameraCoord.x >= 0 && cameraCoord.y >= 0 && cameraCoord.z >= 0 && cameraCoord.x <= 1 && cameraCoord.y <= 1;
    }
    /// <summary>
    /// Rerun the dice roll, ensuring that the dice end up with the desired side
    /// </summary>
    /// <param name="target">The desired dice</param>
    public void Rerun(RecordedAnimation path, int target){
        var targetRotation = Quaternion.FromToRotation(sides[target], sides[path.resultFace]);
        if(!finished) throw new DiceException(DiceFailure.IsRecording);
        StartCoroutine(Rerolling(path, targetRotation));
    }
    private IEnumerator Recording()
    {
        recordedPath = null;
        var recorded = new RecordedAnimation();
        cancel = false;
        finished = false;
        yield return new WaitForFixedUpdate();
        tr.SetPositionAndRotation(Vector3.back, Random.rotationUniform);
        rb.AddForceAtPosition(RandomForce(), RandomInCube(rb.position, cubeDimension, rb.rotation), ForceMode.Impulse);
        while (true)
        {
            recorded.Record(rb.position, rb.rotation);
            if (!IsOnscreen())
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                recordedPath = null;
                break;
            }
            if (Mathf.Approximately(0, rb.angularVelocity.sqrMagnitude + rb.velocity.sqrMagnitude) && recorded.recording.Count > 2)
            {
                var rot = rb.rotation;
                var successful = false;
                for (var i = 0; i < 6; i++)
                {
                    if (rb.rotation * sides[i] == Vector3.back)
                    {
                        recorded.resultFace = i;
                        successful = true;
                        break;
                    }
                }
                recordedPath = successful ? recorded : null;
                break;
            }
            if(cancel){
                recordedPath = null;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        finished = true;
    }
    private IEnumerator Rerolling(RecordedAnimation path, Quaternion target)
    {
        cancel = false;
        finished = false;
        recordedPath = null;
        yield return new WaitForFixedUpdate();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        foreach(var (position, rotation) in path.recording){
            tr.SetPositionAndRotation(position, rotation*target);
            if(cancel) goto ends;
            yield return new WaitForFixedUpdate();
        }
        ends:
        finished = true;
    }
}
[System.Serializable]
public class RecordedAnimation{
    public List<(Vector3, Quaternion)> recording = new List<(Vector3, Quaternion)>();
    public int resultFace;
    public void Record(Vector3 position, Quaternion rotation){
        recording.Add((position, rotation));
    }
    public void Record(Transform transform) => Record(transform.position, transform.rotation);
}