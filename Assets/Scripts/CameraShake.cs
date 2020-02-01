using UnityEngine;

public class CameraShake : MonoBehaviour
{
	// How long the object should shake for.
	[SerializeField] float _shakeDuration = 0.3f;
	// Amplitude of the shake. A larger value shakes the camera harder.
	[SerializeField] float _shakeAmount = 2.0f;
	
	private float _shakeDurationRemaining;
	private Vector3 _originalPos;
	
	public void Initialize()
	{
		_originalPos = transform.localPosition;
	}

	protected void Update()
	{
		if (_shakeDurationRemaining > 0)
		{
			transform.localPosition = _originalPos + Random.insideUnitSphere * _shakeAmount;
			_shakeDurationRemaining -= Time.deltaTime;
		}
		else
		{
			_shakeDurationRemaining = 0f;
			transform.localPosition = _originalPos;
		}
	}

	public void Shake()
	{
		_shakeDurationRemaining = _shakeDuration;
	}
}
