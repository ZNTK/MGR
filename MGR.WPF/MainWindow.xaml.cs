using MGR.WPF.DatabaseServices;
using MGR.WPF.MethodsServices.Filters;
using System;
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
        }
    }
}
