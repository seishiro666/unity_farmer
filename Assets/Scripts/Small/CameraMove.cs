using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Класс, отвечающий за движение камеры
public class CameraMove : MonoBehaviour
{
    float velocityMove = 0.01f; // Скорость движения камеры
    bool isMove = false; // Флаг для движения камеры
    public List<Transform> edges = new List<Transform>(); // Границы движения камеры

    private float minX = -20f; // Минимальная координата X
    private float maxX = 20f; // Максимальная координата X
    private float minZ = -15f; // Минимальная координата Z
    private float maxZ = 20f; // Максимальная координата Z

    [SerializeField] BedController bedController; // Контроллер кровати

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isMove = !isMove; // Переключаем движение камеры
        }

        if (isMove)
        {
            Vector3 newPosition = transform.position; // Получаем текущую позицию камеры

            // Двигаем камеру влево
            if (Input.mousePosition.x < edges[0].position.x && newPosition.x >= minX)
            {
                newPosition.x -= velocityMove;
            }
            // Двигаем камеру вправо
            else if (Input.mousePosition.x > edges[1].position.x && newPosition.x <= maxX)
            {
                newPosition.x += velocityMove;
            }

            // Двигаем камеру вниз
            if (Input.mousePosition.y < edges[2].position.y && newPosition.z >= minZ)
            {
                newPosition.z -= velocityMove;
            }
            // Двигаем камеру вверх
            else if (Input.mousePosition.y > edges[3].position.y && newPosition.z <= maxZ)
            {
                newPosition.z += velocityMove;
            }

            // Устанавливаем новую позицию камеры с ограничениями
            transform.position = new Vector3(Mathf.Clamp(newPosition.x, minX, maxX), newPosition.y, Mathf.Clamp(newPosition.z, minZ, maxZ));
        }

        // Проверяем, находится ли указатель мыши над UI элементами
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Если указатель над UI, выходим из метода
        }

        // Проверяем нажатие левой кнопки мыши
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Создаем луч из камеры
            RaycastHit hit; // Переменная для хранения информации о столкновении

            // Проверяем, попал ли луч в объект
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("BedCol")) // Если объект имеет тег "BedCol"
                {
                    bedController.SetupBed(hit.collider.gameObject); // Настраиваем кровать
                }
            }
        }
    }
}