using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSwitch : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(gameObject.name == "task1" && other.tag == "Player")
        {
            TaskManager.Instance.GetComponent<TaskManager>().Level1Tutorial();
        }
        else if(gameObject.name == "task2" && other.tag == "Player")
        {
            TaskManager.Instance.GetComponent<TaskManager>().Level2Tutorial();
        }
        else if(gameObject.name == "task3" && other.tag == "Player")
        {
            TaskManager.Instance.GetComponent<TaskManager>().Level3Tutorial();
        }
    }
}
