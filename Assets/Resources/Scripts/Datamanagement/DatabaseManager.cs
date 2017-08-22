using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using VRTK;

public class DatabaseManager : MonoBehaviour
{

	#region ### PUBLIC PROPERTIES ###
	public VRTK_ControllerEvents controller;
	#endregion

	#region ### PRIVATE PROPERTIES ###
	private MongoClient _client = null;
	private MongoDatabase _database = null;
	private MongoServer _server = null;
	private Vector3 _objectPosition = new Vector3();
	private int i = 0;
	private bool triggerButtonDown = false;
	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private bool droneLanded = true;
	private bool isEmergency = false;

	private static string DRONEDBNAME = "DroneDB";
	private static string ISCOORDINATES = "IsCoordinates";
	private static string SHOULDCOORDINATES = "ShouldCoordinates";
	private static string DRONECOMMANDS = "DroneCommands";
	#endregion

	#region ### PUBLIC METHODS ###

	#endregion

	#region ### PRIVATE METHODS ###
	private void sendData(BsonDocument _document, String _collection)
	{
		var collection = _database.GetCollection<BsonDocument>(_collection);
		collection.Insert(_document);
	}

	#endregion

	#region ### UNITY METHODS ###
	// Use this for initialization
	void Start()
	{
		try
		{
			_client = new MongoClient();
			_server = _client.GetServer();
			_database = _server.GetDatabase(DRONEDBNAME);

			MongoCollection collection = _database.GetCollection(SHOULDCOORDINATES);
			collection.Drop();

			collection = _database.GetCollection(ISCOORDINATES);
			collection.Drop();
		}
		catch(Exception ex)
		{
			Debug.LogError("Error on DB components init. " + ex.Message);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(i < 5)
		{
			i++;
		}
		else
		{
			i = 0;
			//Wenn "Dead-Man Button" gedrückt ist, dann weiter machen
			//Wenn "Dead-Man Button" NICHT gedrückt ist und flag "droneLanded" false ist, dann EMERGENCY message senden.
			if(controller == null)
			{
				triggerButtonDown = false;
			}
			else
			{
				triggerButtonDown = controller.triggerPressed;  //Des geht :D
			}


			if(triggerButtonDown == false && droneLanded == false && isEmergency == false)
			{
				Debug.Log("Left Trigger Button losgelassen");
				isEmergency = true;
				emergency();    //EMERGENCY message senden
			}

			if(triggerButtonDown == true)
			{
				Debug.Log("Left Trigger Button pressed");
				_objectPosition = this.gameObject.transform.position;

				//Wenn y-Position kleiner als 0.4 ist und flag "droneLanded" false ist, landen und flag true setzen
				if(_objectPosition.y <= 0.4f && droneLanded == false)
				{
					droneLanded = true;
					Debug.Log("Drone is landing...");
					land();
					return;
				}

				//wenn y-Position kleiner als 0.4 ist und flag "droneLanded" true ist, nichts machen (Drone am Boden)

				//Wenn y-Position größer als 0.4 ist und flag "droneLanded" true ist, starten.
				if(_objectPosition.y > 0.4f && droneLanded == true)
				{
					droneLanded = false;
					Debug.Log("Drone is taking off...");
					//Hier nochmal alle Datenbank Einträge löschen
					resetDatabase();
					takeoff();
					return;
				}

				//Wenn y-Position größer als 0.4 ist und flag "droneLanded" false ist, Positionregelung durchführen.
				if(_objectPosition.y > 0.4f && droneLanded == false)
				{
					////Wenn x kleiner als -2 ist, dann -2 setzen
					//if(objPos.x < -2f)
					//{
					//	objPos.x = -2f;
					//}

					////Wenn x größer als 2 ist, dann 2 setzen
					//if(objPos.x > 2f)
					//{
					//	objPos.x = 2f;
					//}

					////Wenn z kleiner als -2 ist, dann -2 setzen
					//if(objPos.z < -2f)
					//{
					//	objPos.z = -2f;
					//}

					////Wenn z größer als 2 ist, dann 2 setzen
					//if(objPos.z > 2f)
					//{
					//	objPos.z = 2f;
					//}

					////Wenn y größer als 2 ist dann 2 setzen
					//if(objPos.y > 2f)
					//{
					//	objPos.y = 2f;
					//}

					BsonDocument doc = new BsonDocument
							{
								{ "Coordinates", "should" },
								{ "x", _objectPosition.x.ToString() },
								{ "y", _objectPosition.y.ToString() },
								{ "z", _objectPosition.z.ToString() },

								//Timestamp
								{ "Timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString()}
							};

					if(doc == null)
					{
						return;
					}

					sendData(doc, SHOULDCOORDINATES);
				}
			}
		}
	}

	private void emergency()
	{
		try
		{
			BsonDocument document = new BsonDocument
							{
								 { "Command", "emergency" },
								 { "Timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString()},
							};

			Debug.Log("Button Takeoff works");
			sendData(document, DRONECOMMANDS);
		}
		catch(Exception)
		{

		}
	}

	private void takeoff()
	{
		try
		{
			BsonDocument document = new BsonDocument
							{
								 { "Command", "takeoff" },
								 { "Timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString()},
							};

			Debug.Log("Takeoff works");
			sendData(document, DRONECOMMANDS);

		}
		catch(Exception)
		{

		}
	}

	private void land()
	{
		try
		{
			BsonDocument document = new BsonDocument
							{
								 { "Command", "land" },
								 { "Timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString()},
							};
			Debug.Log("Land works");
			sendData(document, DRONECOMMANDS);
		}
		catch(Exception)
		{

		}
	}

	private void resetDatabase()
	{
		if(_database != null)
		{
			_database.DropCollection(ISCOORDINATES);
			_database.DropCollection(SHOULDCOORDINATES);
			_database.DropCollection(DRONECOMMANDS);
		}
	}
	#endregion
}
