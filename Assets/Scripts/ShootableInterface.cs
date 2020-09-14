using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Shootable
{
    void OnShot(Vector3 position, Vector3 directrion);
}
