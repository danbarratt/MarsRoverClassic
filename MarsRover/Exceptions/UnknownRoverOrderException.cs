using System;

namespace MarsRover.Exceptions
{
	/// <summary>
	/// Summary description for UnknownRoverOrderException.
	/// </summary>
	public class UnknownRoverOrderException : Exception
	{
		private string m_unknownOrder;

		public UnknownRoverOrderException(char unknownOrder) : base()
		{
            m_unknownOrder = unknownOrder.ToString();
		}

		public override string Message
		{
			get
			{
				return string.Format("An unknown order was deteted by a Rover '{0}'", m_unknownOrder);
			}
		}

	}
}
