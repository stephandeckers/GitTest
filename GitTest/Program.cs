using System;
--v5 change

-- conflict 2
=======
--v1-1 change
namespace GitTest
{
	class Program
	{
		static void Main( string[] args )
		{
			Console.WriteLine( "Hello World!" );
			Console.WriteLine( "Hello World! - b1" );
			Console.WriteLine( "still working in b1" );
		}

		private void b1Only ( )
		{
		}

		private void masterOnly ( )
		{
		}

	}
}
