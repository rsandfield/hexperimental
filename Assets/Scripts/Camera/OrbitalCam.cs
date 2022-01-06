using UnityEngine;
using System.Collections;

public class OrbitalCam : MonoBehaviour {

	public Camera cam;
	public float speed = 1f;
	public float zoom = 1.5f;
    public float minZoom = 0.8f;

	private float pitch = 0f;
	private float yaw = 0f;
	private float radius = 10f;

	// Use this for initialization
	void Start () {
		transform.localPosition = new Vector3 (0, 0, radius);
		cam.transform.localPosition = new Vector3 (0, 0, -radius * zoom);
	}
	
	void Update () {
		float zoomDelta = Input.GetAxis ("Mouse ScrollWheel");
		if (zoomDelta != 0f) {
			AdjustZoom (zoomDelta);
		}

		float xDelta = Input.GetAxis ("Horizontal");
		float yDelta = Input.GetAxis ("Vertical");
		if (xDelta != 0f || yDelta != 0f) {
			AdjustPosition (xDelta, yDelta);
		}
	}

	void AdjustZoom (float delta) {
		zoom = Mathf.Clamp01 (zoom - delta);
        if(zoom < minZoom) zoom = minZoom;

		float distance = Mathf.Lerp (radius * 0.01f, radius * 1.5f, zoom);
		cam.transform.localPosition = new Vector3 (0f, 0f, -distance);
	}

	void AdjustPosition (float xDelta, float zDelta) {
		pitch += zDelta * speed;
		if (pitch > 90) pitch = 90;
		if (pitch < -90) pitch = -90;
		yaw -= xDelta * speed * (Mathf.Cos (pitch * Mathf.PI / 180) / 2 + 0.5f);

		transform.eulerAngles = new Vector3 (pitch, yaw, 0f);
		transform.position = transform.rotation * Vector3.forward * radius;
	}
}
