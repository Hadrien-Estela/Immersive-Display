using UnityEngine;

public class Display: MonoBehaviour
{

	/// <summary>
	/// This must be the physical size of the screen you want to render on.
	/// </summary>
	[SerializeField] private Vector2 size = Vector2.one;

    public float width { get { return (this.size.x); } }
	public float height { get { return (this.size.y); } }
	public Vector3 position { get { return (base.transform.position); } }
	public Quaternion rotation { get { return (base.transform.rotation); } }
	public Vector3 topLeft { get { return (this.position + this.rotation * new Vector2(-width / 2f, height / 2f)); } }
	public Vector3 topRight { get { return (this.position + this.rotation * new Vector2(width / 2f, height / 2f)); } }
	public Vector3 bottomLeft { get { return (this.position + this.rotation * new Vector2(-width / 2f, -height / 2f)); } }
	public Vector3 bottomRight { get { return (this.position + this.rotation * new Vector2(width / 2f, -height / 2f)); } }

	private void OnDrawGizmos ()
	{
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);
	}

}
