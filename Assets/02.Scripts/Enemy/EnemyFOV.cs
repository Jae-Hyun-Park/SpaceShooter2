using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour {

    public float viewRange = 15.0f;

    [Range(0, 360)]
    public float viewAngle = 120.0f;

    private Transform enemyTr;
    private Transform playerTr;
    private int playerLayer;
    private int obstacleLayer;
    private int layerMask;

    private float offset = 1.5f;

	// Use this for initialization
	void Awake () {

        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        playerLayer = LayerMask.NameToLayer("PLAYER");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        int barrelLayer = LayerMask.NameToLayer("BARREL");
        layerMask = 1 << playerLayer | 1 << obstacleLayer | 1 << barrelLayer;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 startPos = enemyTr.position + Vector3.up * offset;
        Vector3 dir = (playerTr.position - enemyTr.position).normalized;

        Debug.DrawLine(startPos, startPos + enemyTr.forward * 4.0f, Color.red);
        Debug.DrawLine(startPos, startPos + dir * 4.0f, Color.yellow);
    }

    public Vector3 CirclePoint(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool IsTracePlayer()
    {
        bool isTrace = false;

        if (enemyTr == null) return isTrace;
        
        Collider[] colls = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);

        if (colls.Length == 1)
        {
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;

            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }

    public bool IsViewPlayer()
    {
        bool isView = false;

        if (enemyTr == null) return isView;

        Vector3 startPos = enemyTr.position + Vector3.up * offset;

        Collider[] colls = Physics.OverlapSphere(startPos, viewRange, 1 << playerLayer);

        if (colls.Length == 1)
        {
            RaycastHit hit;

            if (Physics.Raycast(startPos, enemyTr.forward, out hit, viewRange, layerMask))
            {
                isView = (hit.collider.CompareTag("Player"));
            }
        }
        return isView;
    }
}
