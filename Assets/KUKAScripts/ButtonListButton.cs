using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListButton : MonoBehaviour
{
    [SerializeField]
    private Text myText;
    [SerializeField]
    private ButtonListControl btnControl;
    

    public void setText(string t) {
        myText.text = t;
        this.gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        btnControl.buttonClicked(myText.text);
    }




}
