using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobitTarget : MonoBehaviour
{
    [SerializeField]
    AssetListSO targetList;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void OnEnable()
    {
        targetList.objects.Add(gameObject);
    }

    void OnDisable()
    {
        targetList.objects.Remove(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Robit robit;
        if (other.TryGetComponent<Robit>(out robit))
        {
            meshRenderer.material.color = robit.color;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
