using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMath : MonoBehaviour
{
	public static Vector2 RadianToVector2(float radian)
	{
		return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
	}

	public static Vector2 DegreeToVector2(float degree)
	{
		return RadianToVector2(degree * Mathf.Deg2Rad);
	}

	public double FindAngle(Vector3 vec)
	{
		float x = vec.x;
		float y = vec.y;
		double angle = Mathf.Atan(y / x);
		angle = (angle / Mathf.PI) * 180.0;

		if (x < 0 && y > 0)
		{
			angle = -angle;
			angle = 180 - angle;
		}
		else if (x < 0 && y < 0)
		{
			angle = 180 + angle;
		}
		else if (x > 0 && y < 0)
		{
			angle = -angle;
			angle = 360 - angle;
		}
		return angle;
	}
}
