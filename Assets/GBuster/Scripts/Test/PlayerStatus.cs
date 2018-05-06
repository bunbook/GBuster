using UnityEngine;

public class PlayerStatus : MonoBehaviour {

    public enum StatusType
    {
        Alive = 0,
        Miss,
        Invincible
    }

    public StatusType status;

    public float speed;

    public int hp;//or life

    public int waterAmount;

    public Vector3 direction;
    public Vector3 preDirection;

}
