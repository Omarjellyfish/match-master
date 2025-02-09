using System.Collections;
using UnityEngine;

public class Candy : MonoBehaviour
{
    [Header("Board Variables")]
    public int col;
    public int row;
    private int previousCol;
    private int previousRow;

    private int targetX;
    private int targetY;

    private Board board;
    private GameObject otherCandy;
    private FindMatches findMatches;

    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;

    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public bool isMatched = false;

    void Start()
    {
        // find the board in current scene
        board = FindFirstObjectByType<Board>();
        findMatches = FindFirstObjectByType<FindMatches>();
        // init positions
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //col = targetX;
        //previousRow = row;
        //previousCol = col;
    }

    void Update()
    {
        //FindMatches();

        if (isMatched)
        {
            // remove color and lower opacity to indicate a match
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, .2f);
        }

        // update target positions
        targetX = col;
        targetY = row;

        // nove towards the target X position
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // move towards the target
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, Time.deltaTime * 10f);
            findMatches.FindAllMatches();
        }
        else
        {
            // directly set position
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
        }

        // move towards the target Y position
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // move towards the target
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, Time.deltaTime * 10f);
            findMatches.FindAllMatches();
        }
        else
        {
            // directly set position
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }

        // update only after making sure candies change positions
        if (Mathf.Abs(targetX - transform.position.x) < .1 && Mathf.Abs(targetY - transform.position.y) < .1)
        {
            // ensure correct position of candies on board
            transform.position = new Vector2(targetX, targetY);
            board.allCandies[col, row] = this.gameObject;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.3f);

        if (otherCandy != null)
        {
            // swap back if no match
            if (!isMatched && !otherCandy.GetComponent<Candy>().isMatched)
            {
                SwapCandies(col, row, otherCandy.GetComponent<Candy>().col, otherCandy.GetComponent<Candy>().row);
                yield return new WaitForSeconds(.5f);
                board.state = GameState.move;
            }
            else
            {
                //there is a match, destroy it
                board.DestroyMatches();
            }
            otherCandy = null;
        }
    }

    private void OnMouseDown()
    {
        if (board.state == GameState.wait)
        {
            return;
        }
        // get firsttouchpos
        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        // get finaltouchpos
        if (board.state == GameState.wait)
        {
            return ;
        }
        finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalcAngle();
    }

    void CalcAngle()
    {
        // check if swipes pass the treshold
        if (Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            MovePieces();
            board.state = GameState.wait;
        }
        else
        {
            board.state = GameState.move;
        }
    }

    void MovePieces()
    {
        // Ensure correct swapping direction
        if (swipeAngle > -45 && swipeAngle <= 45 && col < board.width - 1)
        {
            // Right swipe
            SwapCandies(col, row, col + 1, row);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            // Up swipe
            SwapCandies(col, row, col, row + 1);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && col > 0)
        {
            // Left swipe
            SwapCandies(col, row, col - 1, row);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // Down swipe
            SwapCandies(col, row, col, row - 1);
        }
        //start coroutine to ensure move is eligible
        StartCoroutine(CheckMoveCo());
    }

    void SwapCandies(int col1, int row1, int col2, int row2)
    {
        // Update the reference to the other candy
        otherCandy = board.allCandies[col2, row2];

        // Store previous positions for potential swap back
        previousCol = col;
        previousRow = row;

        otherCandy.GetComponent<Candy>().previousCol = otherCandy.GetComponent<Candy>().col;
        otherCandy.GetComponent<Candy>().previousRow = otherCandy.GetComponent<Candy>().row;

        // Swap in the board array
        board.allCandies[col2, row2] = this.gameObject;
        board.allCandies[col1, row1] = otherCandy;

        // Update the candies col and row values
        col = col2;
        row = row2;

        otherCandy.GetComponent<Candy>().col = col1;
        otherCandy.GetComponent<Candy>().row = row1;
    }

    //void FindMatches()
    //{
    //    // Check for horizontal matches
    //    if (col > 0 && col < board.width - 1)
    //    {
    //        GameObject leftCandy = board.allCandies[col - 1, row];
    //        GameObject rightCandy = board.allCandies[col + 1, row];

    //        if (leftCandy != null && rightCandy != null)
    //        {
    //            if (leftCandy.CompareTag(this.gameObject.tag) && rightCandy.CompareTag(this.gameObject.tag))
    //            {
    //                leftCandy.GetComponent<Candy>().isMatched = true;
    //                rightCandy.GetComponent<Candy>().isMatched = true;
    //                isMatched = true;
    //            }
    //        }
    //    }

    //    // Check for vertical matches
    //    if (row > 0 && row < board.height - 1)
    //    {
    //        GameObject upCandy = board.allCandies[col, row + 1];
    //        GameObject downCandy = board.allCandies[col, row - 1];

    //        if (upCandy != null && downCandy != null)
    //        {
    //            if (upCandy.CompareTag(this.gameObject.tag) && downCandy.CompareTag(this.gameObject.tag))
    //            {
    //                upCandy.GetComponent<Candy>().isMatched = true;
    //                downCandy.GetComponent<Candy>().isMatched = true;
    //                isMatched = true;
    //            }
    //        }
    //    }
    //}
}