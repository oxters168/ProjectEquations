using OxMath;
using System;
//using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectEquations
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Console.WriteLine(Generator.GenerateSeedFromNumber(12.8));
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
