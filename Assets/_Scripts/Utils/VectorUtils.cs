using UnityEngine;

public static class VectorUtils {

    /// <summary>
    /// Splits a vector into 2 cardinal vectors;
    /// <br></br> Will return default values if all dimensions are non-zero;
    /// <br></br> The normal must be cardinal (i.e. Vector3.left);
    /// </summary>
    /// <param name="normal"> Cardinal normal to split the vector by; </param>
    /// <param name="vec"> Vector to split; </param>
    /// <param name="vec1"> First dimension; </param>
    /// <param name="vec2"> Second dimension; </param>
    public static void SplitVectorByNormal(Vector3 normal, Vector3 vec,
                                          out Vector3 vec1, out Vector3 vec2) {
        if (normal.x != 0) {
            vec1 = new(0, vec.y, 0);
            vec2 = new(0, 0, vec.z);
        } else if (normal.y != 0) {
            vec1 = new(vec.x, 0, 0);
            vec2 = new(0, 0, vec.z);
        } else if (normal.z != 0) {
            vec1 = new(vec.x, 0, 0);
            vec2 = new(0, vec.y, 0);
        } else {
            Debug.LogWarning("SplitVectorByNormal() requires a cardinal normal with non-zero values;"
                           + "\nDefault vectors were returned;");
            vec1 = Vector3.zero;
            vec2 = Vector3.zero;
        }
    }

    /// <summary>
    /// Injects a series of points from one array into another forming vector pairs;
    /// </summary>
    /// <param name="srcArr"> Source array; </param>
    /// <param name="destArr"> Destination array; </param>
    /// <param name="destIndex"> Start index in destination array; </param>
    public static void InjectSegments(this Vector3[] srcArr, Vector3[] destArr,
                                    int destIndex) {
        for (int i = 0; i < srcArr.Length * 2; i += 2) {
            destArr[i + destIndex] = srcArr[i / 2];
            destArr[i + destIndex + 1] = srcArr[(i / 2 + 1) % srcArr.Length];
        }
    }

    /// <summary>
    /// Get a random unit direction vector in the X-Z plane
    /// </summary>

    /// <summary>
    /// Offset an array of vectors;
    /// </summary>
    /// <param name="vecs"> Array to operate on; </param>
    /// <param name="offset"> Offset to be added to each vector; </param>
    public static void Offset(this Vector3[] vecs, Vector3 offset) {
        for (int i = 0; i < vecs.Length; i++) vecs[i] += offset;
    }

    public static Vector3 Mod(Vector3 vec1, Vector3 vec2) {
        return new Vector3(vec1.x % vec2.x,
                           vec1.y % vec2.y,
                           vec1.z % vec2.z);
    }

    public static Vector3 Abs(this Vector3 vec) {
        return new Vector3(Mathf.Abs(vec.x),
                           Mathf.Abs(vec.y),
                           Mathf.Abs(vec.z));
    }

    public static Vector3 Mult(Vector3 vec1, Vector3 vec2) {
        return new Vector3(vec1.x * vec2.x,
                           vec1.y * vec2.y,
                           vec1.z * vec2.z);
    }

    public static Vector3Int Normalize(this Vector3Int vec) {
        return ((Vector3) vec).normalized.Round();
    }

    public static Vector3Int Abs(this Vector3Int vec) {
        return new Vector3Int(Mathf.Abs(vec.x),
                              Mathf.Abs(vec.y),
                              Mathf.Abs(vec.z));
    }

    public static Vector3Int Round(this Vector3 vec) {
        return new Vector3Int(Mathf.RoundToInt(vec.x),
                              Mathf.RoundToInt(vec.y),
                              Mathf.RoundToInt(vec.z));
    }

    public static Vector3 GetRandomUnitVector2D() {
        float angle = Random.Range(0f, Mathf.PI * 2);
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }
}
