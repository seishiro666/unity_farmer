using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedController : MonoBehaviour
{
    GameObject currentBed;

    public void SetupBed(GameObject bed)
    {
        currentBed = bed;
        BedWork bedWork = currentBed.transform.parent.GetComponent<BedWork>();
        bedWork.SetupBed();
    }

    public GameObject GetCurrentBed()
    {
        return currentBed;
    }

    public void ClearBed()
    {
        currentBed = null;
    }
}
