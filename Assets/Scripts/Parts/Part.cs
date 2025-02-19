using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Part : MonoBehaviour, PickupAble
{
    public Item itemConfig;
    void Start()
    {
        if (itemConfig != null)
        {
            Apply();
        }
    }

    void Apply()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        if (itemConfig.Mesh != null )
        {
            meshFilter.mesh = itemConfig.Mesh;
            meshCollider.sharedMesh = itemConfig.Mesh;
        }

        if (itemConfig.Materials != null && itemConfig.Materials.Length > 0)
        {
            meshRenderer.materials = itemConfig.Materials;
        }
        
    }
    
}
