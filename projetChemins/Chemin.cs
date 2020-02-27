using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;


namespace algoDarwin
{
    public class Chemin: IComparable<Chemin>
    {
        private List<Ville> villes;

        public List<Ville> listeVilles
        {
            get
            {
                return this.villes;
            }
            set
            {
                this.villes = value;
            }
        }

        public Chemin(List<Ville> liste)
        {
            this.listeVilles = liste;
        }

        public double Score
        {
            get {
                return CalculScore();
            }
        }

        public double CalculScore()
        {
            double result = 0;
            for (int i = 0; i < this.listeVilles.Count - 1; i++)
            {
                result += calculCheminEntreDeuxVille(this.listeVilles[i], this.listeVilles[i + 1]);
            }
            return result;
        }
        public double calculCheminEntreDeuxVille(Ville v1, Ville v2)
        {
            double xDiff = v1.XVille - v2.XVille;
            double yDiff= v1.YVille - v2.YVille;
            double result = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
            return result;
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Chemin chemin = (Chemin)obj;

                if (this.listeVilles.Count != chemin.listeVilles.Count)
                {
                    return false;
                }

                for (int i = 0; i < this.listeVilles.Count; i++)
                {
                    if (this.listeVilles[i] != chemin.listeVilles[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            foreach (Ville v in this.listeVilles)
            {
                sb.Append(v.NomVille);
                if(!v.Equals(this.listeVilles[this.listeVilles.Count - 1]))
                {
                    sb.Append(" -> ");
                }
            }
            sb.Append(" ] : " + this.CalculScore());
            return sb.ToString();
        }

        public int CompareTo(Chemin other)
        {
            if (other == null)
            {
                return 1;
            }
            return this.CalculScore().CompareTo(other.CalculScore());
        }

        public List<Ville> GetDoublons()
        {
            List<Ville> doublons = new List<Ville>();
            foreach (Ville v in this.listeVilles)
            {
                if (this.listeVilles.IndexOf(v) != this.listeVilles.LastIndexOf(v) && !doublons.Contains(v))
                {
                    doublons.Add(v);
                }
            }
            return doublons;
        }

        public List<Ville> GetRemaining(List<Ville> initialList)
        {
            List<Ville> remaining = new List<Ville>();
            foreach (Ville v in initialList)
            {
                if (this.listeVilles.IndexOf(v) == -1)
                {
                    remaining.Add(v);
                }
            }
            return remaining;
        }
    }
}
