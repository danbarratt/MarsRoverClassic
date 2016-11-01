using System;

namespace MarsRover.Exceptions
{
	/// <summary>
	/// Summary description for MissingInputFileException.
	/// </summary>
	public class MissingInputFileException : Exception
	{

		private string m_inputFileName;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputFilePath"></param>
		public MissingInputFileException(string inputFileName) : base()
		{
			this.m_inputFileName = inputFileName;
		}

		/// <summary>
		/// 
		/// </summary>
		public override string Message
		{
			get
			{
				return string.Format("The file containing the rover input commands could not be found. Make sure either a '{0}' file exists in the working directory of this application, or specify the location of the file using a '{1}' command line argument.", m_inputFileName, Constants.COMMAND_LINE_ARGUMENT_INPUT_FILE);
			}
		}

	}
}
