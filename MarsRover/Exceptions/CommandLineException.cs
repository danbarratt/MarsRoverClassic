using System;

namespace MarsRover.Exceptions
{
	/// <summary>
	/// Exception to be raised when the command line arguments can't be processed properly
	/// </summary>
	public class CommandLineException : Exception
	{

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="message"></param>
		public CommandLineException(string message) : base(message)
		{
		}

	}
}
