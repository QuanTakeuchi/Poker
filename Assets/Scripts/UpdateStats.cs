using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UpdateStats : MonoBehaviour
{

    private TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = "Updated Statistics";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
