using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using System;

public class DronePos : MonoBehaviour
{

	public MongoClient Client = null;
	public MongoDatabase Database = null;
	public MongoServer Server = null;
	private int dataSampleCount = int.MinValue;

	// Use this for initialization
	void Start()
	{
		//initDatabaseConnection();
	}

	internal void initDatabaseConnection()
	{
		//try
		//{
		//	dataSampleCount = 0;
		//	Client = new MongoClient();
		//	Server = Client.GetServer();
		//	Database = Server.GetDatabase("DroneDB");

		//	var collection = Database.GetCollection("DroneSollPosition");
		//	collection.Drop();
		//}
		//catch(Exception ex)
		//{
		//	Debug.Log("Error on establishing database connection! " + ex.Message);
		//}
	}

	// Update is called once per frame
	void Update()
	{
	//	if(dataSampleCount >= 50)
	//	{
	//		dataSampleCount = 0;

	//		Vector3 XYZCords = this.transform.position;
	//		var EulerCords = this.transform.eulerAngles;

	//		try
	//		{
	//			var document = new BsonDocument
	//						{
	//							//Normal Coordinates
	//							 { "Coordinates", "current" },
	//							 { "x", XYZCords.x.ToString() },
	//							 { "y", XYZCords.y.ToString() },
	//							 { "z", XYZCords.z.ToString() },

	//							 //Euler Coordinates (Pitch, Yaw, Roll ?)
	//							 { "pitch_x", EulerCords.x.ToString() },
	//							 { "yaw_y", EulerCords.y.ToString() },
	//							 { "roll_z", EulerCords.z.ToString() },

	//							 //Timestamp
	//							 { "Timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString()},
	//						};
	//			sendData(document, "DroneSollPosition");
	//		}
	//		catch(Exception ex)
	//		{
	//			Debug.Log("Error on sending Soll position! " + ex.Message);
	//		}
	//	}
	//	dataSampleCount++;
	}

	internal void sendData(BsonDocument _document, String _collection)
	{
		//var collection = Database.GetCollection<BsonDocument>(_collection);
		//collection.Insert(_document);
		//Debug.Log("Sent data!");
	}
}
