using UnityEngine;

public class WalkerObject
{
    public Vector2 position;
    public Vector2 direction;
    public float chanceToChangeDirection;

    public WalkerObject(Vector2 pos, Vector2 dir, float chance)
    {
        position = pos;
        direction = dir;
        chanceToChangeDirection = chance;
    }
}
