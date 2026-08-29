using UnityEngine;

public class Hole : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Ball b = other.GetComponent<Ball>();

        if (b != null)
        {
            bool isDestroy = false;
            
            Destroy(b.gameObject );
            isDestroy = true;

            if (b.Point == 0 && isDestroy) 
            {
                GameManager.Instance.SetBall(b.OutColor, 0);
                GameManager.Instance.ShowString(" DIE ",true);
                Time.timeScale = 0f ;
                return;
            }
            GameManager.Instance.ShowScoreText(b.Point);
            ;


        }
    }
}
