using UnityEngine;
using System.Collections;

//Global parameters are stored here...
public class GlobalCore {
	public static int loadingQueue = 0;
	public static string seed="boris";
	public static Planet currentPlanet;
	public static PlanetCharacterController mainController;
	public static Vector3 universeOffset = Vector3.zero;
	public static Transform sun;
}
