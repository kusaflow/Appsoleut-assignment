using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    // List of mesh renderers to change materials
    public List<MeshRenderer> meshRenderers; 

    public void SetMaterial(Material material)
    {
       // transform.rotation = 
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.material = material;
        }
    }
}
