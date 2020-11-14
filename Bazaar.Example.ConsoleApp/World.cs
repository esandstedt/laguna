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
        private readonly static int TOWN_AGENT_COUNT = 500;
        private readonly static double TOWN_MAX_MONEY = TOWN_AGENT_COUNT * 100;

        private readonly static int ROUTE_AGENT_COUNT = 20;

        public List<Town> Towns { get; } = new List<Town>();
        public List<Route> Routes { get; } = new List<Route>();

        private IEnumerable<IAgent> Agents => this.Towns.SelectMany(x => x.Agents).Concat(this.Routes.SelectMany(x => x.Agents));
        private IEnumerable<IMarket> Markets => this.Towns.Select(x => x.Market);

        public Town AddTown(Area area)
        {
            var town = new Town(area, TOWN_AGENT_COUNT, TOWN_MAX_MONEY);
            this.Towns.Add(town);
            return town;
        }

        public Route AddRoute(Town fst, Town snd)
        {
            var route = new Route(fst, snd, ROUTE_AGENT_COUNT);
            this.Routes.Add(route);
            return route;
        }

        public void Step()
        {
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
