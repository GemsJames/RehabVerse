using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ButtonListControl : MonoBehaviour
{
    [SerializeField]

    private GameObject btnTemplate;

    string robot_path = "Assets/ROBOT_PATHS/";

    public GUI_IIWA otherScipt;

    void Start() {
        DirectoryInfo dir = new DirectoryInfo(robot_path);
        FileInfo[] info = dir.GetFiles("*.txt");
        foreach (FileInfo f in info) {

            string p = f.FullName;
            string filename = Path.GetFileName(p);
            
            GameObject btn = Instantiate(btnTemplate) as GameObject;

            btn.SetActive(true);

            btn.GetComponent<ButtonListButton>().setText(filename);

            btn.transform.SetParent(btnTemplate.transform.parent, false);
         }
        
    }

    public void buttonClicked(string m) {
        otherScipt.loadPathCallback(m);
    }


}
