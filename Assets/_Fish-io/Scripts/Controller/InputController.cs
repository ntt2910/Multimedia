using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : SingletonMonoBehavier<InputController>
{
    [SerializeField] Joystick joystick;

    public float Hor => joystick.Horizontal;
    public float Ver => joystick.Vertical;
}
