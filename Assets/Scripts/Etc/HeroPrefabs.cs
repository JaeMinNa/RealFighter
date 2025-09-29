using UnityEngine;

public class HeroPrefabs : MonoBehaviour
{
    [SerializeField] private GameObject Obj_RenderCamera = null;
    [SerializeField] private GameObject Obj_Heroes = null;

    private void Start()
    {
        for (int index = 0; index < Obj_Heroes.transform.childCount; ++index)
        {
            Obj_Heroes.transform.GetChild(index).transform.LookAt(Obj_RenderCamera.transform);
        }
    }

}
