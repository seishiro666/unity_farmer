using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedSpawner : MonoBehaviour
{
    [SerializeField] GameObject bedPrefab;
    [SerializeField] GameObject bedsGrid;
    [SerializeField] UserData userData;

    float spacingColumn = -4.5f;
    float spacingRows = 5f;
    int rows, cols;

    public void StartSpawn()
    {
        cols = 3;
        rows = 2;

        SetupBedCount(userData.lvl);
    }

    public void SpawnBeds(int rows, int columns)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 position = new Vector3(
                    column * spacingColumn,
                    0.16f,
                    row * spacingRows
                );

                GameObject bed = Instantiate(bedPrefab, position, Quaternion.identity, bedsGrid.transform);
                bed.transform.localPosition = position;
            }
        }
    }

    public void SetupBedCount(int lvl)
    {
        cols = 3;
        rows = 2;

        if (lvl > 1 && lvl < 5)
        {
            int additionalBeds = (lvl - 1) * 3;
            rows = Mathf.CeilToInt((float)(6 + additionalBeds) / cols);
        }

        SpawnBeds(rows, cols);
    }
}
