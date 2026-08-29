using UnityEngine;
using UnityEngine.EventSystems;

public enum BallColor
{
    White,
    Red,
    Yellow,
    Green,
    Brown,
    Blue,
    Pink,
    Black
}

public class Ball : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int point;
    public int Point { get { return point; } set { point = value; } }

    [SerializeField] private BallColor color;
    public BallColor OutColor { get { return color; } set { color = value; } }

    [SerializeField] private MeshRenderer rd;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(point);
        GameManager.Instance.ShowScoreText(point);

        // ปิดการชนและการมองเห็นเพื่อไม่ให้กระทบกล้องและ Physics
        // ก่อนจะทำลาย Object เพื่อป้องกัน Exception เกิดขึ้นกลางเฟรม
        GetComponent<Collider>().enabled = false;
        if (rd != null) rd.enabled = false;

        Destroy(gameObject);
    }

    void Awake()
    {
        rd = GetComponent<MeshRenderer>();
    }

    public void SetColorAndPoint(BallColor color)
    {
        this.color = color;

        if (rd == null) rd = GetComponent<MeshRenderer>();

        switch (color)
        {
            case BallColor.White:
                point = 0;
                rd.material.color = Color.white;
                break;

            case BallColor.Red:
                point = 1;
                rd.material.color = Color.red;
                break;

            case BallColor.Yellow:
                point = 2;
                rd.material.color = Color.yellow;
                break;

            case BallColor.Green:
                point = 3;
                rd.material.color = Color.green;
                break;

            case BallColor.Brown:
                point = 4;
                // Unity ไม่มี Color.brown จึงสร้าง RGB (Hex #964B00)
                rd.material.color = new Color(0.58f, 0.29f, 0f);
                break;

            case BallColor.Blue:
                point = 5;
                rd.material.color = Color.blue;
                break;

            case BallColor.Pink:
                point = 6;
                // Unity ไม่มี Color.pink จึงสร้าง RGB (Hex #FFC0CB)
                rd.material.color = new Color(1f, 0.75f, 0.79f);
                break;

            case BallColor.Black:
                point = 7;
                rd.material.color = Color.black;
                break;
        }
    }
}