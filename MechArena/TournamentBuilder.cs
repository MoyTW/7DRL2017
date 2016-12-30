using RogueSharp.Random;

using MechArena.Tournament;

using System;
using System.Collections.Generic;
using System.IO;

namespace MechArena
{
    class TournamentBuilder
    {
        private static List<string> adjectives = new List<string>();
        private static List<string> cities = new List<string>();

        private static List<string> ReadFileLines(string filepath)
        {
            string line;
            List<string> lines = new List<string>();

            StreamReader file = new StreamReader(filepath);
            while ((line = file.ReadLine()) != null)
            {
                lines.Add(line);
            }
            file.Close();

            return lines;
        }

        private static List<String> GetAdjectives()
        {
            if (adjectives.Count == 0)
            {
                adjectives = ReadFileLines("Resources/Adjectives.txt");
            }
            return adjectives;
        }

        private static List<String> GetCities()
        {
            if (cities.Count == 0)
            {
                cities = ReadFileLines("Resources/Cities.txt");
            }
            return cities;
        }

        public static Competitor BuildCompetitor(IRandom rand)
        {
            var adjective = rand.RandomElement(GetAdjectives());
            var city = rand.RandomElement(GetCities());
            return new Competitor(adjective, city);
        }

        public static Schedule_Tournament BuildTournament(Competitor player, IRandom rand)
        {
            var entreants = new List<Competitor>();
            entreants.Add(player);
            for (int i = 0; i < 255; i++)
            {
                var comp = BuildCompetitor(rand);
                entreants.Add(comp);
            }
            return new Schedule_Tournament(entreants);
        }
    }
}
