using System;
using System.IO;

namespace MarsRover
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class MardRoverConsole
	{
		private static string inputFileName;
        

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{

			try
			{
				HandledEntryPoint(args);
			}
			catch (Exception ex)
			{
				DumpToConsoleOutput("---- This application is badly written, an unexpected exception has occured ----");
				DumpToConsoleOutput(ex.ToString());
			}

			PauseConsole();
		}


		/// <summary>
		/// 
		/// </summary>
		private static void HandledEntryPoint(string[] args)
		{

			// Set the default values for the application
			inputFileName = Constants.DEFAULT_INPUT_FILE_NAME;

			
			// Process the command line srguments (input file name, etc)
			string[] commandLineParts;
			string commandLineKey;
			string commandLineValue;

			foreach (string commandLineArgument in args)
			{
				if ( commandLineArgument.IndexOf(Constants.COMMAND_LINE_ARGUMENT_BREAK_CHAR) > -1 )
				{

					// Break up the command line argument into the 'key' and 'value'
					commandLineParts = commandLineArgument.Split(Constants.COMMAND_LINE_ARGUMENT_BREAK_CHAR);
					commandLineKey = commandLineParts[0];
					commandLineValue = commandLineParts[1];
					
					switch ( commandLineKey.ToLower() )
					{
						case Constants.COMMAND_LINE_ARGUMENT_INPUT_FILE :
						{
							inputFileName = commandLineValue;
							break;
						}
					}

				}
				else
				{
					throw new Exceptions.CommandLineException(string.Format("The command line argument '{0}' doesn't match the structure 'key{1}value'.", commandLineArgument, Constants.COMMAND_LINE_ARGUMENT_BREAK_CHAR));
				}
			}


			// Load the input file into memory
			inputFileName = Path.GetFullPath(inputFileName);

			if ( !File.Exists(inputFileName) )
				throw new Exceptions.MissingInputFileException(inputFileName);

			// Init the MissionControl object
			MissionControl missionControl = new MissionControl();


			// Setup the 'Theatre of operations'
			DumpToConsoleOutput( string.Format("Loading program input from '{0}'", inputFileName) );
			missionControl.LoadOrdersFromFile(inputFileName);

			// Start the mission
			missionControl.DeployRovers();

#if DEBUG
			DumpToConsoleOutput("----Normal Program Output----");
#endif


			// Check for mission success
			if ( missionControl.MissionSuccess )
			{
				// SUCCESS: Show the requested program output.
				DumpToConsoleOutput( "The mission was a success! Program output as follows:" );
				DumpToConsoleOutput( missionControl.RoverLocations );
			}

			else
			{
				// FAILED: Show the reason why
				DumpToConsoleOutput( string.Format("The mission was a failure: {0}", missionControl.MissionOutcome) );
			}

#if DEBUG
			DumpToConsoleOutput("----Mission Control Event Log----");
			DumpToConsoleOutput( missionControl.EventLog );
#endif

		}


		/// <summary>
		/// Pause thread execution untill the user hits the 'ENTER' key
		/// </summary>
		private static void PauseConsole()
		{
			System.Console.WriteLine("Press ENTER to continue...");
			System.Console.Read();
		}


		/// <summary>
		/// Display a message to the output console
		/// </summary>
		/// <param name="outputMessages"></param>
		private static void DumpToConsoleOutput(string[] outputMessages)
		{
			foreach (string outputMessage in outputMessages)
			{
				Console.WriteLine(outputMessage);
			}
		}

		/// <summary>
		/// Display a message to the output console
		/// </summary>
		/// <param name="outputMessage">Single output message</param>
		private static void DumpToConsoleOutput(string outputMessage)
		{
			Console.WriteLine(outputMessage);
		}

		/// <summary>
		/// Display a message to the output console
		/// </summary>
		/// <param name="outputObject">Object from which .ToString() will be called</param>
		private static void DumpToConsoleOutput(object outputObject)
		{
			Console.WriteLine(outputObject.ToString());
		}

	}
}
