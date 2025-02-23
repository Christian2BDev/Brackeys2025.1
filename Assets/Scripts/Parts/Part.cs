using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Part : MonoBehaviour, PickupAble
{
    public Item itemConfig;
    public Vector3 startPosition;
    public GameObject canvasE;
    void Start()
    {
        if (itemConfig != null)
        {
            Apply();
        }
        startPosition = transform.position;
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

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            if (player.pickups.Contains(gameObject))return ;
            player.pickups.Add(gameObject);
            canvasE.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            player.pickups.RemoveAll(item => item == gameObject);
            canvasE.SetActive(false);
        }
    }
}
