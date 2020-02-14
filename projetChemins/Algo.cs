using System;
using System.Collections.Generic;
using System.Linq;

namespace projetChemins
{
    static class Algo
    {
        public static List<Generation> Launch(List<Ville> listeVilles, int populationNumber,
            int mutationsPercentage, int xoverPercentage, int elitesPercentage)
        {
            // TODO : tester les params fournis
            List<Generation> generations = new List<Generation>();
            generations.Add(new Generation(GenerateRandomChemins(listeVilles, populationNumber)));

            while (checkStopCondition(new List<Generation>(generations)))
            {
                generations.Add(GetNextGeneration(generations.Last(), mutationsPercentage, xoverPercentage, elitesPercentage));
            }
            return generations;
        }

        public static bool checkStopCondition(List<Generation> generations)
        {
            return generations.Count >= 30 ? false : true;
            //STOP if meilleur score revient plus de 2* nb de ville
        }

        public static Generation GetNextGeneration(Generation genN,
            int mutationsPercentage, int xoverPercentage, int elitesPercentage)
        {
            var xovers = Algo.GenerateXOver(genN, xoverPercentage, 3);
            var mutations = Algo.GenerateMutations(genN, mutationsPercentage);
            var elites = genN.GetMeilleurChemins(elitesPercentage);

            var genPrimeChemins = new List<Chemin>(mutations);
            genPrimeChemins.AddRange(elites);
            genPrimeChemins.AddRange(xovers);

            var genPrime = new Generation(genPrimeChemins).GetMeilleurChemins(10);
            return new Generation(genPrime);
        }


        public static List<Chemin> GenerateRandomChemins(List<Ville> villes, int cheminsNumber)
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

        public static List<Chemin> GenerateXOver(Generation gen, int XOversNumber, int pivotPosition)
        {
            //TODO : test number de Xover possible ???
            List<Chemin> result = new List<Chemin>();
            while(result.Count < XOversNumber)
            {
                // Console.WriteLine("result size : " + result.Count);

                //Choisir deux chemins randoms
                int rnd1 = new Random().Next(gen.listeChemins.Count);
                int rnd2 = new Random().Next(gen.listeChemins.Count);

                // Test pour être sûr d'avoir des randoms diff
                while(rnd2 == rnd1)
                {
                    rnd2 = new Random().Next(gen.listeChemins.Count);
                }

                // Generation XOvers bruts
                List<Ville> newListVilles = new List<Ville>();
                for(int i = 0; i < pivotPosition; i++)
                {
                    newListVilles.Add(gen.listeChemins[rnd1].listeVilles[i]);
                }
                for (int i = pivotPosition; i < gen.listeChemins[0].listeVilles.Count; i++)
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
                Console.WriteLine(newChemin);
                if(!result.Contains(newChemin) && !gen.listeChemins.Contains(newChemin))
                {
                    result.Add(newChemin);
                }
            }
            return result;
        }
        public static List<Chemin> GenerateMutations(Generation gen, int mutationsNumber)
        {
            List<Chemin> result = new List<Chemin>();
            while (result.Count < mutationsNumber)
            {
                int rndCheminIndex = new Random().Next(gen.listeChemins.Count);

                //Choisir deux chemins randoms
                int rnd1 = new Random().Next(gen.listeChemins[rndCheminIndex].listeVilles.Count);
                int rnd2 = new Random().Next(gen.listeChemins[rndCheminIndex].listeVilles.Count);
                // Test pour être sûr d'avoir des randoms diff
                while (rnd2 == rnd1)
                {
                    rnd2 = new Random().Next(gen.listeChemins[rndCheminIndex].listeVilles.Count);
                }
                // faire une mutation
                List<Ville> newListVilles = new List<Ville>(gen.listeChemins[rndCheminIndex].listeVilles);
                // On stocke la 1e ville pour pas la perdre
                var tmp = gen.listeChemins[rndCheminIndex].listeVilles[rnd1];

                //On remplace la liste à l'index rnd1 avec le contenu à l'index rnd2 et l'index rnd2 celui contenu dans tmp
                newListVilles[rnd1] = newListVilles[rnd2];
                newListVilles[rnd2] = tmp;

                var newChemin = new Chemin(newListVilles);
                if (!result.Contains(newChemin) && !gen.listeChemins.Contains(newChemin))
                {
                    result.Add(newChemin);
                }
            }
            return result;
        }
    }
}
