# ♠️ Poker Unity Game

[![Unity](https://img.shields.io/badge/Unity-6000-black.svg)](https://unity.com)
[![C#](https://img.shields.io/badge/C%23-.NET%20Standard%202.1-purple.svg)](https://learn.microsoft.com/en-us/dotnet/standard/net-standard)
[![Platform](https://img.shields.io/badge/Platform-Windows-blue.svg)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> Карточный покер на Unity, где вы сражаетесь один на один с ИИ-ботом. Реализованы все основные комбинации и упрощённая логика ставок.

<img width="2559" height="1439" alt="image" src="https://github.com/user-attachments/assets/c0f3498c-9074-4054-86ed-515ae078137a" />


---


## 🎮 О проекте

**Poker Unity Game** — это учебный проект, созданный для демонстрации навыков разработки на Unity и C#. Игра представляет собой классический Техасский Холдем против ИИ-противника с реалистичной логикой комбинаций и адаптивным поведением бота.

Проект полностью самостоятельный — от архитектуры до реализации ИИ.

---

## 🛠️ Технологический стек

| Компонент | Технология |
|-----------|------------|
| **Движок** | Unity 6000 |
| **Язык** | C# (.NET Standard 2.1) |
| **Платформа** | ПК (Windows) |
| **Архитектура** | ООП с разделением на GameManager, Deck, Card, Player, Bot, HandEvaluator |

---

## 🧠 Основные механики

| Механика | Описание |
|----------|----------|
| 🃏 **Колода** | 52 карты, случайное перемешивание (алгоритм Фишера-Йетса) |
| 🤝 **Раздача** | По 2 карманные карты игроку и боту, 5 общих карт на столе |
| 🏆 **Комбинации** | От старшей карты до флеш-рояля — все 10 комбинаций |
| 🤖 **ИИ бота** | Адаптивное поведение: чек / колл / рейз на основе силы карт |
| 💰 **Банк** | Отображение текущего банка и результата раунда |

---

## 🎬 Скриншоты

| Начало игры | Раздача и ставки | Открытие трёх карт |
|:---:|:---:|:---:|
| <img width="2559" height="1439" alt="image" src="https://github.com/user-attachments/assets/58720c21-4c33-42fe-9e18-fe6131457cfa" />
| <img width="2552" height="1439" alt="image" src="https://github.com/user-attachments/assets/bacc058d-6b80-4c65-bfe8-f6b459266b53" />
| <img width="2556" height="1437" alt="image" src="https://github.com/user-attachments/assets/0b28f228-9edd-4ed2-8663-400fd71e286e" />
|

| Победа после сброса врага | Враг идёт ва-банк | Конец игры |
|:---:|:---:|:---:|
| <img width="2559" height="1439" alt="image" src="https://github.com/user-attachments/assets/3d2d2b5a-222a-4af7-9e66-1696eca64cb0" />
| <img width="2559" height="1439" alt="image" src="https://github.com/user-attachments/assets/9fbb698b-038c-4c82-afdc-09fc0682b1ce" />
| <img width="2549" height="1439" alt="image" src="https://github.com/user-attachments/assets/6b8b492c-9fe1-41d1-99e5-95ace79c7f3a" />
|

---

## 🧩 Архитектура проекта

```mermaid
classDiagram
    class GameManager {
        -Deck deck
        -Player player
        -Bot bot
        -int pot
        +StartRound()
        +PlaceBet()
        +Showdown()
    }
    class Deck {
        -List~Card~ cards
        +Shuffle()
        +DealCard()
    }
    class Card {
        -Suit suit
        -Rank rank
        +ToString()
    }
    class HandEvaluator {
        +EvaluateHand(List~Card~ hand)
        +CompareHands()
    }
    class Player {
        -List~Card~ hand
        -int chips
        +Fold()
        +Call()
        +Raise()
    }
    class Bot {
        -List~Card~ hand
        -int chips
        +MakeDecision()
        +CalculateHandStrength()
    }

    GameManager --> Deck
    GameManager --> Player
    GameManager --> Bot
    GameManager --> HandEvaluator
    Player --> Card
    Bot --> Card
    HandEvaluator --> Card
