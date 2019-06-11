using System;
using Library;

namespace ParserApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Parser(10000).Parse(@"D:\ProjectTrash\1.xml");
            new Parser(10000).Parse(@"C:\Users\Raman_Hrytsuk\Downloads\rutracker-20190323.xml.gz");
        }
    }
}
