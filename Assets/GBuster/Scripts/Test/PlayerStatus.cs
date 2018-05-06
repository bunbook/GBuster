using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour {

    public enum CommandType
    {
        Up = 0,
        Down,
        Right,
        Left,
        Shot
    }

    public enum StatusType
    {
        Alive = 0,
        Miss,
        Invincible
    }

    public StatusType status;

    public Dictionary<CommandType, int> commands;

    public float speed;

    public int hp;//or life

    public int waterAmount;

    public Vector3 direction;
    public Vector3 preDirection;

}
