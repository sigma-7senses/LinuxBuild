using UnityEngine;
using System;
using System.Threading.Tasks;

public class StdinReader : MonoBehaviour
{
    void Start()
    {
        Task.Run(() =>
        {
            string line;
            Debug.LogError("WhileStart");
            while ((line = Console.ReadLine()) != null)
            {
                Debug.LogError("GetLine:" + line);
            }
            Debug.LogError("WhileEnd");
        });
    }
}