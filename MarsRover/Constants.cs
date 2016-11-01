using System;

namespace MarsRover
{

	/// <summary>
	/// Compass directions to determint the current heading of Rovers.
	/// </summary>
	public enum CompassDirection
	{
		North,
		South,
		East,
		West
	}

	/// <summary>
	/// Constants file for the project
	/// </summary>
	public class Constants
	{
	
		/// <summary>Default file name to load the input data from</summary>
		internal const string	DEFAULT_INPUT_FILE_NAME						= "..\\INPUT.TXT";

		/// <summary>The character breaking the key and value in a command line argument (e.g. key=value)</summary>
		internal const char		COMMAND_LINE_ARGUMENT_BREAK_CHAR			= '=';

		/// <summary>The command line argument key to pick up the input file</summary>
		internal const string	COMMAND_LINE_ARGUMENT_INPUT_FILE			= "inputfile";

		/// <summary>Text used to separate specific messages from Event Logs</summary>
		internal const char		EVENT_LOG_LINE_BREAK						= '|';

		/// <summary>Regular Expression match for Comment lines</summary>
		internal const string	INPUT_ORDERS_REGEX_COMMENT					= "[#]";

		/// <summary>Regular Expression match for Plateau Size</summary>
		internal const string	INPUT_ORDERS_REGEX_PLATEAU_SIZE				= "[0-9]+ [0-9]+";

		/// <summary>Regular Expression match for Rover start location</summary>
		internal const string	INPUT_ORDERS_REGEX_ROVER_START_LOCATION		= "[0-9]+ [0-9]+ [NSEW]";

		/// <summary>Regular Expression match for Rover orders</summary>
		internal const string	INPUT_ORDERS_REGEX_ROVER_ORDERS				= "[MLR]+";

		/// <summary>Possible Order Command - Move Forward</summary>
		internal const char		INPUT_ORDER_MOVE							= 'M';

		/// <summary>Possible Order Command - Turn Left</summary>
		internal const char		INPUT_ORDER_LEFT							= 'L';

		/// <summary>Possible Order Command - Turn Right</summary>
		internal const char		INPUT_ORDER_RIGHT							= 'R';

	}
}
