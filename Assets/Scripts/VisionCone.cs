using UnityEngine;
using UCM.IAV.Movimiento;

public class VisionCone : MonoBehaviour
{
    public Llegada llegada;
    public Transform player;

    [SerializeField] private MinoRotacion rotacion;
    [SerializeField] private MinoPatrulla patrulla;

    [SerializeField] private float visionAngle = 30f;
    [SerializeField] private float visionDistance = 5f;

    private void Awake()
    {
        if (player == null && GameManager.instance != null)
            player = GameManager.instance.GetPlayer().transform;

        if (llegada == null)
            llegada = GetComponent<Llegada>();

        // Serializamos rotacion/patrulla: si tienes uno de los dos, se asigna automáticamente
        if (rotacion == null)
            rotacion = GetComponent<MinoRotacion>();
        if (patrulla == null)
            patrulla = GetComponent<MinoPatrulla>();
    }

    void Update()
    {
        if (player == null || llegada == null) return;

        bool veJugador = PuedeVerJugador(); // Llama a tu función y guarda el resultado

        // Debug rápido
        if (veJugador)
        {
            Debug.Log($"{name} ve al jugador");
        }
        else
        {
            Debug.Log($"{name} NO ve al jugador");
        }
        if (PuedeVerJugador())
        {
            llegada.SetObjetivo(player.gameObject);
            llegada.enabled = true;

            if (rotacion != null)
                rotacion.enabled = false;
            if (patrulla != null)
                patrulla.enabled = false;
        }
        else
        {
            llegada.enabled = false;

            if (rotacion != null)
                rotacion.enabled = true;
            if (patrulla != null)
                patrulla.enabled = true;
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