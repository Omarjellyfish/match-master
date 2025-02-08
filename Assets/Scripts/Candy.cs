using System.Collections;
using UnityEngine;

public class Candy: MonoBehaviour
{
    [Header("Board Variables")]
    public int col;
    public int row;
    public int previousCol;
    public int previousRow;

    public int targetX;
    public int targetY; 

    private Board board;
    private GameObject otherCandy;

    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public bool isMatched=false;

    void Start()
    {
        board = FindFirstObjectByType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        col = targetX;
        previousRow = row;
        previousCol = col;

    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();
        if (isMatched)
        {
            SpriteRenderer mySprite=GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, .2f);
        }
        targetY = row;
        targetX = col;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //move to the target
            tempPos=new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position,tempPos, Time.deltaTime * 10f);
            if (board.allCandies[col,row]!=this.gameObject)
            {
                board.allCandies[col,row]=this.gameObject;
            }
        }
        else
        {
            //directly set pos 
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position=tempPos;
            if (board.allCandies[col, row] != this.gameObject)
            {
                board.allCandies[col, row] = this.gameObject;
            }
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //move to the target
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, Time.deltaTime * 10f);
        }
        else
        {
            //directly set pos 
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
            board.allCandies[col, row] = this.gameObject;
        }
        Debug.Log(tempPos);

    }
    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.3f);
        if (otherCandy != null) {
            if (!isMatched && !otherCandy.GetComponent<Candy>().isMatched)
            {
                otherCandy.GetComponent<Candy>().row = row;
                otherCandy.GetComponent<Candy>().col = col;
                row = previousRow;
                col = previousCol;
            }
           else
            {
                board.DestroyMatches();
            }
            otherCandy = null;
        }
       
    }
    private void OnMouseDown()
    {
        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
    }
    private void OnMouseUp()
    {
        finalTouchPos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalcAngle();
    }
    void CalcAngle()
    {
        if (Mathf.Abs(finalTouchPos.y - firstTouchPos.y)> swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            //Debug.Log(swipeAngle);
            MovePieces();
        }
        
    }
    void MovePieces()
    {
        if(swipeAngle>-45 &&swipeAngle<=45 &&col<board.width-1)
        {
            //right swipe
            otherCandy = board.allCandies[col + 1, row];
            otherCandy.GetComponent<Candy>().col -= 1;
            col++;
        }else if(swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1)
        {
            //up swipe
            otherCandy = board.allCandies[col, row+1];
            otherCandy.GetComponent<Candy>().row-= 1;
            row++;
        }
        else if ((swipeAngle > 135 || swipeAngle<=-135) && col>0)
        {
            //left swipe 
            otherCandy = board.allCandies[col-1, row];
            otherCandy.GetComponent<Candy>().col += 1;
            col--;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row>0)
        {
            //down swipe
            otherCandy = board.allCandies[col, row - 1];
            otherCandy.GetComponent<Candy>().row += 1;
            row--;
        }
        StartCoroutine(CheckMoveCo());

    }
    void FindMatches()
    {
        if (col > 0 && col < board.width - 1)
        {

            GameObject leftCandy1 = board.allCandies[col - 1, row];
            GameObject rightCandy1 = board.allCandies[col + 1, row];
            if (leftCandy1 != null && rightCandy1 != null)
            {
                if (leftCandy1.CompareTag(this.gameObject.tag) && rightCandy1.CompareTag(this.gameObject.tag))
                {
                    leftCandy1.GetComponent<Candy>().isMatched = true;
                    rightCandy1.GetComponent<Candy>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upCandy1 = board.allCandies[col, row+1];
            GameObject bottomCandy1 = board.allCandies[col, row-1];
            if (upCandy1 != null && bottomCandy1 != null)
            {
                if (upCandy1.CompareTag(this.gameObject.tag) && bottomCandy1.CompareTag(this.gameObject.tag))
                {
                    upCandy1.GetComponent<Candy>().isMatched = true;
                    bottomCandy1.GetComponent<Candy>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
}
