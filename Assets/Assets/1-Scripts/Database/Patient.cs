using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patient", menuName = "ScriptableObjects/Patient", order = 1)]
public class Patient : ScriptableObject
{
    public string first_name;
    public string last_name;
    public string gender;
    public string date_of_birth;
    public string email;
}
