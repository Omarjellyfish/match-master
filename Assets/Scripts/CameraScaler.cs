using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float aspectRatio = 0.625f;
    public float padding = 2;

    void Start()
    {
        board = FindFirstObjectByType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width-1, board.height-1);
        }
    }
    void RepositionCamera(float x, float y)
    {
        Vector3 tempPos = new Vector3(x / 2, y / 2, cameraOffset);
        transform.position = tempPos;
        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize=(board.height / 2 + padding);
        }
    }
}
