using System;
using System.Text;

namespace MarsRover
{
	/// <summary>
	/// Summary description for Rover.
	/// </summary>
	public class Rover
	{

		/// <summary>
		/// Current Mission Status of a Rover
		/// </summary>
		public enum RoverStatus
		{
			/// <summary>Rover is currently under construction</summary>
			InConstruction,
			/// <summary>Rover is ready to be deployed on planet surface</summary>
			WaitingForDeployment,
			/// <summary>Rover is currently active on planet surface</summary>
			Roving,
			/// <summary>Mission orders have been completed</summary>
			MissionComplete,
			/// <summary>Mission went wrong somewhere</summary>
			MissionAborted
		}


		private RoverStatus		m_currentStatus;
		private RoverLocation	m_currentLocation;
		private RoverLocation	m_startingLocation;
		private string			m_MissionOrders;
		private string			m_reasonForAbort;
		private string			m_Name;
		private int				m_plateauXDimension;
		private int				m_plateauYDimension;
		private StringBuilder	m_EventLogs;


		/// <summary>
		/// Main Constructor
		/// </summary>
		public Rover()
		{
			m_currentStatus = RoverStatus.InConstruction;
			m_reasonForAbort = string.Empty;
			m_EventLogs = new StringBuilder(30);
		}

		/// <summary>
		/// Tell the rover that it has landed and is about to start wondering about.
		/// </summary>
		/// <returns></returns>
		public bool LandOnPlateau()
		{

			if ( m_plateauXDimension == 0 || m_plateauYDimension == 0 )
				throw new Exceptions.NotReadyForLandingException("This Rover doesn't know the size of the plateau to land on.");

			// Set the current location to the pre-programmed starting location
			m_currentLocation = m_startingLocation.Clone();

			// Update the status of the rover
			m_currentStatus = RoverStatus.Roving;

			return true;

		}

		/// <summary>
		/// Start executing the Mission Orders
		/// </summary>
		public bool ExecuteOrders()
		{

			// Assumptions: The Mission Orders have already been parsed by Regex to only contain 'M', 'L' or 'R' characters.

			bool			orderCarriedOut				= false;
			RoverLocation	previousRoverLocation		= null;

			// Log the start of the mission
			AddToEventLog(string.Format("{0}: Starting Mission", Name));

			foreach ( char currentOrder in this.m_MissionOrders.ToCharArray() )
			{

				// Remember the current location, for comparison with the new location.
				previousRoverLocation = m_currentLocation.Clone();

				// Log the next order
				AddToEventLog(string.Format("{0}: Processing Order:{1}", Name, currentOrder));

				switch (currentOrder)
				{
					case Constants.INPUT_ORDER_MOVE :
						orderCarriedOut = MoveForward();
						break;

					case Constants.INPUT_ORDER_LEFT :
						orderCarriedOut = TurnLeft();
						break;

					case Constants.INPUT_ORDER_RIGHT :
						orderCarriedOut = TurnRight();
						break;

					default:
						throw new Exceptions.UnknownRoverOrderException(currentOrder);

				}

				// If an order was unsuccesful, stop processing further mission orders.
				if ( !orderCarriedOut )
				{
					AddToEventLog(string.Format("{0}: Mission Failed!", Name));
					return false;
				}

				// Log the succesful movement
				AddToEventLog(string.Format("{0}: Succesfully changed position from '{1}' to '{2}'", Name, previousRoverLocation, m_currentLocation));

			}

			// Nothing has gone wrong, mark the mission as a success
			AddToEventLog(string.Format("{0}: Mission Completed!", Name));
			m_currentStatus = RoverStatus.MissionComplete;
			return true;
		}


		/// <summary>
		/// Turns the rover 1/4 turn to the left (Anti-clockwise)
		/// </summary>
		/// <returns>True, if moved succesfully</returns>
		private bool TurnLeft()
		{

			switch (m_currentLocation.Heading)
			{
				case CompassDirection.North :
					m_currentLocation.Heading = CompassDirection.West;
					break;

				case CompassDirection.East :
					m_currentLocation.Heading = CompassDirection.North;
					break;

				case CompassDirection.South :
					m_currentLocation.Heading = CompassDirection.East;
					break;

				case CompassDirection.West :
					m_currentLocation.Heading = CompassDirection.South;
					break;

			}

			return true;
		}


		/// <summary>
		/// Turns the rover 1/4 turn to the right (Clockwise)
		/// </summary>
		/// <returns>True, if moved succesfully</returns>
		private bool TurnRight()
		{

			switch (m_currentLocation.Heading)
			{
				case CompassDirection.North :
					m_currentLocation.Heading = CompassDirection.East;
					break;

				case CompassDirection.East :
					m_currentLocation.Heading = CompassDirection.South;
					break;

				case CompassDirection.South :
					m_currentLocation.Heading = CompassDirection.West;
					break;

				case CompassDirection.West :
					m_currentLocation.Heading = CompassDirection.North;
					break;

			}

			return true;
		}


		/// <summary>
		/// Try to move 1 step forward
		/// </summary>
		/// <returns>True, if moved succesfully</returns>
		private bool MoveForward()
		{

			// Check we're not currently facing the edge of the Plateau
			if ( CheckForPlateauEdge() )
			{
				m_currentStatus = RoverStatus.MissionAborted;
				m_reasonForAbort = "The edge of the plateau has been detected, I can not move another step forward.";
				AddToEventLog(string.Format("{0}: {1}", Name, m_reasonForAbort));
				return false;
			}


			// Check to see if there are any obsticals in the way (such as another rover)
			if ( CheckForObstacles() )
			{
				m_currentStatus = RoverStatus.MissionAborted;
				m_reasonForAbort = "The edge of the plateau has been detected, I can not move another step forward.";
				AddToEventLog(string.Format("{0}: {1}", Name, m_reasonForAbort));
				return false;
			}


			// Move the Rover 1 step forward - in the correct direction.
			switch (m_currentLocation.Heading)
			{
				case CompassDirection.North :
					m_currentLocation.YPosition += 1;
					break;

				case CompassDirection.South :
					m_currentLocation.YPosition -= 1;
					break;

				case CompassDirection.East :
					m_currentLocation.XPosition += 1;
					break;

				case CompassDirection.West :
					m_currentLocation.XPosition -= 1;
					break;

			}
           
			return true;
		}


		/// <summary>
		/// The Original Mission orders for the rover
		/// </summary>
		/// <remarks>
		/// Orders can only be set before the mission has started
		/// </remarks>
		public string MissionOrders
		{
			set
			{

				if ( HasLanded )
					throw new Exceptions.RoverConstructionException("Rover can not accept new mission orders once it has landed");

				// Save the mission orders
				this.m_MissionOrders = value;
				CheckForOperationalReadyness();

			}
			get
			{
				return this.m_MissionOrders;
			}

		}


		/// <summary>
		/// Remember the dimensions of the landing plateau
		/// </summary>
		/// <param name="PlateauXDimension"></param>
		/// <param name="PlateauYDimension"></param>
		public void StorePlateauMapInformation(int PlateauXDimension, int PlateauYDimension)
		{
            this.m_plateauXDimension = PlateauXDimension;
			this.m_plateauYDimension = PlateauYDimension;
		}


		/// <summary>
		/// The original starting location before any orders have been carried out
		/// </summary>
		/// <remarks>
		/// Orders can only be set before the mission has started
		/// </remarks>
		public RoverLocation StartLocation
		{
			set
			{
				if ( HasLanded )
					throw new Exceptions.RoverConstructionException("Rover can not accept new mission orders once it has landed");

				this.m_startingLocation = value;
				CheckForOperationalReadyness();
			}
			get
			{
				return this.m_startingLocation;
			}
		}

		/// <summary>
		/// Returns the current location on the 
		/// </summary>
		/// <returns></returns>
		public RoverLocation CurrentLocation
		{
			get { return m_currentLocation; }
		}


		/// <summary>
		/// Current status of the Rovers mission
		/// </summary>
		public RoverStatus CurrentStatus
		{
			get { return this.m_currentStatus; }
		}


		/// <summary>
		/// Friendly name of the Rover
		/// </summary>
		public string Name
		{
			set { m_Name = value; }
			get { return m_Name; }
		}
        

		/// <summary>
		/// Return a friendly message about the mission
		/// </summary>
		public string MissionMessage
		{
			get
			{
				if ( m_currentStatus == RoverStatus.MissionAborted )
					return m_reasonForAbort;

				else if ( m_currentStatus == RoverStatus.MissionComplete )
					return "Mission Succesfully Completed";

				else if ( m_currentStatus == RoverStatus.Roving )
					return "Mision in Progress";

				else
					return "Mission has not started yet";
			}
		}


		/// <summary>
		/// Determinte if the rover has landed.
		/// </summary>
		public bool HasLanded
		{
			get
			{
				return (this.m_currentStatus == RoverStatus.Roving || this.m_currentStatus == RoverStatus.MissionAborted || this.m_currentStatus == RoverStatus.MissionComplete);
			}
		}


		/// <summary>
		/// When setting mission parameters, check to see when the rover is ready to start roving
		/// </summary>
		private void CheckForOperationalReadyness()
		{
			if (this.m_currentStatus == RoverStatus.InConstruction )
				if ( this.m_MissionOrders != null && this.m_currentLocation != null ) 
					m_currentStatus = RoverStatus.WaitingForDeployment;
		}


		/// <summary>
		/// Check to see if the edge of the Landing Plateau is directly in front of the Rover
		/// </summary>
		/// <returns></returns>
		private bool CheckForPlateauEdge()
		{
			bool returnValue = false;

			switch (m_currentLocation.Heading)
			{
				case CompassDirection.North :
					returnValue = ( m_currentLocation.YPosition == this.m_plateauYDimension );
					break;

				case CompassDirection.East :
					returnValue = ( m_currentLocation.XPosition == this.m_plateauXDimension );
					break;

				case CompassDirection.South :
					returnValue = ( m_currentLocation.YPosition == 0 );
					break;

				case CompassDirection.West :
					returnValue = ( m_currentLocation.XPosition == 0 );
					break;

			}

			return returnValue;
		}


		/// <summary>
		/// Check with MissonControl if there are any other Rovers in the way of this one
		/// </summary>
		/// <returns>True if an obsticle exists, otherwise false.</returns>
		private bool CheckForObstacles()
		{
            // Not Implemented yet
			return false;

		}



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
		/// Returns the Event Log from the current mission
		/// </summary>
		public string[] EventLog
		{
			get
			{
				return this.m_EventLogs.ToString().Split(Constants.EVENT_LOG_LINE_BREAK);
			}
		}

	}
}
