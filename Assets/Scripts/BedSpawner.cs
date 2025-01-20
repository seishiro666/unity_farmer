using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        rows = 2;
        cols = 3;
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
        rows = 2; cols = 3;
        if (lvl > 1 && lvl < 5)
        {
            rows += lvl - 1;
        }
        SpawnBeds(rows, cols);
    }
}
