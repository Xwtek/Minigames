using UnityEngine;
namespace SnakeLadder
{
public class Token : MonoBehaviour
{
    private Texture2D _texture;
    public SpriteRenderer spriteRenderer;
    private Sprite _sprite;
    public Texture2D image
    {
        get => _texture;
        set
        {
            _texture = value;
            _sprite = Sprite.Create(value, new Rect(Vector2.zero, Vector2.one * 500), new Vector2(0.5f, 0.5f), 125f / 0.3f);
            spriteRenderer.sprite = _sprite;
        }
    }
    private Movement? currentMovement;
    public bool IsMoving => currentMovement.HasValue;
public void MoveNext(int nextBox, int tokenInBox, int tokenIndexInBox, float distance, float totalTime)
{
    var movement = new Movement
    {
        source = transform.localPosition,
        target = SnakeLadderHelper.ToCoordinate(nextBox).ToZoomingCoordinate(transform.position.z),
        elapsedTime = 0f,
        totalTime = totalTime
    };
    if (tokenInBox > 0 && tokenIndexInBox > 0)
    {
        // Set them aside
        float angle = 360 * tokenIndexInBox;
        if (tokenInBox > 1) angle /= tokenInBox - 1;
        angle += 135;
        angle *= Mathf.Deg2Rad;
        movement.target += new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * distance;
    }
    currentMovement = movement;
}
        private void Update()
        {
            if (currentMovement.HasValue)
            {
                var mov = currentMovement.Value;
                mov.elapsedTime += Time.deltaTime;
                var t = mov.totalTime <= 0 ? 1 : mov.elapsedTime / mov.totalTime;
                if (t >= 1)
                {
                    transform.localPosition = mov.target;
                    currentMovement = null;
                }
                else
                {
                    transform.localPosition = t * mov.target + (1 - t) * mov.source;
                    currentMovement = mov;
                }
            }
        }
        struct Movement
        {
            public Vector3 source;
            public Vector3 target;
            public float elapsedTime;
            public float totalTime;
        }
    }
}