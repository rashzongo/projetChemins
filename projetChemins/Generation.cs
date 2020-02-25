using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace projetChemins
{
    class Generation
    {
        private List<Chemin> chemins;

        public Generation(List<Chemin> listeChemins)
        {
            this.chemins = listeChemins;
        }

        public List<Chemin> listeChemins
        {
            get
            {
                return this.chemins;
            }
            set
            {
                this.chemins = value;
            }
        }

        public List<Chemin> GetMeilleurChemins(int numberChemins)
        {
            // Distinct pour la séléction (Xovers, mutations, elites)
            List<Chemin> sortedList = listeChemins.Distinct().ToList();
            sortedList.Sort();
            return sortedList.GetRange(0, numberChemins);
        }

        public double GetMeilleurScore()
        {
            return this.GetMeilleurChemins(1)[0].Score;
        }
        public double GetMoyenneScore()
        {
            var listeScore = from chemin in this.listeChemins select chemin.Score;
            return listeScore.Average();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Meilleur Score : " + this.GetMeilleurScore() + " - Moyenne Score : " + this.GetMoyenneScore() + "\n");
            sb.Append("{\n");
            foreach (Chemin c in this.listeChemins)
            {
                sb.Append("\t" + c);
                sb.Append("\n");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
