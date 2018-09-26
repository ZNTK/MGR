using MGR.WPF.DatabaseServices;
using MGR.WPF.MethodsServices.Classifier;
using MGR.WPF.MethodsServices.FeatureSelection;
using MGR.WPF.MethodsServices.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void Button_Click(object sender, RoutedEventArgs e)//pearson
        {
            DatabaseService databaseService = new DatabaseService();
            
            string collectionName = CollectionName.Text;
            int featureCount = int.Parse(FeatureCount.Text);
            //int featureToSelectCount = int.Parse(FeatureToSelectCount.Text);

            PearsonCorrelation pearsonCorrelation = new PearsonCorrelation();

            var dataSet = databaseService.ConvertMongoColectionToListOfLists(featureCount, collectionName);

            var wynik = pearsonCorrelation.MakeCorelationTable(featureCount, collectionName, dataSet);

            //before your loop
            var csv = new StringBuilder();
            for (int i = 0; i < featureCount + 1; i++)
            {
                var newLine = string.Empty;
                for (int j = 0; j < featureCount + 1; j++)
                {
                    newLine += wynik[i, j].ToString() + ";";
                }
                csv.AppendLine(newLine);
            }
            //var newLine = string.Format("{0},{1}", first, second);


            //after your loop
            File.WriteAllText($"E://cos//wynikiDobreDoMGR//Pearson_{collectionName}_corelationTable_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.txt", csv.ToString());


            //FiltersHelper filtersHelper = new FiltersHelper();
            //filtersHelper.SelectFeaturesAndWriteToFile(wynik, featureToSelectCount, "Pearson_" + collectionName, featureCount);


            FeatureSelection featureSelection = new FeatureSelection();
            featureSelection.SelectFeaturesAndCheckClasification(wynik, featureCount +1, dataSet, collectionName, "Pearson");
            MessageBox.Show("Wykonano obliczenia.");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string collectionName = CollectionName.Text;
            int featureCount = int.Parse(FeatureCount.Text);
            //int featureToSelectCount = int.Parse(FeatureToSelectCount.Text);

            DatabaseService databaseService = new DatabaseService();
            var dataSet = databaseService.ConvertMongoColectionToListOfLists(featureCount, collectionName);
            SpearmanCorrelation spearmanCorrelation = new SpearmanCorrelation(); 

            var wynik = spearmanCorrelation.MakeCorelationTable(featureCount, collectionName, dataSet);

            ////before your loop
            var csv = new StringBuilder();
            for (int i = 0; i < featureCount +1; i++)
            {
                var newLine = string.Empty;
                for (int j = 0; j < featureCount +1 ; j++)
                {
                    newLine += wynik[i, j].ToString() + ";";
                }
                csv.AppendLine(newLine);
            }

            File.WriteAllText($"E://cos//wynikiDobreDoMGR//Spearman_{collectionName}_corelationTable_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.txt", csv.ToString());

            //FiltersHelper filtersHelper = new FiltersHelper();
            //filtersHelper.SelectFeaturesAndWriteToFile(wynik, featureToSelectCount, "Spearman_" + collectionName, featureCount);

            FeatureSelection featureSelection = new FeatureSelection();
            featureSelection.SelectFeaturesAndCheckClasification(wynik, featureCount +1, dataSet, collectionName, "Spearman");
            MessageBox.Show("Wykonano obliczenia.");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string collectionName = CollectionName.Text;
            int featureCount = int.Parse(FeatureCount.Text);
            //int featureToSelectCount = int.Parse(FeatureToSelectCount.Text);

            DatabaseService databaseService = new DatabaseService();
            var dataSet = databaseService.ConvertMongoColectionToListOfLists(featureCount, collectionName);
            KendallCorelation kendallCorelation = new KendallCorelation();

            var wynik = kendallCorelation.MakeCorelationTable(featureCount, collectionName, dataSet);

            var csv = new StringBuilder();
            for (int i = 0; i < featureCount +1; i++)
            {
                var newLine = string.Empty;
                for (int j = 0; j < featureCount +1; j++)
                {
                    newLine += wynik[i, j].ToString() + ";";
                }
                csv.AppendLine(newLine);
            }

            File.WriteAllText($"E://cos//wynikiDobreDoMGR//Kendall_{collectionName}_corelationTable_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.txt", csv.ToString());
            //FiltersHelper filtersHelper = new FiltersHelper();
            //filtersHelper.SelectFeaturesAndWriteToFile(wynik, featureToSelectCount, "Kendall_" + collectionName, featureCount);

            //MessageBox.Show("Wykonano obliczenia.");
            FeatureSelection featureSelection = new FeatureSelection();
            featureSelection.SelectFeaturesAndCheckClasification(wynik, featureCount +1, dataSet, collectionName, "Kendall");
            MessageBox.Show("Wykonano obliczenia.");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string collectionName = CollectionName.Text;
            int featureCount = int.Parse(FeatureCount.Text);
            //int featureToSelectCount = int.Parse(FeatureToSelectCount.Text);

            DatabaseService databaseService = new DatabaseService();
            var dataSet = databaseService.ConvertMongoColectionToListOfLists(featureCount, collectionName);

            NaiveBayesClassifier naiveBayesClassifier = new NaiveBayesClassifier();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            naiveBayesClassifier.GenerateProbabilites(dataSet, collectionName);
            stopWatch.Stop();
            //var result = naiveBayesClassifier.GetProbabilityOfFeatureValuesFormFile(collectionName);
        }
    }
}
