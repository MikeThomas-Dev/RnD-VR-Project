using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;


public class BtnScript : MonoBehaviour {

	public Button Button_TakeOff;
	public Button Button_Land;
	public Text errorText;

	MongoClient _client;
	MongoDatabase _database;
	MongoServer _server;

	// Use this for initialization
	void Start ()
	{
		Button_TakeOff.enabled = true;
		Button_Land.enabled = false;
		try
		{
			_client = new MongoClient();
			_server = _client.GetServer();
			_database = _server.GetDatabase("DroneDB");
		}
		catch(Exception)
		{
			errorText.text = "Error: MongoDB connection might be down";
			Button_TakeOff.enabled = false;
			Button_Land.enabled = false;
		}
	}

	public void takeoff()
	{
		try
		{
			var document = new BsonDocument
							{
								 { "Command", "takeoff" },
								 { "Timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString()},
							};

			Debug.Log("Button Takeoff works");
			sendData(_database, document);
			Button_TakeOff.enabled = false;
			Button_Land.enabled = true;
		}
		catch(Exception)
		{
			errorText.text = "Error: Please check if there is connection to the drone!";
			Button_TakeOff.enabled = false;
			Button_Land.enabled = false;
		}		
	}

	public void land()
	{
		try
		{
			var document2 = new BsonDocument
							{
								 { "Command", "land" },
								 { "Timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString()},
							};
			sendData(_database, document2);
			Debug.Log("Button Land works");
			Button_TakeOff.enabled = true;
			Button_Land.enabled = false;
		}
		catch(Exception)
		{
			errorText.text = "Error: Please check if there is connection to the drone!";
			Button_TakeOff.enabled = false;
			Button_Land.enabled = false;
		}
	}

	// Update is called once per frame
	void Update ()
	{

	}

	private void sendData(MongoDatabase _database, BsonDocument _document)
	{
		var collection = _database.GetCollection<BsonDocument>("DroneCommands");
		collection.Insert(_document);
	}
}
