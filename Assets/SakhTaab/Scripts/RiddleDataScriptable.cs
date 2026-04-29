using UnityEngine;
using System.Collections.Generic;

// позволяет создавать этот файл через меню Create (в папке Project)
[CreateAssetMenu(fileName = "RiddlesData", menuName = "RiddlesData", order = 1)]
public class RiddleDataScriptable : ScriptableObject
{
    public List<RiddleData> riddles; // список всех загадок
}