using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves Patrollers (and rotates their models) along their patrol Points.
public class Patroller : MonoBehaviour
{
    private const float rotationSlerpAmount = .68f;
    [Header("References")]
    public Transform trans;
    public Transform modelTrans;
    [Header("Stats")]
    public float movespeed = 30;

    private int currentPointIndex;
    private Transform currentPoint;
    private Transform[] patrolPoints;

    // returns the patrol points of this game object
    private List<Transform> GetUnsortedPatrolPoints()
    {
        // get the array of transforms and make it a list
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        var points = new List<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            // identify patrol points by naming convention
            if (children[i].gameObject.name.StartsWith("Point ("))
            {
                points.Add(children[i]);
            }
        }
        return points;
    }

    // informs patrolPoints and sets the current one
    private void SetupPatrolPoints()
    {
        List<Transform> points = GetUnsortedPatrolPoints();
        if (points.Count > 0)
        {
            patrolPoints = new Transform[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                // find the point number in the name and store the point at the corresponding position in points
                int closingParenthesisIndex = points[i].gameObject.name.IndexOf(')');
                string indexSubstring = points[i].gameObject.name.Substring(7, closingParenthesisIndex - 7);
                int patrolPointNumber = System.Convert.ToInt32(indexSubstring);
                patrolPoints[patrolPointNumber] = points[i];
                points[i].SetParent(null); // remove to prevent it from tracking the parent
                points[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            SetCurrentPatrolPoint(0);
        }
    }

    private void SetCurrentPatrolPoint(int index)
    {
        currentPointIndex = index;
        currentPoint = patrolPoints[index];
    }

    void Start()
    {
        SetupPatrolPoints();
    }

    void Update()
    {
        // update the game object transforms
        if (currentPoint != null)
        {
            // advance root game object towards current point
            trans.position = Vector3.MoveTowards(trans.position, currentPoint.position, movespeed * Time.deltaTime);
            if (trans.position == currentPoint.position)
            {
                if (currentPointIndex >= patrolPoints.Length - 1)
                    SetCurrentPatrolPoint(0);
                else
                    SetCurrentPatrolPoint(currentPointIndex + 1);
            }
            else
            {
                // make model game object continue looking towards the current point
                Quaternion lookRotation = Quaternion.LookRotation((currentPoint.position - trans.position).normalized);
                modelTrans.rotation = Quaternion.Slerp(modelTrans.rotation, lookRotation, rotationSlerpAmount);
            }
        }
    }
}
