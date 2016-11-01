using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace MarsRover
{
	/// <summary>
	/// The main controller class that takes care of the Rovers
	/// </summary>
	public class MissionControl
	{

		#region Private Members

		private string			m_MissionOutcomeMessage;
		private bool			m_MissionSuccess = false;

		private RoverCollection	m_Rovers = null;
		private StringBuilder	m_EventLogs = null;

		private int plateauXDimension = 0;
		private int plateauYDimension = 0;

		#endregion


		#region Constructor

		/// <summary>
		/// Main Constructor
		/// </summary>
		public MissionControl()
		{
			this.m_EventLogs = new StringBuilder(100);
		}

		#endregion


		#region Load Orders

		/// <summary>
		/// Loads the input orders from an external file
		/// </summary>
		/// <param name="inputOrdersFileName"></param>
		/// <returns></returns>
		public bool LoadOrdersFromFile(string inputOrdersFileName)
		{

			bool		plateauSizeFound		= false;
			bool		expectingRoverOrders	= false;

			string		currentInputLine		= string.Empty;
			string[]	currentInputLineParts	= null;

			Rover		constructedRover		= null;
			int			roverIndex				= 0;

            
			if ( !File.Exists(inputOrdersFileName) )
				throw new Exceptions.MissingInputFileException(inputOrdersFileName);

			this.m_Rovers = new RoverCollection();

            StreamReader streamReader = new StreamReader(inputOrdersFileName);

			if ( streamReader.Peek() == -1 )
			{
				throw new Exceptions.EmptyInputFileException(inputOrdersFileName);
			}

			while ( streamReader.Peek() >= 0 )
			{

				// Get the whole input line to parse
				currentInputLine = streamReader.ReadLine();


				//
				// Try to match the input line against known line formats (Plateau size, Rover Location, Rover Orders)
				//

				if ( Regex.IsMatch(currentInputLine, Constants.INPUT_ORDERS_REGEX_COMMENT) )
				{
					// Ignore any comment lines in the input file
				}
				else if ( Regex.IsMatch(currentInputLine, Constants.INPUT_ORDERS_REGEX_ROVER_START_LOCATION) )
				{

					// Current input line is a start location for a rover.
					constructedRover = new Rover();

					// Set the starting location of the rover
					constructedRover.StartLocation = new RoverLocation(currentInputLine);
					constructedRover.Name = string.Format("Rover{0}", roverIndex);

					// The next input line should be a set of orders for the rover
					expectingRoverOrders = true;

				}
				else if ( Regex.IsMatch(currentInputLine, Constants.INPUT_ORDERS_REGEX_ROVER_ORDERS) )
				{

					if ( !expectingRoverOrders )
						throw new Exceptions.OrdersFileParsingException("The orders file does not seem to be in the correct format, after the plateau definition, there shoud be alternating locations and mission order lines.");

					// Current input line is a set of orders for a rover.
					constructedRover.MissionOrders = currentInputLine;

					// Add the completed rover to the collection
					this.m_Rovers.Add(roverIndex, constructedRover);

					this.AddToEventLog(string.Format("Succesfully created Rover No.{0}, using the base location '{1}' and orders '{2}'", roverIndex, constructedRover.StartLocation, constructedRover.MissionOrders));

					constructedRover = null;
					roverIndex++;


					// Don't expect the next line to be a set of orders, if there are more lines, the next one should be a location+direction for a new Rover.
					expectingRoverOrders = false;

				}
				else if ( Regex.IsMatch(currentInputLine, Constants.INPUT_ORDERS_REGEX_PLATEAU_SIZE) )
				{

					// Check we've not already found a plateau size.
					if ( plateauSizeFound )
						throw new Exceptions.OrdersFileParsingException("Apparently two lines determining the plateau size have been detected, you might want to check the input file.");

					// Current input line is the Plateau size
					currentInputLineParts = currentInputLine.Split(' ');

					// TODO: Check the positioning of the x+y co-ords from the input file
					this.plateauXDimension = int.Parse(currentInputLineParts[0]);
					this.plateauYDimension = int.Parse(currentInputLineParts[1]);

					this.AddToEventLog(string.Format("Succesfully loaded in the Plateau details X:{0} Y:{1}", this.plateauXDimension, this.plateauYDimension));

					plateauSizeFound = true;

				}

			}

			streamReader.Close();
			streamReader = null;


			return (this.m_Rovers.Count > 0);
		}

		#endregion


		#region Deploy Rovers

		/// <summary>
		/// Land the rovers on Mars and set each one off.
		/// </summary>
		/// <returns></returns>
		public void DeployRovers()
		{
			
			this.m_MissionSuccess = true;

			Rover rover = null;

			foreach (int roverIndex in m_Rovers.Keys)
			{
				rover = m_Rovers[roverIndex];

				rover.StorePlateauMapInformation(this.plateauXDimension, this.plateauYDimension);
				rover.LandOnPlateau();
				rover.ExecuteOrders();

				AddToEventLog(rover.EventLog);

				if ( rover.CurrentStatus != Rover.RoverStatus.MissionComplete )
				{
					m_MissionSuccess = false;
					m_MissionOutcomeMessage = string.Format("{0} aborted it's mission with the following message: {1}", rover.Name, rover.MissionMessage);
					break;
				}

			}

		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Determine if the mission is a success.
		/// </summary>
		public bool MissionSuccess
		{
			get
			{
				return m_MissionSuccess;
			}
		}


		/// <summary>
		/// Return an string array containing the locations of the landed rovers
		/// </summary>
		public string[] RoverLocations
		{
			get
			{
				string[]	roverLocations	= new string[m_Rovers.Count];
				Rover		rover			= null;

				foreach (int roverIndex in m_Rovers.Keys)
				{
					rover = m_Rovers[roverIndex];

					if ( rover.HasLanded )
						roverLocations[roverIndex] = rover.CurrentLocation.ToString();                  					
					else
						// TODO: Remove this, it's for debugging only
						roverLocations[roverIndex] = string.Format("{0} Hasn't landed yet, no position information avaliable,", rover.Name);

				}

				return roverLocations;
			}
		}


		/// <summary>
		/// Return a message describing the success or problems encoutnered.
		/// </summary>
		public string MissionOutcome
		{
			get
			{
				return m_MissionOutcomeMessage;
			}
		}

		#endregion


		#region Event Log Methods

		/// <summary>
		/// Add an item to the EventLog
		/// </summary>
		/// <param name="message"></param>
		private void AddToEventLog(string message)
		{
            this.m_EventLogs.Append(message);
			this.m_EventLogs.Append(Constants.EVENT_LOG_LINE_BREAK);
		}

		/// <summary>
		/// Add multiple items to the EventLog
		/// </summary>
		/// <param name="messages"></param>
		private void AddToEventLog(string[] messages)
		{
			foreach (string message in messages)
				AddToEventLog(message);

		}

		/// <summary>
		/// Returns the Event Log from the current mission
		/// </summary>
		public string[] EventLog
		{
			get
			{
				return this.m_EventLogs.ToString().Split(Constants.EVENT_LOG_LINE_BREAK);
			}
		}

		#endregion

	}
}
