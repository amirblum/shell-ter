using UnityEngine;

public class CameraShake : MonoBehaviour
{
	[SerializeField] float _maxShakeAmount = 2f;
	private float _shakeAmount;

	protected void Update()
	{
		if (_shakeAmount > 0)
		{
			transform.localPosition = Random.insideUnitSphere * _shakeAmount;
		}
		else
		{
			transform.localPosition = Vector3.zero;
		}
	}

	public void SetShakeAmount(float intensity)
	{
		_shakeAmount = Mathf.Lerp(0f, _maxShakeAmount, intensity * intensity);
	}
}
