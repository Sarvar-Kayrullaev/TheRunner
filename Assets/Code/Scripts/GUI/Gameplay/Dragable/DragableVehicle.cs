using System.Collections.Generic;
using PlayerRoot;
using UnityEngine;

public class DragableVehicle : MonoBehaviour
{
    public List<RectTransform> HideScreens;
    public List<RectTransform> ShowScreens;

    public RectTransform EnterButton;
    public RectTransform ExitButton;

    public Player player;
    private CarController carController;
    private IndicatorManager indicatorManager;

    void Awake()
    {
        indicatorManager = FindFirstObjectByType<IndicatorManager>();
    }

    public void Register(CarController carController, bool ride)
    {
        if (carController)
        {
            this.carController = carController;
            EnterButton.gameObject.SetActive(ride);
        }
        else
        {
            this.carController = null;
            EnterButton.gameObject.SetActive(ride);
        }
    }

    public void Ride()
    {
        //Switch to car mode
        player.isContollable = false;
        carController.activate = true;
        carController.gameObject.layer = LayerMask.NameToLayer("Vehicle");

        player.transform.parent = carController.playerPosition;
        player.stackCamera.gameObject.SetActive(false);
        player.holster.gameObject.SetActive(false);
        player.Head.localEulerAngles = new(15,0,0);
        player.transform.position = carController.playerPosition.position;
        player.character.enabled = false;
        player.collider.enabled = false;
        player.transform.eulerAngles = carController.playerPosition.eulerAngles;

        //Hide - Show
        foreach (RectTransform screen in HideScreens)
        {
            screen.gameObject.SetActive(false);
        }

        foreach (RectTransform screen in ShowScreens)
        {
            screen.gameObject.SetActive(true);
        }
    }

    public void Disembark()
    {

        //Exit from car mode
        player.isContollable = true;
        carController.activate = false;
        carController.gameObject.layer = LayerMask.NameToLayer("Environments");

        player.stackCamera.gameObject.SetActive(true);
        player.holster.gameObject.SetActive(true);
        player.transform.position = carController.playerOutPosition.position;
        player.transform.eulerAngles = new(0, carController.playerOutPosition.eulerAngles.y, 0);
        player.character.enabled = true;
        player.collider.enabled = true;
        player.transform.parent = null;

        //Hide - Show
        foreach (RectTransform screen in HideScreens)
        {
            screen.gameObject.SetActive(true);
        }

        foreach (RectTransform screen in ShowScreens)
        {
            screen.gameObject.SetActive(false);
        }
    }
}
