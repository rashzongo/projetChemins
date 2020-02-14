using projetChemins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace interfaceChemins
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Ville> villes = new ObservableCollection<Ville>();
        private Dictionary<Ville, Ellipse> pointsCarte = new Dictionary<Ville, Ellipse>();
        private int indexVilles;
        private int cheminsPerGeneration = 10;
        private int mutations = 10;
        private int xovers = 2;
        private int elites = 3;

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
            listeVilles.ItemsSource = DataBase.getDataBase().Select("SELECT * FROM Ville");
            //listeVilles.ItemsSource = this.villes;
            this.DataContext = this;

        }

        public void AddSelectedPointToList(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(canvasImageCarte);
            var newVille = new Ville("ville" + this.indexVilles++, p.X, p.Y);
            this.villes.Add(newVille);
            DataBase.getDataBase().InsertVille(newVille);
            // Add Point on canvas
            this.AddPointToCanvas(newVille, p);
        }

        private void delete_SelectedVille(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Ville laville = listeVilles.SelectedItem as Ville;
            Console.WriteLine("La ville à Supprimer "+ laville);
            if (laville != null)
            {
                this.RemovePointFromCanvas(laville);
                DataBase.getDataBase().Delete(laville);
            }
            this.villes.Remove(laville);
        }

        private void AddPointToCanvas(Ville v, Point p)
        {
            Ellipse el = new Ellipse();
            el.Width = 5;
            el.Height = 5;
            el.Fill = Brushes.Red;
            Thickness t = new Thickness(p.X, p.Y, 0, 0);
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
            Console.WriteLine("Running --------------------------");

            //Insertion des Params  
            Params NbXovers = new Params("NbXovers", this.NbXovers);
            DataBase.getDataBase().InsertParams(NbXovers);
            Params NbMutations = new Params("NbMutations", this.NbMutations);
            DataBase.getDataBase().InsertParams(NbMutations);
            Params NbElites = new Params("NbXovers", this.NbElites);
            DataBase.getDataBase().InsertParams(NbElites);
            Params NbCheminsPerGeneration = new Params("NbCheminsPerGeneration", this.NbCheminsPerGeneration);
            DataBase.getDataBase().InsertParams(NbCheminsPerGeneration);

            var generations = Algo.Launch(new List<Ville>(this.villes), this.NbCheminsPerGeneration, this.NbMutations, this.NbXovers, this.NbElites);


            int i = 0;
            foreach (Generation g in generations)
            {
                Console.WriteLine("Generation " + i++ + " : **************************************");
                Console.WriteLine(g);
            }


        }
    }
}
