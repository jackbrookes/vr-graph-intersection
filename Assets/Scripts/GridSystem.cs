using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{

	public int nx = 4;
    public int ny = 4;
    public int nz = 4;

    public float spacing = 0.2f;

	List<Vector3> availablePositions;

	public GridSystem()
	{
        Reset();
	} 

	public void Reset()
	{

		Vector3 offset = -1 * new Vector3(nx, ny, nz) * spacing * 0.5f;

        availablePositions = new List<Vector3>(nx * ny * nz);
        for (int x = 0; x < nx; x++)
            for (int y = 0; y < ny; y++)
				for (int z = 0; z < nz; z++)
				{
					Vector3 rand = new Vector3(
						Random.value - .5f,
                        Random.value - .5f,
                        Random.value - .5f
					) * spacing; 
                    availablePositions.Add(new Vector3(x, y, z) * spacing + offset + rand);
                }

	}

	public Vector3 Sample()
	{
		int idx = Random.Range(0, availablePositions.Count);
		var sample = availablePositions[idx];
        availablePositions.RemoveAt(idx);
		return sample;
    }



}
