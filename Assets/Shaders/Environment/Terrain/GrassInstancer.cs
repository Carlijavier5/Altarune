using System.Collections.Generic;
using UnityEngine;

public class GrassInstancer : MonoBehaviour {
    [Header("Grass Properties")]
    public int Instances;
    public Mesh mesh;
    public Material[] Materials;
    private List<List<Matrix4x4>> Batches = new List<List<Matrix4x4>>();
    private List<Vector4> normals = new List<Vector4>();
    private MaterialPropertyBlock propertyBlock;
    [SerializeField] private float grassScale = 1f;
    [SerializeField] private float grassYOffset = 0f;
    
    [Header("Render Properties")]
    [SerializeField] [Range(0, 1)] private float xCoord = 0;
    [SerializeField] [Range(0, 1)] private float zCoord = 0;

    [SerializeField] private Vector3 renderAreaSize = new Vector3(100f, 100f, 100f);
    public Terrain terrain;
    
    

    private void RenderBatches() {
        foreach (List<Matrix4x4> Batch in Batches) {
            for (int i = 0; i < mesh.subMeshCount; i++) {
                Graphics.DrawMeshInstanced(mesh, i, Materials[i], Batch, propertyBlock);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, renderAreaSize);
        Vector3 size = terrain.terrainData.size;
        Vector3 start = new Vector3(((xCoord * size.x)) + terrain.transform.position.x, terrain.SampleHeight(new Vector3(xCoord * size.x + terrain.transform.position.x, 0f, 
            (zCoord * size.z) + terrain.transform.position.z)) + terrain.transform.position.y, ((zCoord * size.z) + terrain.transform.position.z));
        Vector3 end = (terrain.terrainData.GetInterpolatedNormal(xCoord, zCoord) * 100) + new Vector3(0, terrain.SampleHeight(new Vector3(xCoord * size.x + terrain.transform.position.x, 0f, 
            (zCoord * size.z) + terrain.transform.position.z)) + terrain.transform.position.y, 0);
        Gizmos.DrawRay(start, end);
    }

    private void Update() {
        RenderBatches();
    }

    private void Start() {
        propertyBlock = new MaterialPropertyBlock();
        int AddedMatricies = 0;

        for (int i = 0; i < Instances; i++) {
            if (AddedMatricies < Instances && Batches.Count != 0) {
                float x = Random.Range(0, renderAreaSize.x) + transform.position.x - (renderAreaSize.x / 2) + 1;
                float y = Random.Range(0, renderAreaSize.y) + transform.position.y - (renderAreaSize.y / 2) + 1;
                float z = Random.Range(0, renderAreaSize.z) + transform.position.z - (renderAreaSize.z / 2) + 1;
                y = terrain.SampleHeight(new Vector3(x, y, z)) + grassYOffset;


                normals.Add(terrain.terrainData.GetInterpolatedNormal(
                    ((x - terrain.transform.position.x) / terrain.terrainData.size.x),
                    (z - terrain.transform.position.z) / terrain.terrainData.size.z));

                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, terrain.terrainData.GetInterpolatedNormal(
                    ((x - terrain.transform.position.x) / terrain.terrainData.size.x),
                    (z - terrain.transform.position.z) / terrain.terrainData.size.z));

                int orientation = Random.Range(0, 360);
                Batches[Batches.Count - 1].Add(item:
                    Matrix4x4.TRS(new Vector3(x, y, z),
                        Quaternion.Euler(new Vector3(0f, orientation, 0f)),
                        new Vector3(Random.Range(0.8f, 1.2f) * grassScale, Random.Range(0.8f, 1.2f) * grassScale, Random.Range(0.8f, 1.2f) * grassScale)));
                AddedMatricies += 1;
            }
            else {
                Batches.Add(new List<Matrix4x4>());
                AddedMatricies = 0;
            }
        }
        
        propertyBlock.SetVectorArray("_SurfaceNormal", normals);
    }
}
