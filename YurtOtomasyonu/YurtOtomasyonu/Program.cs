﻿using System;
using System.Windows.Forms;
using YurtOtomasyonu.Forms;

namespace YurtOtomasyonu
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AnaForm());
        }
    }
}
