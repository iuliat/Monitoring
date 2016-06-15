﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                var RAMAgent = new RAMAgent();
                RAMAgent.start();

                //var CPUAgent = new CPUAgent();
                //CPUAgent.start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex.Message);
            }


        }
    }

    

}
