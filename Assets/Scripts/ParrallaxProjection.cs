
using UnityEngine;

/// <summary>
/// This script is for setting up the projection matrix of the non-symmetric camera frustum and compute
/// the off-axis projection matrix used for the eye camera
/// </summary>
[ExecuteInEditMode,
RequireComponent(typeof(Camera)),
DisallowMultipleComponent]
public class ParrallaxProjection: MonoBehaviour
{
	
	[SerializeField] private Display m_display;

	private float left, right, bottom, top, near, far;
	private float displayDistance;

	private Camera m_camera;
	
	/// <summary>
	/// The camera attached to the component.
	/// </summary>
	private Camera Camera
	{
		get
		{
			if (!m_camera)
				m_camera = this.GetComponent<Camera>();
			return (m_camera);
		}
	}

	private Display Display => (m_display);

	private void LateUpdate()
	{
		// Look towards the display
		Quaternion q = this.Display.transform.rotation;
		this.Camera.transform.rotation = q;

		Vector3 relativeDisplayPosition = this.Camera.transform.worldToLocalMatrix.MultiplyPoint(this.Display.transform.position); // find display position in rendering camera's view space.
		Vector3 relativeDisplayForwardVector = this.Camera.transform.worldToLocalMatrix.MultiplyVector(this.Display.transform.forward); // normal of plane defined by device camera.
		Plane displayPlane = new Plane(relativeDisplayForwardVector, relativeDisplayPosition); // The display plane in camera's view space.

		Vector3 close = displayPlane.ClosestPointOnPlane(Vector3.zero);
		near = close.magnitude;
		far = this.Camera.farClipPlane;

		// Relative display corners
		left = relativeDisplayPosition.x - this.Display.width / 2f ;
		right = relativeDisplayPosition.x + this.Display.width / 2f ;
		top = relativeDisplayPosition.y + this.Display.height / 2f ;
		bottom = relativeDisplayPosition.y - this.Display.height / 2f;

		displayDistance = near;

		// move near to 0.01 (1 cm from eye)
		float scale_factor = 0.01f / near;
		near *= scale_factor;
		left *= scale_factor;
		right *= scale_factor;
		top *= scale_factor;
		bottom *= scale_factor;

		Matrix4x4 m = PerspectiveOffCenter(left, right, bottom, top, near, far);
		this.Camera.projectionMatrix = m;
	}

	static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
	{
		float x = 2.0F * near / (right - left);
		float y = 2.0F * near / (top - bottom);
		float a = (right + left) / (right - left);
		float b = (top + bottom) / (top - bottom);
		float c = -(far + near) / (far - near);
		float d = -(2.0F * far * near) / (far - near);
		float e = -1.0F;
		Matrix4x4 m = new Matrix4x4();
		m[0, 0] = x;
		m[0, 1] = 0;
		m[0, 2] = a;
		m[0, 3] = 0;
		m[1, 0] = 0;
		m[1, 1] = y;
		m[1, 2] = b;
		m[1, 3] = 0;
		m[2, 0] = 0;
		m[2, 1] = 0;
		m[2, 2] = c;
		m[2, 3] = d;
		m[3, 0] = 0;
		m[3, 1] = 0;
		m[3, 2] = e;
		m[3, 3] = 0;
		return m;
	}

	private void OnDrawGizmos()
	{
		Vector3[] frustumCorners = new Vector3[4];
		this.Camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), this.displayDistance, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

		for (int i = 0; i < 4; i++)
		{
			var worldSpaceCorner = this.Camera.transform.TransformVector(frustumCorners[i]);
			Debug.DrawRay(this.Camera.transform.position, worldSpaceCorner, Color.yellow);
		}

	}

}