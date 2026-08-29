using UnityEngine;

public class Test : MonoBehaviour
{
    private int n = 0;

    private float timer = 0f;
    void Awake()
    {
        Debug.Log("Awake");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        n++;
        Debug.Log(Time.deltaTime);
        //if (timer > 0f) 

        //{
        //   Debug.Log($"Update:{n} ");
        //}
        if (timer >= 1f) 
        {
           Debug.Log($"Update:{n} ");
            timer = 0f;
            n = 0;
        }
    }
}
