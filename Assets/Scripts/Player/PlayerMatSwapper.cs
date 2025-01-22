using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMatSwapper : MonoBehaviour
{
    public Material basePlayerMaterial;
    public Material invisibleMaterial;
    public Material coreMaterial;


    private readonly MeshRenderer[] playerMeshes;
    public MeshRenderer chestLightUpMesh;
    // Start is called before the first frame update
    void Start()
    {
        var tmpObjs = GameObject.FindGameObjectsWithTag("PlayerMesh");
        foreach (var obj in tmpObjs)
        {
            TryGetComponent<MeshRenderer>(out MeshRenderer m);
            if (m != null)
            {
                playerMeshes.Append(m);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetPlayerColor(Color line, Color fill)
    {

    }
    void StartLerpPlayerColor(Color line, Color fill)
    {

    }
    void ResetPlayerColor()
    {
        foreach(MeshRenderer m in playerMeshes){
            m.material = basePlayerMaterial;
        }
    }
}
