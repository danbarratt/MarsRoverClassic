using System;
using System.Collections;

namespace MarsRover
{
	/// <summary>
	/// Holds a collection of Rovers
	/// </summary>
	public class RoverCollection : Hashtable
	{
		/// <summary>
		/// Rover Collection
		/// </summary>
		public RoverCollection()
		{
		}

		/// <summary>
		/// Adds a Rover to the collection against the specified key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Add(int roverIndex, Rover rover)
		{
			base.Add(roverIndex, rover);
		}

		/// <summary>
		/// Gets a rover from the collection
		/// </summary>
		public Rover this[int roverIndex]
		{
			get
			{
				return base[roverIndex] as Rover;
			}
		}

	}
}
