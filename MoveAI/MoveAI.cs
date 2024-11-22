using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using AppServiceInterfaces;
using AppCommon.Game;
using AppCommon.GameModel;
using AppCommon.GameModel.GameLogic;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using System.Fabric.Description;
using System.Collections.ObjectModel;

namespace MoveAI
{
    internal sealed class MoveAI : StatelessService, IMoveAI
    {

        private static string metricName = "DailyInstanceTarget";
        public MoveAI(StatelessServiceContext context)
            : base(context)
        { }

        public async Task<PlayCard> DetermineMove(GameState gameState, AIDificulty aIDificuly, CancellationToken cancellationToken)
        {
            switch (aIDificuly)
            {
                case AIDificulty.Dummy:
                    return new PlayCard { PlayerId = gameState.PlayerStates[gameState.CurrentTurn].ActorId, CardIndex = 0, TargetLeft = true };
                case AIDificulty.Easy:
                    int cardIndex = DetermineCardIndex(gameState);
                    bool target = DetermineTarget(gameState);
                    return new PlayCard { PlayerId = gameState.PlayerStates[gameState.CurrentTurn].ActorId, CardIndex = cardIndex, TargetLeft = target };
            }
            throw new NotImplementedException();
        }

        private bool DetermineTarget(GameState gameState)
        {
            var currentPlayer = gameState.CurrentTurn;
            var firstOnTheLeft = gameState.FindAliveNeighbour(gameState.PlayerStates[gameState.CurrentTurn], true);
            var firstOnTheRight = gameState.FindAliveNeighbour(gameState.PlayerStates[gameState.CurrentTurn], false);

            if(firstOnTheLeft?.HealthPoints < firstOnTheRight?.HealthPoints)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int DetermineCardIndex(GameState gameState)
        {
            var hand = gameState.PlayerStates[gameState.CurrentTurn].Deck.Hand;

            for (int i =0;i<hand.Count;i++)
            {
                if (hand[i].HasEffect(CardEffect.CardEffectTypes.Draw))
                {
                    return i;
                }
                if (hand[i].HasEffect(CardEffect.CardEffectTypes.ExtraCard))
                {
                    return i;
                }
            }

            return 0;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ReportDailyMetric();
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
        }

        protected override Task OnOpenAsync(CancellationToken cancellationToken)
        {
            SetupMetricsAutocaling();
            return base.OnOpenAsync(cancellationToken);
        }

        public void SetupMetricsAutocaling()
        {
            FabricClient fabric = new FabricClient();
            StatelessServiceUpdateDescription updateDescription = new StatelessServiceUpdateDescription();

            var userLoadMetric = new StatelessServiceLoadMetricDescription
            {
                Name = metricName,
                DefaultLoad = 1,
                Weight = ServiceLoadMetricWeight.High
            };
            updateDescription.Metrics = new KeyedMetricList();
            updateDescription.Metrics.Add(userLoadMetric);
            
            PartitionInstanceCountScaleMechanism scaleMechanism = new PartitionInstanceCountScaleMechanism
            {
                MinInstanceCount = 1,
                MaxInstanceCount = 5,
                ScaleIncrement = 1
            };
            
            AveragePartitionLoadScalingTrigger trigger = new AveragePartitionLoadScalingTrigger
            {
                MetricName = metricName,
                LowerLoadThreshold = 1.1,
                UpperLoadThreshold = 1.5,
                ScaleInterval = TimeSpan.FromSeconds(5)
            };
            
            ScalingPolicyDescription scalingPolicyDescription = new ScalingPolicyDescription(scaleMechanism, trigger);
            
            updateDescription.ScalingPolicies = new List<ScalingPolicyDescription>();
            updateDescription.ScalingPolicies.Add(scalingPolicyDescription);

            fabric.ServiceManager.UpdateServiceAsync(Context.ServiceName, updateDescription);
        }

        public Task ReportDailyMetric()
        {
            int val = CalculateDailyMetricBasedOnTime();

            var loadMetrics = new List<LoadMetric>
            {
                new LoadMetric(metricName, val)
            };

            ServiceEventSource.Current.ServiceMessage(Context, "Reported custom metric DailyInstanceTarget: {0}", val);
            Partition.ReportLoad(loadMetrics);
            
            return Task.CompletedTask;
        }

        public int CalculateDailyMetricBasedOnTime()
        {
            DateTime now = DateTime.Now;
            TimeSpan start = new TimeSpan(9, 0, 0);
            TimeSpan end = new TimeSpan(11, 0, 0);

            if (now.TimeOfDay >= start && now.TimeOfDay < end)
            {
                return 5;
            }
            return 1;
        }
    }
}
