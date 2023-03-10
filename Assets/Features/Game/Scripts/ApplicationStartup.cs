using Assets.Features.Cards.Scripts.Interfaces;
using Assets.Features.Command;
using Assets.Features.UI.Scripts;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Zenject;
using UnityEngine;
using System;

namespace Assets.Features.Game.Scripts
{
    public readonly struct ApplicationStartupProtocol
    {
        public readonly int NumberOfCards;
        public readonly int CardModelID;

        public ApplicationStartupProtocol(int cardModelID, int numberOfCards)
        {
            CardModelID = cardModelID;
            NumberOfCards = numberOfCards;
        }
    }
    public class ApplicationStartup : IDisposable
    {
        private const string UIMainCanvasPrefabPath = "UI/UIMainCanvas";
        private IInstantiator _instantiator;
        private UIController _uIController;
        public ApplicationStartup(IInstantiator instantiator, ApplicationStartupProtocol protocol)
        {
            _instantiator = instantiator;
            Start(protocol.NumberOfCards, protocol.CardModelID).Forget();
        }        

        private async UniTaskVoid Start(int numberOfCards, int cardModelID)
        {
            var mainCanvas = _instantiator.InstantiatePrefabResourceForComponent<UIMainCanvas>(UIMainCanvasPrefabPath);

            var createCardsResult = await _instantiator.Instantiate<CreateSameCardsCommand>(new object[] 
            { new CreateSameCardsCommandProtocol(
                mainCanvas.CardContainer,
                numberOfCards,
                cardModelID) })
                .Do();

            var cards = (List<ICardView>)createCardsResult.Body;
            _uIController = _instantiator.Instantiate<UIController>(new object[] { new UIControllerProtocol(mainCanvas, cards) });
            _uIController.Init();
            Resources.UnloadUnusedAssets();
        }

        public void Dispose()
        {
            _uIController.Dispose();
        }
    }
}