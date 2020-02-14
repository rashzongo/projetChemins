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
            List<Chemin> sortedList = listeChemins.Distinct().ToList();
            sortedList.Sort();
            return sortedList.GetRange(0, numberChemins);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (Chemin c in this.listeChemins)
            {
                sb.Append(c + " : " + c.CalculScore());
                sb.Append("\n");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
