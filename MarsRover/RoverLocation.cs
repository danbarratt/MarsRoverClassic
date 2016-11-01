using System;

namespace MarsRover
{
	/// <summary>
	/// Class to track the Location and Heading of a Rover
	/// </summary>
	public class RoverLocation
	{

		#region Private Members

		private CompassDirection m_direction;
		private int m_xPosition;
		private int m_yPosition;

		#endregion


		#region Constructors

		/// <summary>
		/// Defines the heading and direction of a Rover
		/// </summary>
		public RoverLocation()
		{
		}


		/// <summary>
		/// Defines the heading and direction of a Rover
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="xPosition"></param>
		/// <param name="yPosition"></param>
		public RoverLocation(CompassDirection direction, int xPosition, int yPosition)
		{
			m_direction = direction;
			m_xPosition = xPosition;
			m_yPosition = yPosition;
		}

		/// <summary>
		/// Defines the heading and direction of a Rover
		/// </summary>
		/// <param name="locationString">The string from which to extract the location+direction. This should be in the form '2 4 N'.</param>
		public RoverLocation(string locationString)
		{
			string[] locationStringParts = locationString.Split(' ');

			if ( locationStringParts.Length != 3 )
				throw new Exceptions.RoverLocationParseException("Something has gone awry, the input string matches the regex string for a location, but doesn't contain 3 separate parts.");

			// Determine the X+Y co-ordinates
			try
			{
				m_xPosition = int.Parse(locationStringParts[0]);
				m_yPosition = int.Parse(locationStringParts[1]);
			}
			catch (FormatException)
			{
				throw new Exceptions.RoverLocationParseException("The x+y co-ordinates could not be determined from the supplied location string.");
			}

			// Determine the orientation
			// TODO: refactor to not need to know about the enum values
			switch ( locationStringParts[2].ToUpper() )
			{
				case "W" :
                    m_direction = CompassDirection.West;
					break;
				case "E" :
					m_direction = CompassDirection.East;
					break;
				case "N" :
					m_direction = CompassDirection.North;
					break;
				case "S" :
					m_direction = CompassDirection.South;
					break;
			}

		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Current direction (Watch for null values!)
		/// </summary>
		public CompassDirection Heading
		{
			set	{ m_direction = value; }
			get { return m_direction; }
		}


		/// <summary>
		/// Y Position (0=Lower Edge)
		/// </summary>
		public int XPosition
		{
			set { m_xPosition = value; }
			get { return m_xPosition; }
		}


		/// <summary>
		/// X Postion (0=Left Edge)
		/// </summary>
		public int YPosition
		{
			set { m_yPosition = value; }
			get { return m_yPosition; }
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Returns the current direction and heading
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0} {1} {2}", 
				m_xPosition, 
				m_yPosition, 
				m_direction.ToString().Substring(0, 1)
				);
		}

		/// <summary>
		/// Generate an exact copy of this object
		/// </summary>
		/// <returns></returns>
		public RoverLocation Clone()
		{
			RoverLocation clone = new RoverLocation(this.ToString());
			return clone;
		}

		#endregion

	}
}
