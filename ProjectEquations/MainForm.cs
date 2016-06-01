using OxMath;
using System;
using System.Windows.Forms;

namespace ProjectEquations
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Console.WriteLine(infinint.Unconvertish(infinint.Convertish(2901020, 16), 16));
            //Generator.GetCombinations(56, 7, 2);
            //Console.WriteLine((infinint)51 * (infinint)4);
            //Console.WriteLine(infinint.Log(2, 17));
            //Console.WriteLine(Generator.GenerateSeedFromNumber(14565474322.6548));
            //Console.WriteLine(Generator.GenerateNumberFromSeed(62580310).ToString());
            //DebugGenerator(25);
        }

        public static void DebugGenerator(int number)
        {
            for (int i = 0; i < number; i++)
            {
                Console.WriteLine(i + ": " + Generator.GenerateVector3Point(i, 0));
            }
        }
    }
}
