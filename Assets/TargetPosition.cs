using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

public class TargetDronePosition : MonoBehaviour
{
	MongoClient _client;
	MongoDatabase _database;
	MongoServer _server;
	int i;
	GameObject drone;

	// Use this for initialization
	void Start()
	{
		i = 0;
		try
		{
			drone = this.gameObject;
			//_client = new MongoClient();
			//_server = _client.GetServer();
			//_database = _server.GetDatabase("DroneDB");
		}
		catch(Exception)
		{

		}
	}

	// Update is called once per frame
	void Update()
	{
		if(i == 50) //Basically means, every 50 frames.
		{
			i = 0;
			//int id = System.Convert.ToInt32(rbList[index].Attributes["ID"].InnerText);

			//float x = (float)System.Convert.ToDouble(rbList[index].Attributes["x"].InnerText);
			//float y = (float)System.Convert.ToDouble(rbList[index].Attributes["y"].InnerText);
			//float z = (float)System.Convert.ToDouble(rbList[index].Attributes["z"].InnerText);

			//float qx = (float)System.Convert.ToDouble(rbList[index].Attributes["qx"].InnerText);
			//float qy = (float)System.Convert.ToDouble(rbList[index].Attributes["qy"].InnerText);
			//float qz = (float)System.Convert.ToDouble(rbList[index].Attributes["qz"].InnerText);
			//float qw = (float)System.Convert.ToDouble(rbList[index].Attributes["qw"].InnerText);

			////== coordinate system conversion (right to left handed) ==--
			//z = -z;
			//qz = -qz;
			//qw = -qw;

			////== bone pose ==--
			//Vector3 position = new Vector3(x, y, z);
			//Quaternion orientation = new Quaternion(qx, qy, qz, qw);

			////== set bone's pose ==--
			//drone.transform.position = position;
			//drone.transform.rotation = orientation;

			//Position Data: 
			Vector3 objPos = drone.transform.position;
			Debug.Log(drone.name + " coordinates [x/y/z]: " + objPos.x.ToString() + " " + objPos.y.ToString() + " " + objPos.z.ToString());

			try
			{
				var document = new BsonDocument
							{
								//Normal Coordinates
								 { "Coordinates", "target" },
								 { "x", objPos.x.ToString() },
								 { "y", objPos.y.ToString() },
								 { "z", objPos.z.ToString() },

								 //Euler Coordinates (Pitch, Yaw, Roll ?)
								 { "pitch_x", drone.transform.eulerAngles.x.ToString() },
								 { "yaw_y", drone.transform.eulerAngles.y.ToString() },
								 { "roll_z", drone.transform.eulerAngles.z.ToString() },

								 //Timestamp
								 { "Timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString()},
							};
				sendData(document, "TargetCoordinates");
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

	private void sendData(BsonDocument _document, String _collection)
	{
		var collection = _database.GetCollection<BsonDocument>(_collection);
		collection.Insert(_document);
	}
}
