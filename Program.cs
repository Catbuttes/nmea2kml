using System;

namespace nmea2kml
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Use me correctly");
                return;
            }

            var file = args[0];
            using(var textFile = new System.IO.StreamReader(file))
            {
                var dataLine = textFile.ReadLine();
                while (!String.IsNullOrEmpty(dataLine))
                {
                    if(dataLine.StartsWith("$GPGGA"))
                    {
                        var x = new GGADecoder(dataLine);
                        Console.WriteLine(x.PrettyPrint());
                        Console.WriteLine("---------------------------------");
                    }

                    dataLine = textFile.ReadLine();
                }
            }
        }
    }
}
