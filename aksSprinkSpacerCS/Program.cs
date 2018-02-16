using System;

namespace aksSprinkSpacer {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            SprinkSpacerWPF SpSpWPF = new SprinkSpacerWPF();
            SpSpWPF.ShowDialog();          
        }
    }
}
