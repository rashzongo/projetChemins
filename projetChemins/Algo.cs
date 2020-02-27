using System;
using System.Collections.Generic;
using System.Linq;

namespace algoDarwin
{
    public static class Algo
    {
        public static Random random = new Random();
        public static List<Generation> Launch(List<Ville> listeVilles, int populationNumber,
            int mutationsPercentage, int xoverPercentage, int elitesPercentage)
        {
            List<Generation> generations = new List<Generation>();
            var fisrstGen = new Generation(GenerateRandomChemins(listeVilles, populationNumber));
            generations.Add(fisrstGen);

            while (!checkStopCondition(new List<Generation>(generations)))
            {
                generations.Add(GetNextGeneration(generations.Last(), mutationsPercentage, xoverPercentage, elitesPercentage));
            }
            return generations;
        }

        // Condition d'arrêt lorsque le meilleur score revient autant de fois qu'il y a de chemins par generation
        private static bool checkStopCondition(List<Generation> generations)
        {
            if(generations.Count > generations[0].listeChemins.Count)
            {
                for(int i = generations.Count - 1; i > generations.Count - generations[0].listeChemins.Count; i--)
                {
                    if(generations[i].GetMeilleurScore() != generations.Last().GetMeilleurScore())
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static Generation GetNextGeneration(Generation genN,
            int mutationsPercentage, int xoverPercentage, int elitesPercentage)
        {
            // CrossOver avec pivotPosition la moitié du nombre de villes
            var xovers = Algo.GenerateXOver(genN, xoverPercentage);
            var mutations = Algo.GenerateMutations(genN, mutationsPercentage);
            var elites = genN.GetMeilleurChemins(elitesPercentage);

            var genPrimeChemins = new List<Chemin>(mutations);
            genPrimeChemins.AddRange(elites);
            genPrimeChemins.AddRange(xovers);

            var genPrime = new Generation(genPrimeChemins).GetMeilleurChemins(genN.listeChemins.Count);
            return new Generation(genPrime);
        }

        private static int ChoixDebutPivot(Chemin c1, Chemin c2)
        {
            int pivot = 1;
            for(int i = 0; i < c1.listeVilles.Count - 1; i++)
            {
                if(c1.listeVilles[i] != c2.listeVilles[i])
                {
                    pivot = i;
                    break;
                }
            }
            return pivot;
        }


        private static List<Chemin> GenerateRandomChemins(List<Ville> villes, int cheminsNumber)
        {
            List<Chemin> result = new List<Chemin>();
            while (result.Count < cheminsNumber)
            {
                Chemin newChemin = new Chemin(villes.OrderBy(a => Guid.NewGuid()).ToList());
                if(!result.Contains(newChemin))
                {
                    result.Add(newChemin);
                }
            }
            return result;
        }

        private static List<Chemin> GenerateXOver(Generation gen, int XOversNumber)
        {
            //TODO : test number de Xover possible ???
            List<Chemin> result = new List<Chemin>();
            while(result.Count < XOversNumber)
            {
                //Choisir deux chemins randoms
                int rnd1 = random.Next(gen.listeChemins.Count);
                int rnd2 = random.Next(gen.listeChemins.Count);
                // Test pour être sûr d'avoir des randoms diff
                while (rnd2 == rnd1)
                {
                    rnd2 = random.Next(gen.listeChemins.Count);
                }

                // Choix de pivot aléatoire optimal
                int pivotStart = ChoixDebutPivot(gen.listeChemins[rnd1], gen.listeChemins[rnd2]);
                int pivotPosition = random.Next(pivotStart, gen.listeChemins[0].listeVilles.Count);

                // Generation XOvers bruts
                List<Ville> newListVilles = new List<Ville>();
                for(int i = 0; i <= pivotPosition; i++)
                {
                    newListVilles.Add(gen.listeChemins[rnd1].listeVilles[i]);
                }
                for (int i = pivotPosition+1; i < gen.listeChemins[0].listeVilles.Count; i++)
                {
                    newListVilles.Add(gen.listeChemins[rnd2].listeVilles[i]);
                }

                // Netoyage : Suppression des doublons dans la liste des villes
                var doublons = new Chemin(newListVilles).GetDoublons();
                var remaining = new Chemin(newListVilles).GetRemaining(gen.listeChemins[0].listeVilles);

                    // Normalement doublons.Count == remaining.Count
                if(doublons.Count != remaining.Count)
                {
                    Console.Error.WriteLine("Warning : doublons.Count != remaining.Count !");
                }

                    // On parcours les doublons et on replace par les remainings
                for(int i = 0; i < doublons.Count; i++)
                {
                    newListVilles[newListVilles.LastIndexOf(doublons[i])] = remaining[i];
                }
                var newChemin = new Chemin(newListVilles);
                
                result.Add(newChemin);
            }
            return result;
        }

        private static List<Chemin> GenerateMutations(Generation gen, int mutationsNumber)
        {
            List<Chemin> result = new List<Chemin>();
            while (result.Count < mutationsNumber)
            {
                int rndCheminIndex = random.Next(gen.listeChemins.Count);

                //Choisir deux chemins randoms
                int rnd1 = random.Next(gen.listeChemins[rndCheminIndex].listeVilles.Count);
                int rnd2 = random.Next(gen.listeChemins[rndCheminIndex].listeVilles.Count);
                // Test pour être sûr d'avoir des randoms diff
                while (rnd2 == rnd1)
                {
                    rnd2 = random.Next(gen.listeChemins[rndCheminIndex].listeVilles.Count);
                }
                // faire une mutation
                List<Ville> newListVilles = new List<Ville>(gen.listeChemins[rndCheminIndex].listeVilles);
                // On stocke la 1e ville pour pas la perdre
                var tmp = gen.listeChemins[rndCheminIndex].listeVilles[rnd1];

                //On remplace la liste à l'index rnd1 avec le contenu à l'index rnd2 et l'index rnd2 celui contenu dans tmp
                newListVilles[rnd1] = newListVilles[rnd2];
                newListVilles[rnd2] = tmp;

                var newChemin = new Chemin(newListVilles);
                if (!result.Contains(newChemin))
                {
                    result.Add(newChemin);
                }
            }
            return result;
        }
    }
}
