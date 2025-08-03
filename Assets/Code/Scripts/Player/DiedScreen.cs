using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiedScreen : MonoBehaviour
{
    [Range(1,20)]
    [SerializeField] float Radius = 1;
    private Material material;
    void Start()
    {
        if(TryGetComponent(out Image image)){
            material = image.material;
        }
    }
    void Update()
    {
        material.SetFloat("_Radius",Radius);
    }

    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
