using Microsoft.Data.Sqlite;
using System.Globalization;

string connectionString = @"Data Source=habit-Tracker.db";

using (var connection = new SqliteConnection(connectionString))
{
	connection.Open();
	var tableCmd = connection.CreateCommand();

	tableCmd.CommandText = 
		@"CREATE TABLE IF NOT EXISTS drinking_water(
			Id INTEGER PRIMARY KEY AUTOINCREMENT,
			Date TEXT,
			Quantity INTEGER
		)";

	tableCmd.ExecuteNonQuery();

	connection.Close();
}

GetUserInput();

void GetUserInput()
{
	Console.Clear();
	bool closeApp = false;
	while (closeApp == false)
	{
        Console.WriteLine("\n\nMAIN MENU");
		Console.WriteLine("\nWhat would you like to do?");
        Console.WriteLine("\nType 0 to Close Application");
        Console.WriteLine("Type 1 to View All Records");
        Console.WriteLine("Type 2 to Insert Record");
        Console.WriteLine("Type 3 to Delete Record");
        Console.WriteLine("Type 4 to Update Record");
        Console.WriteLine("-----------------------------\n");

		string command = Console.ReadLine();

		switch (command)
		{
			case "0":
                Console.WriteLine("\nGoodbye!\n");
				closeApp = true;
				break;
			case "1":
				GetAllRecords();
				break;
			case "2":
				Insert();
				break;
			case "3":
				Delete();
				break;
			case "4":
				Update();
				break;
				//default:
				//             Console.WriteLine("\nInvalid Command. Please enter a number form 0 to 4.\n");
				//	break;
		}
	}
}



void GetAllRecords()
{
	Console.Clear();
	using (var connection = new SqliteConnection("Data Source=habit-Tracker.db"))
	{
		connection.Open();
		var tableCmd = connection.CreateCommand();
		tableCmd.CommandText =
			$@"SELECT * FROM drinking_water";

		List<DrinkingWater> tableData = new();

		SqliteDataReader reader = tableCmd.ExecuteReader();

		if (reader.HasRows)
		{
			while (reader.Read())
			{
				tableData.Add(
					new DrinkingWater
					{
						Id = reader.GetInt32(0),
						Date = DateTime.ParseExact(reader.GetString(1), "dd.MM.yy", new CultureInfo("de-DE")),
						Quantity = reader.GetInt32(2)
					}
				);
			}
		}
		else
		{
            Console.WriteLine("No rows found");
        }

		connection.Close();

        Console.WriteLine("--------------------------\n");
		foreach (var dw in tableData)
		{
			Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd.MMMyyyy")} - Quantity: {dw.Quantity}");
		}
		Console.WriteLine("--------------------------\n");
    }
}


void Insert()
{
	string date = GetDateInput();

	int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choise (no decimals allowed). Type 0 to return to main menu.\n\n");

	using (var connection = new SqliteConnection(connectionString))
	{
		connection.Open();
		var tableCmd = connection.CreateCommand();
		tableCmd.CommandText =
			$@"INSERT INTO drinking_water (date,quantity) VALUES('{date}', {quantity})";

		tableCmd.ExecuteNonQuery();

		connection.Close();
	}
}

string GetDateInput()
{
	Console.WriteLine("\n\nPlease insert the date: (Format: dd.mm.yy). Type 0 to return to main menu.\n\n");
	
	string dateInput = Console.ReadLine();

	if (dateInput == "0") GetUserInput();

	return dateInput;
}

int GetNumberInput(string message)
{
    Console.WriteLine(message);

	string numberInputStr = Console.ReadLine();

	if (numberInputStr == "0") GetUserInput();

	int numberInput = Convert.ToInt32(numberInputStr);

	return numberInput;
}

void Delete()
{
	Console.Clear();
	GetAllRecords();

	var recordId = GetNumberInput("\n\nPlease enter the Id of the record you want to delete or type 0 to return to main menu.\n\n");

	string recordInput = Console.ReadLine();

	if (recordInput == "0") GetUserInput();

	using (var connection = new SqliteConnection(connectionString))
	{
		connection.Open();
		var tableCmd = connection.CreateCommand();
		tableCmd.CommandText = 
			$@"DELETE from drinking_water WHERE Id = '{recordId}'";

		int rowCount = tableCmd.ExecuteNonQuery();

		if (rowCount == 0)
		{
            Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
			connection.Close();
			Delete();
        }
	}
    Console.WriteLine($"\n\nRecord with Id {recordId} was deleted.\n\n");
	GetUserInput();
}

void Update()
{
	Console.Clear();
	GetAllRecords();

	var recordId = GetNumberInput("\n\nPlease enter the Id of the record you want to update or type 0 to return to main menu.\n\n");

	using (var connection = new SqliteConnection(connectionString))
	{
		connection.Open();

		var checkCmd = connection.CreateCommand();
		checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE ID = {recordId})";
		int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

		if (checkQuery == 0)
		{
            Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exists.\n\n");
			connection.Close();
			Update();
        }

		string date = GetDateInput();

		int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choise (no decimals allowed).\n\n");

		var tableCmd = connection.CreateCommand();
		tableCmd.CommandText = $@"UPDATE drinking_water SET date = '{date}',quantity = {quantity} WHERE Id = {recordId}";

		tableCmd.ExecuteNonQuery();
		connection.Close();
	}

}