using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Bazaar.Example.ConsoleApp.Behaviors
{
    public class ProduceBehaviorOptions
    {
        public string Commodity { get; set; }
        public double BaseAmount { get; set; }
        public double EatFactor { get; set; }
        public double ToolsFactor { get; set; }
        public double ToolsBreakChance { get; set; }
    }

    public class ProduceBehavior : AgentBehavior
    {
        private readonly EatBehavior eat;
        private readonly ProduceBehaviorOptions options;

        private string commodity => this.options.Commodity;
        private double limit => this.options.BaseAmount * this.options.EatFactor * this.options.ToolsFactor;
        private double baseAmount => this.options.BaseAmount;
        private double eatFactor => this.options.EatFactor;
        private double toolsFactor => this.options.ToolsFactor;
        private double toolsBreakChance => this.options.ToolsBreakChance;

        public ProduceBehavior(
            Agent agent,
            EatBehavior eat,
            ProduceBehaviorOptions options)
            : base(agent)
        {
            this.eat = eat;
            this.options = options;
        }

        public override void Perform()
        {

            if (this.Agent.Inventory.Get(this.commodity) < this.limit)
            {
                var amount = this.baseAmount;

                if (this.eat.Eaten)
                {
                    amount *= this.eatFactor;
                }

                if (0 < this.Agent.Inventory.Get(Constants.Tools))
                {
                    amount *= this.toolsFactor;

                    if (this.Random.NextDouble() < this.toolsBreakChance)
                    {
                        this.Agent.Consume(Constants.Tools, 1);
                    }
                }

                this.Agent.Produce(this.commodity, amount);

            }
            else
            {
                this.Agent.Consume(Constants.Money, 1);
            }
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(this.commodity);
        }
    }
}
