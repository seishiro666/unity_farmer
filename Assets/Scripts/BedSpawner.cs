using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BedSpawner : MonoBehaviour
{
    public GameObject bedPrefab;
    public GameObject bedsGrid;
    float spacingColumn = -4.5f;
    float spacingRows = 5f;
    int rows, cols;

    public void StartSpawn()
    {
        rows = 2;
        cols = 3;
        SpawnBeds(rows, cols);
    }

    void SpawnBeds(int rows, int columns)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 position = new Vector3(
                    column * spacingColumn,
                    0,
                    row * spacingRows
                );

                GameObject bed = Instantiate(bedPrefab, position, Quaternion.identity, bedsGrid.transform);
                bed.transform.localPosition = position;
            }
        }
    }
}
