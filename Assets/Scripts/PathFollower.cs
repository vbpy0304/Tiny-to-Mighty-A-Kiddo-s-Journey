using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] pathPoints;
    public float moveSpeed = 2f;

    private int currentIndex = 0;   // Point player is currently on
    private int targetIndex = -1;   // Point player is moving to
    private bool isMoving = false;

    [Header("Free Movement")]
    private Vector3 targetPos;          
    private Camera mainCam;
    private bool freeMoveEnabled = false;

    private Animator animator;
    public int CurrentIndex => currentIndex;

    private void Start()
    {
        animator = GetComponent<Animator>();
        mainCam = Camera.main;

        currentIndex = PlayerPrefs.GetInt("PlayerIndex", 0);

        if (currentIndex < pathPoints.Length)
            transform.position = pathPoints[currentIndex].position;

        if (currentIndex >= pathPoints.Length - 1)
            freeMoveEnabled = true;
    }

    private void Update()
    {
        HandleFreeMovementClick();
        HandleMovement();
    }

    private void HandleFreeMovementClick()
    {
        if (!freeMoveEnabled || !Input.GetMouseButtonDown(0))
            return;

        Vector2 clickPos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // Find all "Plane" tagged objects
        GameObject[] planes = GameObject.FindGameObjectsWithTag("Plane");
        foreach (var plane in planes)
        {
            SpriteRenderer sr = plane.GetComponent<SpriteRenderer>();
            if (sr != null && sr.bounds.Contains(clickPos))
            {
                targetPos = clickPos;
                targetIndex = pathPoints.Length; // Flag for free movement
                isMoving = true;
                return; // Stop after finding the first valid plane
            }
        }
    }

    private void HandleMovement()
    {
        if (!isMoving) return;

        Vector2 moveTarget = (targetIndex < pathPoints.Length) ?
            (Vector2)pathPoints[targetIndex].position : (Vector2)targetPos;

        Vector2 direction = (moveTarget - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
        UpdateAnimationBools(direction);

        if (Vector2.Distance(transform.position, moveTarget) < 0.01f)
        {
            isMoving = false;
            ResetAnimationBools();

            if (targetIndex < pathPoints.Length)
            {
                currentIndex = targetIndex;
                targetIndex = -1;

                Tooltips tip = pathPoints[currentIndex].GetComponent<Tooltips>();
                if (tip != null)
                    tip.ShowTooltip();

                PlayerPrefs.SetInt("PlayerIndex", currentIndex);
                PlayerPrefs.Save();

                if (currentIndex >= pathPoints.Length - 1)
                    freeMoveEnabled = true;
            }
        }
    }

    public void MoveOneStep()
    {
        if (!isMoving && currentIndex + 1 < pathPoints.Length)
        {
            targetIndex = currentIndex + 1;
            targetPos = pathPoints[targetIndex].position;
            isMoving = true;
        }
    }

    private void UpdateAnimationBools(Vector2 direction)
    {
        ResetAnimationBools();

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            animator.SetBool(direction.x > 0 ? "Right" : "Left", true);
        else
            animator.SetBool(direction.y > 0 ? "Up" : "Down", true);
    }

    private void ResetAnimationBools()
    {
        animator.SetBool("Up", false);
        animator.SetBool("Down", false);
        animator.SetBool("Left", false);
        animator.SetBool("Right", false);
    }
}
