using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RiddlesData", menuName = "RiddlesData", order = 1)]
public class RiddleDataScriptable : ScriptableObject
{
    public List<RiddleData> riddles;
}