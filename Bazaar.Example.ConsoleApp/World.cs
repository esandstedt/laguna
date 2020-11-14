using Bazaar.Example.ConsoleApp.Agents;
using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar.Example.ConsoleApp
{
    public class World
    {

        public List<Town> Towns { get; } = new List<Town>();
        public List<Route> Routes { get; } = new List<Route>();

        private IEnumerable<IAgent> Agents => this.Towns.SelectMany(x => x.Agents).Concat(this.Routes.SelectMany(x => x.Agents));
        private IEnumerable<IMarket> Markets => this.Towns.Select(x => x.Market);

        public Town AddTown(string name, Area area, int agentCount = 200)
        {
            var town = new Town(name, area, agentCount, 100 * agentCount);
            this.Towns.Add(town);
            return town;
        }

        public Route AddRoute(Town fst, Town snd, int agentCount = 20)
        {
            var route = new Route(fst, snd, agentCount);
            this.Routes.Add(route);
            return route;
        }

        public void Step()
        {
            foreach (var route in this.Routes)
            {
                route.Step();
            }

            foreach (var agent in this.Agents)
            {
                agent.Perform();
                agent.SubmitOffers();
            }

            foreach (var market in this.Markets)
            {
                market.ResolveOffers();
            }

            foreach (var agent in this.Agents)
            {
                agent.HandleOfferResults();
            }

            foreach (var town in this.Towns)
            {
                town.Step();
            }
        }

    }

}
