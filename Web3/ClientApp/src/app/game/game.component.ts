import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { interval, Subscription } from 'rxjs';
import { WebSocketSubject } from 'rxjs/webSocket';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent {

  private socket$: WebSocketSubject<any> | undefined;
  subscription: Subscription | undefined;
  @Input() username: string = '';
  componentShown: string = "meni";
  botNo: number | undefined;
  gameId: string | undefined;
  gameModel: any | undefined;

  constructor(private http: HttpClient) {
    const source = interval(2000);
    this.subscription = source.subscribe(val => this.refreshGame());
    
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.username) {
      console.log('Username changed:', this.username);
    }
  }

  startGame() {
    this.componentShown = "start";
  }

  joinGame() {
    this.componentShown = "join";
  }

  meni() {
    this.componentShown = "meni";
  }


  continueGame() {
    const body = { gameId: this.gameId };
    console.log(body);
    this.http.put(environment.backendHost + 'api/Player/getGame', body)
      .subscribe(response => {
        this.processGameResponse(response);
        console.log('Get game response:', this.gameId, this.gameModel);
      });
  }

  startGameRequest() {
    const body = { username: this.username, botNo: this.botNo };
    this.http.post(environment.backendHost +'api/Player/startGame', body)
      .subscribe(response => {
        this.processGameResponse(response);
        console.log('Start game response:', this.gameId, this.gameModel);
      });
  }

  joinGameRequest() {
    const body = { username: this.username, gameId: "LongKind|" + this.gameId };
    this.http.post(environment.backendHost + 'api/Player/joinGame', body)
      .subscribe(response => {
        this.processGameResponse(response);
        console.log('Join game response:', this.gameId, this.gameModel);
      });
  }
  playCard(actorId:string, cardIndex: number, targetLeft: boolean) {
    const body = { playerId: actorId, cardIndex: cardIndex, targetLeft: targetLeft };
    this.http.put(environment.backendHost + 'api/Player/playCard', body)
      .subscribe(response => {
        this.processGameResponse(response);
        console.log('Play card game response:', this.gameId, this.gameModel);
      });
  }

  isCurrentPlayer(playerActorId: string) {
    return ('StringKind|' + this.username) === playerActorId
  }

  processGameResponse(response:any) {
    this.gameModel = response;
    this.gameId = this.gameModel["gameId"].split('|')[1];
    this.componentShown = "ingame";
  }

  isIngame() {
    return this.componentShown === "ingame";
  }
  refreshGame() {
    if (this.isIngame()) {
      console.log("Trying to refresh game!");
      this.continueGame();
    }
  }
  getPlayText(card: any, target: boolean) {
    var isDamage = this.isDamageCard(card);
    if (isDamage) {
      if (target) {
        return "Target Up";
      }
      else {
        return "Target Down";
      }
    }
    else {
      return "Play"
    }
  }
  isDamageCard(card: any) {
    var isDamage = false;
    for (var i = 0; i < card.cardEffects.length; i++) {
      if (card.cardEffects[i].cardEffectType === "Damage") {
        isDamage = true;
      }
    }
    return isDamage;
  }
  onTurnMarker(player: any) {
    if (this.gameModel.playerStates[this.gameModel.currentTurn].actorId === player.actorId) {
      return "[ON TURN]"
    }
    return "";
  }
  isGameOver() {
    var deadNo = 0;
    for (var i = 0; i < this.gameModel.playerStates.length; i++) {
      if (this.gameModel.playerStates[i].healthPoints <= 0) {
        deadNo++;
      }
    }
    return deadNo > 2;
  }
  isDead(player: any) {
    if (player.healthPoints <= 0) {
      return " - GHOST"
    }
    return "";
  }
  winnerMarker(player: any) {
    if (player.healthPoints > 0 && this.isGameOver()) {
      return "[WINNER]"
    }
    return "";
  }
  getPlayerName(player: any) {
    var parts = player.split("|");
    if (parts[0] === "LongKind") {
      return "Bot";
    }
    return parts[1];
  }

  getHistory(cards: any) {
     return cards.slice(0, 4);
  }

  mockReponse() {
    this.processGameResponse({
      "gameId": "LongKind|-6486860480432403166",
      "playerStates": [
        {
          "actorId": "LongKind|-1328232499509429184",
          "healthPoints": 10,
          "isBot": true,
          "characterType": "Lia",
          "deck": {
            "drawPile": [
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Heal",
                    "value": 2
                  }
                ]
              }
            ],
            "discardPile": [],
            "hand": [
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Heal",
                    "value": 2
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Heal",
                    "value": 2
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Heal",
                    "value": 2
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Heal",
                    "value": 2
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Heal",
                    "value": 2
                  }
                ]
              }
            ],
            "inPlay": []
          }
        },
        {
          "actorId": "LongKind|-8121273130438575018",
          "healthPoints": 10,
          "isBot": true,
          "characterType": "Sutha",
          "deck": {
            "drawPile": [
              {
                "cardEffects": [
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  }
                ]
              }
            ],
            "discardPile": [],
            "hand": [
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Draw",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 1
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Draw",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 1
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Draw",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 1
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  }
                ]
              }
            ],
            "inPlay": []
          }
        },
        {
          "actorId": "StringKind|ttt",
          "healthPoints": 10,
          "isBot": false,
          "characterType": "Sutha",
          "deck": {
            "drawPile": [
              {
                "cardEffects": [
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  }
                ]
              }
            ],
            "discardPile": [],
            "hand": [
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Draw",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 1
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Draw",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 1
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Draw",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 1
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  }
                ]
              }
            ],
            "inPlay": []
          }
        },
        {
          "actorId": "LongKind|5829187252721019942",
          "healthPoints": 10,
          "isBot": true,
          "characterType": "Sutha",
          "deck": {
            "drawPile": [
              {
                "cardEffects": [
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  }
                ]
              }
            ],
            "discardPile": [],
            "hand": [
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Draw",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 1
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Draw",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 1
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "Draw",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 1
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  }
                ]
              },
              {
                "cardEffects": [
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  },
                  {
                    "cardEffectType": "ExtraCard",
                    "value": 2
                  }
                ]
              }
            ],
            "inPlay": []
          }
        }
      ],
      "currentTurn": 0
    });
  }
}
