using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using VRTK;
using MongoDB.Driver;
using MongoDB.Bson;

//=============================================================================----
// Copyright © NaturalPoint, Inc. All Rights Reserved.
// 
// This software is provided by the copyright holders and contributors "as is" and
// any express or implied warranties, including, but not limited to, the implied
// warranties of merchantability and fitness for a particular purpose are disclaimed.
// In no event shall NaturalPoint, Inc. or contributors be liable for any direct,
// indirect, incidental, special, exemplary, or consequential damages
// (including, but not limited to, procurement of substitute goods or services;
// loss of use, data, or profits; or business interruption) however caused
// and on any theory of liability, whether in contract, strict liability,
// or tort (including negligence or otherwise) arising in any way out of
// the use of this software, even if advised of the possibility of such damage.
//=============================================================================----

// Attach Body.cs to an empty Game Object and it will parse and create visual
// game objects based on bone data.  Body.cs is meant to be a simple example 
// of how to parse and display skeletal data in Unity.

// In order to work properly, this class is expecting that you also have instantiated
// another game object and attached the Slip Stream script to it.  Alternatively
// they could be attached to the same object.

public class Body : MonoBehaviour
{

	public GameObject SlipStreamObject;
	GameObject bone = null;
	public MongoClient _client = null;
	public MongoDatabase _database = null;
	public MongoServer _server = null;
	int i = 0;
	private static string DRONEDBNAME = "DroneDB";
	private static string ISCOORDINATES = "IsCoordinates";
	private static string SHOULDCOORDINATES = "ShouldCoordinates";
	private static string DRONECOMMANDS = "DroneCommands";

	// Use this for initialization
	void Start()
	{
		SlipStreamObject.GetComponent<SlipStream>().PacketNotification += new PacketReceivedHandler(OnPacketReceived);
		i = 0;
		try
		{
			_client = new MongoClient();
			_server = _client.GetServer();
			_database = _server.GetDatabase(DRONEDBNAME);

			var collection = _database.GetCollection(ISCOORDINATES);
			collection.Drop();
		}
		catch(Exception)
		{

		}
	}

	// packet received
	void OnPacketReceived(object sender, string Packet)
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(Packet);

		//== rigid bodies ==--

		XmlNodeList rbList = xmlDoc.GetElementsByTagName("RigidBody");

		for(int index = 0; index < rbList.Count; index++)
		{
			int id = System.Convert.ToInt32(rbList[index].Attributes["ID"].InnerText);

			float x = (float)System.Convert.ToDouble(rbList[index].Attributes["x"].InnerText);
			float y = (float)System.Convert.ToDouble(rbList[index].Attributes["y"].InnerText);
			float z = (float)System.Convert.ToDouble(rbList[index].Attributes["z"].InnerText);

			float qx = (float)System.Convert.ToDouble(rbList[index].Attributes["qx"].InnerText);
			float qy = (float)System.Convert.ToDouble(rbList[index].Attributes["qy"].InnerText);
			float qz = (float)System.Convert.ToDouble(rbList[index].Attributes["qz"].InnerText);
			float qw = (float)System.Convert.ToDouble(rbList[index].Attributes["qw"].InnerText);

			//== coordinate system conversion (right to left handed) ==--
			z = -z;
			qz = -qz;
			qw = -qw;

			//== bone pose ==--
			Vector3 position = new Vector3(x, y, z);
			Quaternion orientation = new Quaternion(qx, qy, qz, qw);

			//== locate or create bone object ==--

			string objectName = "Rigidbody" + id.ToString();

			bone = GameObject.Find(objectName);

			//This is, where the new RigidBody is created. Everything we want to change to the rigidbody must happen here.
			if(bone == null)
			{
				//PA_Drone.fbx
				//bone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				//UnityEngine.Object prefab = Resources.Load("PA_SciFiComatants/_Imported3D/Characters/PA_Drone");
				UnityEngine.Object prefab = Resources.Load("PA_Drone");
				
				GameObject t = (GameObject)Instantiate(prefab, new Vector3(0, 0, -6), Quaternion.identity);
				
				//adds databasemanager to the gameobject
				//t.AddComponent<DatabaseManager>();
				t.name = "DroneRepresentation";

				bone = t;

				Vector3 scale = new Vector3(50f, 50f, 50f);
				bone.transform.localScale = scale;
				bone.name = "Rigidbody" + id.ToString();

				Rigidbody gameObjectsRigidBody = bone.AddComponent<Rigidbody>(); // Add the rigidbody.

				gameObjectsRigidBody.mass = 5; // Set the GO's mass to 5 via the Rigidbody.
			}

			//== set bone's pose ==--
			bone.transform.position = position;
			bone.transform.rotation = orientation;

			if(i == 5)
			{
				i = 0;
				//Position Data: 
				Vector3 objPos = bone.transform.position;
				Debug.Log(bone.name + " coordinates [x/y/z]: " + objPos.x.ToString() + " " + objPos.y.ToString() + " " + objPos.z.ToString());

				try
				{
					var document = new BsonDocument
							{
								//Normal Coordinates
								 { "Coordinates", "current" },
								 { "x", objPos.x.ToString() },
								 { "y", objPos.y.ToString() },
								 { "z", objPos.z.ToString() },

								 //Euler Coordinates (Pitch, Yaw, Roll ?)
								 //{ "pitch_x", bone.transform.eulerAngles.x.ToString() },
								 //{ "yaw_y", bone.transform.eulerAngles.y.ToString() },
								 //{ "roll_z", bone.transform.eulerAngles.z.ToString() },

								 //Timestamp
								 { "Timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString()},
							};
					sendData(document, ISCOORDINATES);
				}
				catch(Exception)
				{

				}
			}
			else
			{
				i++;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(bone != null)
		{
			//!!!IMPORTANT !!!
			//The y coordinate is getting sum up, when displaying them here!!!!

			//Vector3 objPos = bone.transform.position;
			//Debug.Log(bone.name + " coordinates [x/y/z]: " + objPos.x.ToString() + " " + objPos.y.ToString() + " " + objPos.z.ToString() + " This is a test");
		}
	}

	private void sendData(BsonDocument _document, String _collection)
	{
		var collection = _database.GetCollection<BsonDocument>(_collection);
		collection.Insert(_document);
	}
}
