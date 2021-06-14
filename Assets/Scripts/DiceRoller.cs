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
    public List<(float3, Quaternion)> record = new List<(float3, Quaternion)>();
    public float minForce;
    public float maxForce;
    public float3 cubeDimension;
    private bool recording = false;
    private int? rerun = null;
    private Quaternion? target = null;
    public bool Running => recording || rerun.HasValue;
    public int currResult = 0;
    private Camera mainCamera;
    public bool Failed { get; private set; } = false;
    public bool ShowDice {
        get => mr.enabled;
        set => mr.enabled = value;
    }
    public void Roll(){
        if(recording) throw new DiceException(DiceFailure.IsRecording);
        if(rerun.HasValue) throw new DiceException(DiceFailure.IsReplaying);
        record.Clear();
        Failed = false;
        recording = true;
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
    private void FixedUpdate(){
        if (recording)
        {
            if (record.Count == 0)
            {
                tr.SetPositionAndRotation(Vector3.back, Random.rotationUniform);
                rb.AddForceAtPosition(RandomForce(), RandomInCube(rb.position, cubeDimension, rb.rotation), ForceMode.Impulse);
            }
            
            record.Add((rb.position, rb.rotation));
            if(!IsOnscreen()){
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                recording = false;
                Failed = true;
                record.Clear();
            }
            if (Mathf.Approximately(0, rb.angularVelocity.sqrMagnitude + rb.velocity.sqrMagnitude) && record.Count > 2)
            {
                recording = false;
                var rot = rb.rotation;
                var successful = false;
                for (var i = 0; i < 6; i++)
                {
                    if (rb.rotation * sides[i] == Vector3.back)
                    {
                        currResult = i;
                        successful = true;
                        break;
                    }
                }
                Failed |= !successful;
                if(Failed) record.Clear();
                Debug.Log(currResult);
            }
        }else if(rerun.HasValue){
            var (position, rotation) = record[rerun.Value];
            tr.SetPositionAndRotation(position, rotation*target.Value);
            rerun++;
            if (rerun == record.Count)
            {
                rerun = null;
                target = null;
            }
        }
    }
    /// <summary>
    /// Rerun the dice roll, ensuring that the dice end up with the desired side
    /// </summary>
    /// <param name="target">The desired dice</param>
    public void Rerun(int target){
        if(Running) throw new DiceException(DiceFailure.IsRecording);
        if(record.Count == 0) throw new DiceException(DiceFailure.NoRecord);
        if(this.rerun.HasValue) throw new DiceException(DiceFailure.IsReplaying);
        rerun = 0;
        this.target = Quaternion.FromToRotation(sides[target], sides[currResult]);
    }
}
