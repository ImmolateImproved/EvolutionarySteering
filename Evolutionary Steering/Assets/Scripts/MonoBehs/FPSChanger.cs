using UnityEngine;

public class FPSChanger : MonoBehaviour
{
    [Range(10, 300)]
    public int FPS = 60;

    private void Update()
    {
        Application.targetFrameRate = FPS;
    }
}