using System;
using System.Text;

namespace projetChemins
{
    class Ville
    {
        private string nom;
        private double x;
        private double y;

        public Ville(string nom, double x, double y)
        {
            this.nom = nom;
            this.x = x;
            this.y = y;
        }

        public string NomVille
        {
            get { return this.nom; }
            set { this.nom = value; }
        }

        public double XVille
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public double YVille
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Ville ville = (Ville)obj;

                return (this.NomVille.Equals(ville.NomVille) &&
                    this.XVille == ville.XVille && this.YVille == ville.YVille);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(" Nom : " + this.NomVille + ", ");
            sb.Append(" X : " + this.XVille + ", ");
            sb.Append(" Y : " + this.YVille);
            sb.Append("}");
            return sb.ToString();
        }
    }
}
