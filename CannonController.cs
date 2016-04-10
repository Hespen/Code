using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Assets.Resources.Scripts;
using UnityEngine.UI;

public class CannonController : MonoBehaviour {

    private Vector3 _objectPosition;
    private GameObject _player;
    public Button BoosterButton;
    private bool _launched;

    public GameObject LaunchButton;
    public GameObject Rocket;


    // Use this for initialization
	void Start () {
        _objectPosition = Camera.main.WorldToScreenPoint(transform.position);
	    _player = GameObject.FindGameObjectWithTag("Player");
        BoosterButton.enabled = false;
        BoosterButton.GetComponent<CanvasRenderer>().SetAlpha(0);
        BoosterButton.transform.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(0);


	    if (Stats.Instance.RocketUnlocked == 1)
	    {
	        Rocket.SetActive(true);

	        if (Stats.Instance.TutorialRocket == 0)
	        {
	            Camera.main.GetComponent<InGameTutorialScript>().ActivateTutorial();
	            Camera.main.GetComponent<Tutorial>().Screenshots(new List<int> {3, 4});
                Stats.Instance.TutorialRocket = 1;
                Camera.main.GetComponent<PlayerPrefsManager>().SavePrefs();
	        }
	    }
	}
	
	// Update is called once per frame
	void Update ()
	{
	    CheckInput();
	}

    private void CheckInput()
    {
        if (Input.GetMouseButton(0) && !_launched)
        {
            if (!IsInBoundaries()) return;
            Rotate();
        }
    }

    private bool IsInBoundaries()
    {
        Vector3 mouseLocation = Input.mousePosition;
        return mouseLocation.y >= _objectPosition.y;
    }

    private void Rotate()
    {
        Vector3 mouseLocation = Input.mousePosition;
        mouseLocation.x = mouseLocation.x - _objectPosition.x;
        mouseLocation.y = mouseLocation.y - _objectPosition.y;
        var angle = Mathf.Atan2(mouseLocation.y, mouseLocation.x)*Mathf.Rad2Deg;
        if (angle > 120 || angle < 60) return;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    public void Launch()
    {
        AudioClip clip = (AudioClip)Resources.Load("Music/Cannonshot", typeof(AudioClip));
        GetComponent<AudioSource>().PlayOneShot(clip);
        _player.GetComponent<PlayerMovement>().Launch(transform);
        Destroy(LaunchButton);
        Camera.main.GetComponent<GameManager>().InitiateCounters();
        _launched = true;
        ShowBoosterButton();
        Stats.Instance.LandedInBath = false;
    }

    private void ShowBoosterButton()
    {
        if (Stats.Instance.RocketUnlocked == 1)
        {
            BoosterButton.enabled = true;
            BoosterButton.GetComponent<CanvasRenderer>().SetAlpha(1);
            BoosterButton.transform.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(1);
        }
    }
}
