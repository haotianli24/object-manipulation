using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class CommandProcessor : MonoBehaviour
{
    [SerializeField] private List<GameObject> manipulableObjects;

    private Dictionary<string, System.Func<string[], bool>> commands;

    private void Start()
    {
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        commands = new Dictionary<string, System.Func<string[], bool>>
        {
            {"move", MoveObject},
            {"rotate", RotateObject},
            {"throw", ThrowObject},
            {"push", PushObject}
        };
    }

    public void ProcessCommand(string commandText)
    {
        string[] words = commandText.ToLower().Split(' ');
        string command = words.FirstOrDefault(w => commands.ContainsKey(w));
        Debug.Log("Command: " + command);
        if (command != null && commands[command](words))
        {
            Debug.Log("Command executed successfully: " + commandText);
        }
        else
        {
            Debug.Log("Invalid command or object not found: " + commandText);
        }
    }

    private bool MoveObject(string[] words)
    {
        GameObject sourceObj = FindObject(words, 0);
        if (sourceObj == null)
        {
            Debug.Log("Source object not found");
            return false;
        }

        GameObject targetObj = FindObject(words, 1);
        if (targetObj == null && sourceObj != null)
        {   

            Debug.Log("Test!");
            Vector3 direction = GetDirection(words);
            sourceObj.transform.position += direction * 100;
            Debug.Log("Object moved: " + sourceObj.name);
            return true;
        } else if (targetObj == null) {
            Debug.Log("Target object not found");
            return false;
        }

        Vector3 targetPosition = GetTargetPosition(words, targetObj);
        sourceObj.transform.position = targetPosition;

        Debug.Log($"Moved {sourceObj.name} to position relative to {targetObj.name}");
        return true;
    }

    private Vector3 GetTargetPosition(string[] words, GameObject targetObj)
    {
        Vector3 targetPosition = targetObj.transform.position;
        string relativePosition = GetRelativePosition(words);

        switch (relativePosition)
        {
            case "above":
                targetPosition += Vector3.up * targetObj.GetComponent<Renderer>().bounds.extents.y * 2;
                break;
            case "below":
                targetPosition -= Vector3.up * targetObj.GetComponent<Renderer>().bounds.extents.y;
                break;
            case "to":
                targetPosition = targetObj.transform.position;
                break;
            default:
                // Do nothing, keep the target object's position
                break;
        }

        return targetPosition;
    }

    private string GetRelativePosition(string[] words)
    {
        int secondTheIndex = GetNthIndex(words, "the", 2);
        if (secondTheIndex > 1)
        {
            return words[secondTheIndex - 1];
        }
        return "to"; // Default to "to" if no relative position is specified
    }

    private GameObject FindObject(string[] words, int theOccurrence)
    {
        int theIndex = GetNthIndex(words, "the", theOccurrence + 1);
        if (theIndex != -1 && theIndex < words.Length - 1)
        {
            string objectName = words[theIndex + 1];
            return manipulableObjects.Find(obj => obj.name.ToLower() == objectName);
        }
        return null;
    }

    private int GetNthIndex(string[] words, string target, int n)
    {
        int count = 0;
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] == target)
            {
                count++;
                if (count == n)
                {   
                    return i;
                }
            }
        }
        return -1;
    }

    private bool RotateObject(string[] words)
    {
        GameObject obj = FindObject(words, 0);
        if (obj == null) return false;

        bool isClockwise = words.Contains("clockwise");
        StartCoroutine(RotateObjectSlowly(obj, isClockwise));
        Debug.Log("Object rotating: " + obj.name);
        return true;
    }

    private IEnumerator RotateObjectSlowly(GameObject obj, bool isClockwise)
    {
        Vector3 startRotation = obj.transform.rotation.eulerAngles;
        float targetXRotation = startRotation.x + (isClockwise ? -200f : 200f);
        float targetYRotation = startRotation.y + (isClockwise ? 200f : -200f);
        float rotationTime = 2f; // Time to complete rotation (in seconds)
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            float newXRotation = Mathf.Lerp(startRotation.x, targetXRotation, elapsedTime / rotationTime);
            float newYRotation = Mathf.Lerp(startRotation.y, targetYRotation, elapsedTime / rotationTime);
            obj.transform.rotation = Quaternion.Euler(newXRotation, newYRotation, startRotation.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.rotation = Quaternion.Euler(startRotation); // Return to original rotation
        Debug.Log("Object rotation completed: " + obj.name);
    }






    private bool ThrowObject(string[] words)
    {
        GameObject obj = FindObject(words, 0);
        if (obj == null) return false;

        Vector3 direction = GetDirection(words);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
        }

        // Disable kinematic mode to allow physics simulation
        rb.isKinematic = false;

        // Add upward force to create an arc
        Vector3 throwForce = (direction + Vector3.up).normalized * 50f;
        rb.AddForce(throwForce, ForceMode.Impulse);

        // Add rotation to make it spin
        rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);

        // Add bounce physics
        PhysicMaterial bouncyMaterial = new PhysicMaterial
        {
            bounciness = 0.6f,
            frictionCombine = PhysicMaterialCombine.Multiply
        };
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            collider.material = bouncyMaterial;
        }
        Debug.Log("Object thrown: " + obj.name);
        return true;
    }

    private bool PushObject(string[] words)
    {
        GameObject obj = FindObject(words, 0);
        if (obj == null) return false;

        Vector3 direction = GetDirection(words);
        float force = DeterminePushForce(words);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
            Debug.Log($"Object pushed: {obj.name} with force: {force}");
        }
        return true;
    }

    private float DeterminePushForce(string[] words)
    {
        if (words.Contains("slowly") || words.Contains("gently"))
        {
            return 5f; // Slow push
        }
        else if (words.Contains("fast") || words.Contains("hard"))
        {
            return 30f; // Fast push
        }
        else
        {
            return 10f; // Default medium push
        }
    }


    private Vector3 GetDirection(string[] words)
    {
        if (words.Contains("forward")) return Vector3.forward;
        if (words.Contains("backward")) return Vector3.back;
        if (words.Contains("left")) return Vector3.left;
        if (words.Contains("right")) return Vector3.right;
        if (words.Contains("up")) return Vector3.up;
        if (words.Contains("down")) return Vector3.down;
        return Vector3.zero;
    }

    private Vector3 GetRotation(string[] words)
    {
        if (words.Contains("clockwise")) return Vector3.up * 360f;
        if (words.Contains("counterclockwise")) return Vector3.down * 360f;
        return Vector3.zero;
    }

}
