using System;

namespace RfcFacil.SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var rfc = RfcBuilder.ForNaturalPerson()
                                .WithName("ERICK ALEJANDRO")
                                .WithFirstLastName("GONZALEZ")
                                .WithSecondLastName("MARTINEZ")
                                .WithDate(1994, 01, 27)
                                .Build();

            Console.WriteLine(rfc);

            Console.ReadKey();
        }
    }
}
