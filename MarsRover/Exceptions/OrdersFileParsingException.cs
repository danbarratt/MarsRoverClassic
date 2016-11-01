using System;

namespace MarsRover.Exceptions
{
	/// <summary>
	/// Summary description for OrdersFileParsingException.
	/// </summary>
	public class OrdersFileParsingException : Exception
	{

		public OrdersFileParsingException(string message) : base(message)
		{
		}

	}
}
