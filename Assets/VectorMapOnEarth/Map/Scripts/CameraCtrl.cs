using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CameraCtrl : MonoBehaviour
{
	public Transform light = null;
	public MeshRenderer earthRenderer = null;
	public MeshRenderer atmosphereRenderer = null;
    public GameObject CameraRotationPoint;
	public GameObject XAxisRotationPoint;

	Quaternion cameraRotation;
	Vector2 targetOffCenter;
	Vector2 offCenter;

	GameObject PreviousSelectedObj = null;

	// Use this for initialization
	void Start()
	{
        targetOffCenter = transform.position;
        offCenter = transform.position;
        cameraRotation = Quaternion.LookRotation(-transform.position.normalized, Vector3.up);
    }

    // Update is called once per frame
    void Update()
	{
		float wheelDelta = Input.GetAxis("Mouse ScrollWheel");
		if (wheelDelta > 0)
		{
			gameObject.GetComponent<Camera>().fieldOfView--;
			if (gameObject.GetComponent<Camera>().fieldOfView < 25)
				gameObject.GetComponent<Camera>().fieldOfView = 25;
		}
		else if (wheelDelta < 0)
		{
			gameObject.GetComponent<Camera>().fieldOfView++;
			if (gameObject.GetComponent<Camera>().fieldOfView > 65)
				gameObject.GetComponent<Camera>().fieldOfView = 65;
		}
		
        float xMove = Input.GetAxis("Mouse X");
		float yMove = Input.GetAxis("Mouse Y");

		if (!RectTransformUtility.RectangleContainsScreenPoint(EarthMapBuilder.instance.SettingsPanel, Input.mousePosition) && !RectTransformUtility.RectangleContainsScreenPoint(EarthMapBuilder.instance.ColorPicker, Input.mousePosition))
		{
			if (Input.GetMouseButton(0))
			{
				const float MOUSE_TRANSLATE_SENSITIVITY = 3;
				CameraRotationPoint.transform.Rotate(0, -xMove * MOUSE_TRANSLATE_SENSITIVITY, 0);
			}
			else if (Input.GetMouseButton(1))
			{
				Quaternion lightRotation = light.rotation;
				lightRotation *= Quaternion.AngleAxis(xMove * 2, Vector3.up);
				light.rotation = lightRotation;
			}
			else if (Input.GetMouseButton(2))
			{
				const float MOUSE_TRANSLATE_SENSITIVITY = 3;
				XAxisRotationPoint.transform.Rotate(yMove * MOUSE_TRANSLATE_SENSITIVITY, 0, 0);
			}
		}

		Vector3 lightDir = Quaternion.Inverse(light.rotation) * Vector3.forward;
		earthRenderer.material.SetVector("_LightDir", lightDir);
		atmosphereRenderer.material.SetVector("_LightDir", lightDir);

	}

}
