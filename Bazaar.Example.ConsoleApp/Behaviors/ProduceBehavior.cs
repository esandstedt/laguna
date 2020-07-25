using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Bazaar.Example.ConsoleApp.Behaviors
{
    public class AreaProduceBehaviorOptions : ProduceBehaviorOptions
    {
        private readonly Area area;

        public AreaProduceBehaviorOptions(Area area, string commodity)
        {
            this.area = area;
            this.Commodity = commodity;
        }

        public override double BaseAmount 
        {
            get => this.area.Production[this.Commodity];
            set => throw new InvalidOperationException();
        }
    }


    public class ProduceBehaviorOptions
    {
        public string Commodity { get; set; }
        public virtual double BaseAmount { get; set; }
        public double EatFactor { get; set; }
        public double ToolsFactor { get; set; }
        public double ToolsBreakChance { get; set; }
    }

    public class ProduceBehavior : AgentBehavior
    {
        private readonly EatBehavior eat;
        private readonly ProduceBehaviorOptions options;

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
            var limit = this.options.BaseAmount * this.options.EatFactor * this.options.ToolsFactor;
            if (this.Agent.Inventory.Get(this.options.Commodity) < limit)
            {
                var amount = this.options.BaseAmount;

                if (this.eat.Eaten)
                {
                    amount *= this.options.EatFactor;
                }

                if (0 < this.Agent.Inventory.Get(Constants.Tools))
                {
                    amount *= this.options.ToolsFactor;

                    if (this.Random.NextDouble() < this.options.ToolsBreakChance)
                    {
                        this.Agent.Consume(Constants.Tools, 1);
                    }
                }

                this.Agent.Produce(this.options.Commodity, amount);

            }
            else
            {
                this.Agent.Consume(Constants.Money, 1);
            }
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(this.options.Commodity);
        }
    }
}
