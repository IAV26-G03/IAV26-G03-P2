using UnityEngine;
using UCM.IAV.Movimiento;

public class VisionCone : MonoBehaviour
{
    public Llegada llegada;
    public Transform player;

    public float visionAngle = 90f;
    public float visionDistance = 30f;

    void Awake()
    {
        if (player == null && GameManager.instance != null)
            player = GameManager.instance.GetPlayer().transform;

        if (llegada == null)
            llegada = GetComponent<Llegada>();
    }

    void Update()
    {
        if (player == null || llegada == null) return; 


        if (PuedeVerJugador())
        {
            llegada.SetObjetivo(player.gameObject);
            llegada.enabled = true;
        }
        else
        {
            llegada.enabled = false;
        }
    }

    bool PuedeVerJugador()
    {
        Vector3 dirToPlayer = player.position - transform.position;

        if (dirToPlayer.magnitude > visionDistance)
            return false;

        float angle = Vector3.Angle(transform.TransformDirection(Vector3.forward), dirToPlayer);
        if (angle > visionAngle / 2f)
            return false;

        RaycastHit hit;
        Vector3 origen = transform.position + Vector3.up * 1.5f;
        if (Physics.Raycast(origen, dirToPlayer.normalized, out hit, visionDistance))
        {
            if (hit.transform == player)
                Debug.DrawRay(origen, dirToPlayer.normalized * visionDistance, Color.red);
            return true;
        }
        Debug.DrawRay(origen, dirToPlayer.normalized * visionDistance, Color.red);
        return false;
    }

}