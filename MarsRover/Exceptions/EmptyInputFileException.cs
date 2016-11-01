using System;

namespace MarsRover.Exceptions
{
	/// <summary>
	/// Summary description for EmptyInputFileException.
	/// </summary>
	public class EmptyInputFileException : Exception
	{

		private string m_inputFilePath;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputFilePath"></param>
		public EmptyInputFileException(string inputFilePath) : base(inputFilePath)
		{
			m_inputFilePath = inputFilePath;
		}

		/// <summary>
		/// 
		/// </summary>
		public override string Message
		{
			get
			{
				return string.Format("The input file '{0}' seems to be empty", m_inputFilePath);
			}
		}

	}
}
