﻿using MGR.WPF.DatabaseServices;
using MGR.WPF.MethodsServices.Filters;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MGR.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DatabaseService databaseService = new DatabaseService();
            var lista = databaseService.Get("ARCENE_TRAIN","Column1");

            var cotam = lista.Select(x => x["Column1"]);

            var listaX = databaseService.Get("ARCENE_TRAIN", "Column1");
            var listaY = databaseService.Get("ARCENE_TRAIN", "Column2");

            PearsonCorrelation pearsonCorrelation = new PearsonCorrelation();
            pearsonCorrelation.CompereTwoFeatures(listaX.Select(x => x["Column1"]).ToList(), listaY.Select(x => x["Column2"]).ToList());

            var wynik = pearsonCorrelation.MakeCorelationTable(1000, "ARCENE_TRAIN");

            //before your loop
            var csv = new StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                var newLine = string.Empty;
                for (int j = 0; j < 1000; j++)
                {
                    newLine += wynik[i, j].ToString() + ";";
                }
                csv.AppendLine(newLine);
            }
            //var newLine = string.Format("{0},{1}", first, second);
           

            //after your loop
            File.WriteAllText("E://cos//testABS.txt", csv.ToString());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DatabaseService databaseService = new DatabaseService();
            FiltersHelper filtersHelper = new FiltersHelper();

            var list = databaseService.Get("testRank1", "Column1");
            var result = filtersHelper.RankFeature(list.Select(x => (double)x["Column1"]).ToList());
        }
    }
}
