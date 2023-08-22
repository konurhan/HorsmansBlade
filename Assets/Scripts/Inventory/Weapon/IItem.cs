using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    public string Name { get; set; }
    public float Weight { get; set; }
    public float Price { get; set; }

}
