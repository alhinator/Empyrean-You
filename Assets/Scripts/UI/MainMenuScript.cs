using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour
{
    [Header("Canvases")]
    public Canvas mainMenuCanvas;
    public Canvas worldCanvas;
    public Canvas leftScreen;
    public Canvas rightScreen;
    public Canvas remappingCanvas;

    [Header("Others")]
    public Transform elevator;
    public EventSystem eventSystem;
    public GameObject selectMeAfterElevator;
    // Start is called before the first frame update
    void Start()
    {
        worldCanvas.enabled = false;
        mainMenuCanvas.enabled = true;
        leftScreen.enabled = false;
        rightScreen.enabled = false;
        remappingCanvas.enabled = false;
    }
    public void StartDescent()
    {
        StartCoroutine(DescendToHangar());
        remappingCanvas.enabled = false;
        mainMenuCanvas.enabled = false;
        eventSystem.SetSelectedGameObject(null);
    }
    private IEnumerator DescendToHangar()
    {

        for (float percent = 0; percent <= 1000; percent++)
        {

            elevator.position = new Vector3(elevator.position.x, Mathf.Lerp(300, 1, percent / 1000), elevator.position.z);
            yield return new WaitForSeconds(0.01f);
        }
        worldCanvas.enabled = true;
        worldCanvas.sortingOrder = 2;
        leftScreen.enabled = true;
        eventSystem.SetSelectedGameObject(selectMeAfterElevator);

    }

    public void ToggleRemappingCanvas()
    {
        remappingCanvas.enabled = !remappingCanvas.enabled;
    }



}
