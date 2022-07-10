using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

public class Android : MonoBehaviour
{
    string path;
    // Start is called before the first frame update
    void Start()
    {
        var uri = new System.Uri(path);
        var converted = uri.AbsoluteUri;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
