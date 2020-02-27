using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using algoDarwin;

namespace projetChemins
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public delegate void MyDelegate(string text);
        private ObservableCollection<Ville> villes = new ObservableCollection<Ville>();
        private Dictionary<Ville, Ellipse> pointsCarte = new Dictionary<Ville, Ellipse>();
        private int indexVilles;
        private int cheminsPerGeneration = 10;
        private int mutations = 30;
        private int xovers = 30;
        private int elites = 2;
        private const string XOVERS_PARAMS_KEY = "NbXovers";
        private const string ELITES_PARAMS_KEY = "NbElites";
        private const string MUTATIONS_PARAMS_KEY = "NbMutations";
        private const string GENERATION_PARAMS_KEY = "NbCheminsPerGeneration";
        private List<Line> cheminLines = new List<Line>();

        //public UpdateConsoleDelegate updateConsoleDelegate;

        public event PropertyChangedEventHandler PropertyChanged;

        public int NbCheminsPerGeneration
        {
            get { return this.cheminsPerGeneration; }
            set
            {
                if (this.cheminsPerGeneration != value)
                {
                    this.cheminsPerGeneration = value;
                    this.NotifyPropertyChanged("NbCheminsPerGeneration");
                }
            }
        }

        public int NbMutations
        {
            get { return this.mutations; }
            set
            {
                if (this.mutations != value)
                {
                    this.mutations = value;
                    this.NotifyPropertyChanged("NbMutations");
                }
            }
        }

        public int NbXovers
        {
            get { return this.xovers; }
            set
            {
                if (this.xovers != value)
                {
                    this.xovers = value;
                    this.NotifyPropertyChanged("NbXovers");
                }

            }
        }

        public int NbElites
        {
            get { return this.elites; }
            set
            {
                if (this.elites != value)
                {
                    this.elites = value;
                    this.NotifyPropertyChanged("NbElites");
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            DataBase.getDataBase().createTableVille();
            DataBase.getDataBase().createTableParams();
            var parameters = DataBase.getDataBase().Select_Params("SELECT * FROM Params");
            updateParameters(parameters);
            var villesEnBase = DataBase.getDataBase().Select("SELECT * FROM Ville");
            listeVilles.ItemsSource = this.villes;
            this.DataContext = this;
            InitCanvas(villesEnBase);
        }

        private void updateParameters(List<Params> parameters)
        {
            foreach(Params p in parameters)
            {
                if(p.Key == ELITES_PARAMS_KEY)
                {
                    this.NbElites = p.Value;
                }
                if (p.Key == XOVERS_PARAMS_KEY)
                {
                    this.NbXovers = p.Value;
                }
                if (p.Key == MUTATIONS_PARAMS_KEY)
                {
                    this.NbMutations = p.Value;
                }
                if (p.Key == GENERATION_PARAMS_KEY)
                {
                    this.NbCheminsPerGeneration = p.Value;
                }
                if (p.Key == "indexVilles")
                {
                    this.indexVilles = p.Value;
                }
            }
        }

        public void AddSelectedPointToList(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(canvasImageCarte);
            var newVille = new Ville("ville" + this.indexVilles++, p.X, p.Y);
            DataBase.getDataBase().InsertVille(newVille);
            // Add Point on canvas
            this.AddVille(newVille);
        }

        private void delete_SelectedVille(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Ville selectedVille = listeVilles.SelectedItem as Ville;
            if (selectedVille != null)
            {
                this.RemovePointFromCanvas(selectedVille);
                DataBase.getDataBase().Delete(selectedVille);
            }
            this.villes.Remove(selectedVille);
        }

        private void InitCanvas(List<Ville> villesEnBase)
        {
            Console.WriteLine("Initing canvas");
            foreach(Ville v in villesEnBase)
            {
                AddVille(v);
            }
        }
        private void AddVille(Ville v)
        {
            this.villes.Add(v);
            Ellipse el = new Ellipse();
            el.Width = 5;
            el.Height = 5;
            el.Fill = Brushes.Red;
            Thickness t = new Thickness((v.XVille - 2.5), (v.YVille - 2.5), 0, 0);
            el.Margin = t;
            this.pointsCarte.Add(v, el);
            canvasImageCarte.Children.Add(el);
        }

        private void RemovePointFromCanvas(Ville v)
        {
            canvasImageCarte.Children.Remove(this.pointsCarte[v]);
            this.pointsCarte.Remove(v);
        }


        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private void runAlgo(object sender, RoutedEventArgs e)
        {
            Thread algoThread = new Thread(this.exexAlgo);
            algoThread.Start();
        }

        private void saveParams()
        {
            //Insertion des Params  
            Params NbXovers = new Params(XOVERS_PARAMS_KEY, this.NbXovers);
            DataBase.getDataBase().InsertParams(NbXovers);
            Params NbMutations = new Params(MUTATIONS_PARAMS_KEY, this.NbMutations);
            DataBase.getDataBase().InsertParams(NbMutations);
            Params NbElites = new Params(ELITES_PARAMS_KEY, this.NbElites);
            DataBase.getDataBase().Update_Params(NbElites);
            Params NbCheminsPerGeneration = new Params(GENERATION_PARAMS_KEY, this.NbCheminsPerGeneration);
            DataBase.getDataBase().InsertParams(NbCheminsPerGeneration);
            Params indexVilles = new Params("indexVilles", this.indexVilles);
            DataBase.getDataBase().InsertParams(indexVilles);
        }

        private void exexAlgo()
        {
            // Supprimer le chemin dessiné
            Dispatcher.Invoke(() =>
            {
                foreach (Line l in cheminLines)
                {
                    canvasImageCarte.Children.Remove(l);
                }
                cheminLines.RemoveRange(0, cheminLines.Count);
            });

            this.saveParams();
            Console.WriteLine("Running Algo --------------------------");
            var generations = Algo.Launch(new List<Ville>(this.villes),
                this.NbCheminsPerGeneration, this.NbMutations, this.NbXovers, this.NbElites);

            Console.WriteLine("End Algo --------------------------");

            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach (Generation g in generations)
            {
                sb.Append("Generation " + i++ + " : **************************************");
                sb.Append("\n");
                sb.Append(g);
                sb.Append("\n");
            }
            Dispatcher.Invoke(() =>
            {
                UpdateSortieConsole(sb);
                //Affichage meilleur chemin
                PrintChemin(generations[generations.Count - 1].listeChemins[0]);
            });
        }

        private void PrintChemin(Chemin chemin)
        {
            for (int i = 0; i < chemin.listeVilles.Count - 1; i++)
            {
                Line line = new Line();
                line.Stroke = SystemColors.WindowFrameBrush;
                line.X1 = chemin.listeVilles[i].XVille;
                line.Y1 = chemin.listeVilles[i].YVille;
                line.X2 = chemin.listeVilles[i + 1].XVille;
                line.Y2 = chemin.listeVilles[i + 1].YVille;
                cheminLines.Add(line);
                canvasImageCarte.Children.Add(line);
            }
        }

        public delegate void monDelegate(StringBuilder sb);


        private void UpdateSortieConsole(StringBuilder sb)
        {
            affichageConsole.Text = sb.ToString();
        }
    }
}
